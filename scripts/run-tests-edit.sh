#!/usr/bin/env bash
# Runs EditMode (fast, no scene) tests headlessly. Exit 0 = all passed.
set -uo pipefail
source "$(dirname "$0")/unity-env.sh"

LOG="$PROJECT_PATH/Logs/tests-edit.log"
RESULTS="$PROJECT_PATH/Logs/TestResults-EditMode.xml"
rm -f "$RESULTS"

echo "Running EditMode tests..."
"$UNITY" -batchmode -projectPath "$PROJECT_PATH" -logFile "$LOG" \
  -runTests -testPlatform EditMode -testResults "$RESULTS"
EXIT=$?

if [ -f "$RESULTS" ]; then
  echo "--- Test summary ---"
  passed=$(grep -c 'result="Passed"' "$RESULTS" || true)
  failed=$(grep -c 'result="Failed"' "$RESULTS" || true)
  echo "Passed entries: $passed / Failed entries: $failed"
  if [ "$failed" -gt 0 ]; then
    echo "--- Failed tests ---"
    grep -B1 -A6 'result="Failed"' "$RESULTS" | head -n 80
  fi
else
  echo "No results XML produced — likely a compile error. Tail of log:"
  grep -E "error CS|CompilerError" "$LOG" | sort -u || tail -n 30 "$LOG"
fi

exit $EXIT
