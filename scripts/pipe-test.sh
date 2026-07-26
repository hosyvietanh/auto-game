#!/usr/bin/env bash
# Runs tests against the ALREADY-OPEN editor via the com.unity.pipeline `run_tests`
# command (NO quitting, no second editor instance). Prints a pass/fail summary.
#
# NOTE: do NOT use `unity test` for this — that spawns its own batch-mode editor and
# collides with the open one ("another Unity instance is running"). The pipeline path is
# `unity command run_tests`, which executes inside the live editor.
#
# EditMode runs SYNCHRONOUSLY. PlayMode CANNOT: entering Play Mode triggers a domain reload
# that drops the sync HTTP request (the raw command returns `Mode: None, 0/0`). So for
# PlayMode this wrapper transparently adds `--async_tests` and polls `test_status` until the
# run completes — callers just say `pipe-test.sh PlayMode` and get the same summary.
#
# Usage:
#   scripts/pipe-test.sh                 # EditMode (default)
#   scripts/pipe-test.sh EditMode
#   scripts/pipe-test.sh PlayMode
#   scripts/pipe-test.sh EditMode Level  # only tests whose name matches "Level"
#   scripts/pipe-test.sh PlayMode InputBindingTests
#
# Exit 0 = all passed, 1 = failures or error.
set -uo pipefail
source "$(dirname "$0")/pipe-env.sh"

MODE="${1:-EditMode}"
FILTER="${2:-}"

require_connected_editor || exit 1

LOG="$PROJECT_PATH/Logs/pipe-test.log"
: > "$LOG"

# Row 2, column 3 of the CLI's TSV output is the result JSON (single physical line, even
# when pretty-printed — the CLI collapses newlines). Shared by both code paths.
result_json() { awk -F'\t' 'NR==2{print $3}'; }

# Summarize either JSON shape: the sync run_tests result (capitalized Summary/Results) or
# the async test_status result (lowercase summary/results). Exit 1 iff any test failed.
summarize() {
  python3 -c '
import json, sys
try:
    d = json.load(sys.stdin)
except json.JSONDecodeError as e:
    print("Result field was not valid JSON from the unity CLI:", e); sys.exit(1)
s = d.get("Summary") or d.get("summary") or {}
def pick(*keys):
    for k in keys:
        if k in s: return s[k]
    return 0
total   = pick("Total", "total")
passed  = pick("Passed", "passed")
failed  = pick("Failed", "failed")
skipped = pick("Skipped", "skipped")
mode    = d.get("Mode") or __import__("os").environ.get("MODE_LABEL", "?")
results = d.get("Results") or d.get("results") or []
print("--- {} summary: {}/{} passed, {} failed, {} skipped ---".format(mode, passed, total, failed, skipped))
if failed:
    print("--- Failures ---")
    for r in results:
        if r.get("Status") != "Passed":
            print("  [{}] {}".format(r.get("Status"), r.get("FullName")))
            msg = r.get("Message"); st = r.get("StackTrace")
            if msg: print("      " + msg.strip())
            if st:  print("      " + st.splitlines()[0].strip())
sys.exit(1 if failed else 0)
'
}

case "$(printf '%s' "$MODE" | tr '[:upper:]' '[:lower:]')" in
  playmode)
    echo "Running PlayMode tests via pipeline (async; filter=${FILTER:-none}, editor stays open)..."
    ARGS=(command run_tests --mode PlayMode --async_tests)
    [ -n "$FILTER" ] && ARGS+=(--filter "$FILTER")
    ARGS+=(--project-path "$PROJECT_PATH" --no-banner)

    START_JSON="$("$UNITY_CLI" "${ARGS[@]}" 2>>"$LOG" | result_json)"
    if ! printf '%s' "$START_JSON" | grep -q '"result": *"running"'; then
      echo "PlayMode test run did not start as expected. Result:" >&2
      printf '%s\n' "${START_JSON:-<empty>}" >&2
      echo "CLI stderr captured in: $LOG" >&2
      exit 1
    fi

    # Poll test_status. SECONDS is a bash builtin counting since shell start.
    DEADLINE=$((SECONDS + 300))
    while [ "$SECONDS" -lt "$DEADLINE" ]; do
      ST="$("$UNITY_CLI" command test_status --project-path "$PROJECT_PATH" --no-banner 2>>"$LOG" | result_json)"
      STATUS="$(printf '%s' "$ST" | python3 -c 'import json,sys
try: print(json.load(sys.stdin).get("status",""))
except Exception: print("")' 2>/dev/null)"
      case "$STATUS" in
        completed)         printf '%s' "$ST" | MODE_LABEL=PlayMode summarize; exit $? ;;
        running|"")        sleep 3 ;;
        no_tests)          echo "No tests matched (status=no_tests, filter=${FILTER:-none})." >&2; exit 1 ;;
        *)                 echo "PlayMode run ended with status=$STATUS:" >&2
                           printf '%s\n' "$ST" >&2; exit 1 ;;
      esac
    done
    echo "Timed out (>300s) waiting for PlayMode tests to complete." >&2
    echo "Poll manually: scripts/pipe-command.sh test_status" >&2
    exit 1
    ;;

  *)
    echo "Running $MODE tests via pipeline (filter=${FILTER:-none}, editor stays open)..."
    ARGS=(command run_tests --mode "$MODE")
    [ -n "$FILTER" ] && ARGS+=(--filter "$FILTER")
    ARGS+=(--project-path "$PROJECT_PATH" --no-banner)

    JSON="$("$UNITY_CLI" "${ARGS[@]}" 2>>"$LOG" | result_json)"
    if [ -z "$JSON" ]; then
      echo "Could not parse a result row (expected TSV column 3 on row 2)." >&2
      echo "This is either a CLI-level failure or an output-format change." >&2
      echo "CLI stderr captured in: $LOG" >&2
      exit 1
    fi
    printf '%s' "$JSON" | summarize; exit $?
    ;;
esac
