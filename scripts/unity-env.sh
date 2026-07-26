#!/usr/bin/env bash
# Shared environment for Unity CLI scripts. Source this; do not run directly.

UNITY_VERSION="6000.2.10f1"
UNITY="${UNITY_BIN:-/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity}"
PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [ ! -x "$UNITY" ]; then
  echo "ERROR: Unity binary not found at: $UNITY" >&2
  echo "Install Unity ${UNITY_VERSION} via Unity Hub, or set UNITY_BIN to the correct path." >&2
  exit 1
fi

if [ -f "$PROJECT_PATH/Temp/UnityLockfile" ]; then
  echo "ERROR: The Unity editor has this project open (Temp/UnityLockfile exists)." >&2
  echo "Close the Unity editor before running CLI commands." >&2
  exit 1
fi

mkdir -p "$PROJECT_PATH/Logs"
