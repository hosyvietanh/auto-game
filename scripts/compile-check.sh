#!/usr/bin/env bash
# Headless compile/import check. Fails on any C# compiler error.
set -uo pipefail
source "$(dirname "$0")/unity-env.sh"

LOG="$PROJECT_PATH/Logs/compile.log"
echo "Running Unity batch-mode compile check (first cold run can take minutes)..."
"$UNITY" -batchmode -quit -projectPath "$PROJECT_PATH" -logFile "$LOG"
EXIT=$?

if grep -qE "error CS|Scripts have compiler errors|CompilerError" "$LOG"; then
  echo "--- COMPILE ERRORS ---"
  grep -E "error CS|CompilerError" "$LOG" | sort -u
  exit 1
fi

if [ $EXIT -ne 0 ]; then
  echo "Unity exited with code $EXIT — check $LOG"
  tail -n 30 "$LOG"
  exit $EXIT
fi

echo "Compile OK"
