#!/usr/bin/env bash
# Generates Assets/Scenes/Game.unity via the SceneBuilder editor script.
set -uo pipefail
source "$(dirname "$0")/unity-env.sh"

LOG="$PROJECT_PATH/Logs/create-scene.log"
"$UNITY" -batchmode -quit -projectPath "$PROJECT_PATH" -logFile "$LOG" \
  -executeMethod BattleCity.EditorTools.SceneBuilder.CreateGameScene
EXIT=$?

if grep -qE "error CS|Aborting batchmode" "$LOG"; then
  echo "--- ERRORS ---"
  grep -E "error CS|Aborting batchmode" "$LOG" | sort -u
  exit 1
fi

if [ -f "$PROJECT_PATH/Assets/Scenes/Game.unity" ]; then
  echo "Game.unity created."
else
  echo "ERROR: Game.unity was not created. Tail of log:"
  tail -n 30 "$LOG"
  exit 1
fi
exit $EXIT
