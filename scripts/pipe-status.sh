#!/usr/bin/env bash
# Shows every Unity editor currently connected via the pipeline (port, project, version, PID, state).
# Use this to confirm the editor is up before running pipe-test.sh / pipe-command.sh.
set -uo pipefail
source "$(dirname "$0")/pipe-env.sh"

"$UNITY_CLI" status --no-banner
