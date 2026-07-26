#!/usr/bin/env bash
# Shared environment for the NEW Unity CLI (pipeline) scripts. Source this; do not run directly.
#
# Unlike unity-env.sh (classic batch mode, which REQUIRES the editor closed), these
# scripts drive an editor that is ALREADY OPEN via the com.unity.pipeline package.
# So there is no lockfile guard here — an open editor is expected, not an error.

# The standalone `unity` CLI (installed via the beta channel), not the editor binary.
UNITY_CLI="${UNITY_CLI:-$HOME/.unity/bin/unity}"
if [ ! -x "$UNITY_CLI" ]; then
  if command -v unity >/dev/null 2>&1; then
    UNITY_CLI="$(command -v unity)"
  else
    echo "ERROR: Unity CLI not found at \$HOME/.unity/bin/unity or on PATH." >&2
    echo "Install it: curl -fsSL https://public-cdn.cloud.unity3d.com/hub/prod/cli/install.sh | UNITY_CLI_CHANNEL=beta bash" >&2
    exit 1
  fi
fi

PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
mkdir -p "$PROJECT_PATH/Logs"

# Fail fast with a friendly message if no editor is connected via the pipeline.
require_connected_editor() {
  local rows
  rows="$("$UNITY_CLI" status --format tsv --no-banner 2>/dev/null | tail -n +2)"
  if [ -z "$rows" ]; then
    echo "No Unity editor is connected via the pipeline." >&2
    echo "Open this project in the Unity editor (with com.unity.pipeline installed) and wait for it to finish compiling, then retry." >&2
    return 1
  fi
  return 0
}
