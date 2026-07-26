#!/usr/bin/env bash
# Runs tests against the ALREADY-OPEN editor via the com.unity.pipeline `run_tests`
# command (NO quitting, no second editor instance). Synchronous: prints a pass/fail summary.
#
# NOTE: do NOT use `unity test` for this — that spawns its own batch-mode editor and
# collides with the open one ("another Unity instance is running"). The pipeline path is
# `unity command run_tests`, which executes inside the live editor.
#
# Usage:
#   scripts/pipe-test.sh                 # EditMode (default)
#   scripts/pipe-test.sh EditMode
#   scripts/pipe-test.sh PlayMode
#   scripts/pipe-test.sh EditMode Level  # only tests whose name matches "Level"
#
# Exit 0 = all passed, 1 = failures or error.
set -uo pipefail
source "$(dirname "$0")/pipe-env.sh"

MODE="${1:-EditMode}"
FILTER="${2:-}"

require_connected_editor || exit 1

ARGS=(command run_tests --mode "$MODE")
[ -n "$FILTER" ] && ARGS+=(--filter "$FILTER")
ARGS+=(--project-path "$PROJECT_PATH" --no-banner)

echo "Running $MODE tests via pipeline (filter=${FILTER:-none}, editor stays open)..."
LOG="$PROJECT_PATH/Logs/pipe-test.log"
RAW="$("$UNITY_CLI" "${ARGS[@]}" 2>"$LOG")"

# The data row is: <command>\t<success>\t<result-json>\t<params-json>. Result JSON has no tabs.
# This assumes the run_tests output is a 1-line header + 1 data row (com.unity.pipeline
# 0.4.0-exp.1). If the CLI's TSV shape changes, JSON comes out empty — the guard below
# dumps the raw output so a format change is distinguishable from a CLI-level failure.
JSON="$(printf '%s\n' "$RAW" | awk -F'\t' 'NR==2{print $3}')"

if [ -z "$JSON" ]; then
  echo "Could not parse a result row (expected TSV column 3 on row 2)." >&2
  echo "This is either a CLI-level failure or an output-format change. Raw stdout:" >&2
  printf '%s\n' "$RAW" | head -20 >&2
  echo "CLI stderr captured in: $LOG" >&2
  exit 1
fi

printf '%s' "$JSON" | python3 -c '
import json, sys
try:
    d = json.load(sys.stdin)
except json.JSONDecodeError as e:
    print("Result field was not valid JSON from the unity CLI:", e)
    sys.exit(1)
s = d.get("Summary") or {}
mode = d.get("Mode", "?")
total, passed, failed, skipped = s.get("Total",0), s.get("Passed",0), s.get("Failed",0), s.get("Skipped",0)
dur = d.get("Duration", 0)
print("--- {} summary: {}/{} passed, {} failed, {} skipped ({}s) ---".format(mode, passed, total, failed, skipped, dur))
if failed:
    print("--- Failures ---")
    for r in d.get("Results") or []:
        if r.get("Status") != "Passed":
            print("  [{}] {}".format(r.get("Status"), r.get("FullName")))
            msg = r.get("Message")
            st = r.get("StackTrace")
            if msg: print("      " + msg.strip())
            if st:  print("      " + st.splitlines()[0].strip())
sys.exit(1 if failed else 0)
'
