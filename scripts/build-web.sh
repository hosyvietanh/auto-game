#!/usr/bin/env bash
# Builds a WebGL release of the game into Build/WebGL/ via the BuildScript editor method.
# Classic batch mode: the editor must be CLOSED (this is a one-time release build, not the
# fast dev loop). Exit 0 = build produced.
set -uo pipefail
source "$(dirname "$0")/unity-env.sh"

LOG="$PROJECT_PATH/Logs/build-web.log"
OUT="$PROJECT_PATH/Build/WebGL"
rm -rf "$OUT"

echo "Building WebGL (this can take several minutes on the first run)..."
"$UNITY" -batchmode -quit -projectPath "$PROJECT_PATH" -logFile "$LOG" \
  -executeMethod BattleCity.EditorTools.BuildScript.BuildWebGL
EXIT=$?

if grep -qE "error CS|Aborting batchmode|WebGL build FAILED" "$LOG"; then
  echo "--- ERRORS ---"
  grep -E "error CS|Aborting batchmode|WebGL build FAILED" "$LOG" | sort -u
  exit 1
fi

if [ -f "$OUT/index.html" ]; then
  echo "WebGL build ready at Build/WebGL/ ($(du -sh "$OUT" | cut -f1))"
else
  echo "ERROR: Build/WebGL/index.html not produced. Tail of log:"
  tail -n 40 "$LOG"
  exit 1
fi
exit $EXIT
