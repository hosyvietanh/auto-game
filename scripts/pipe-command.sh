#!/usr/bin/env bash
# Lists or runs commands exposed by the com.unity.pipeline package on the connected editor.
#
# Usage:
#   scripts/pipe-command.sh                 # list available commands (also: `unity list`)
#   scripts/pipe-command.sh <command> [args...]   # run a command against the open editor
#
# The set of available commands is defined by the pipeline package + any commands the
# project registers. Run with no args first to discover what this editor exposes — e.g.
# asset refresh, a scene rebuild we register, entering play mode, reading the console.
set -uo pipefail
source "$(dirname "$0")/pipe-env.sh"

require_connected_editor || exit 1

if [ "$#" -eq 0 ]; then
  echo "Available pipeline commands on the connected editor:"
  "$UNITY_CLI" command --project-path "$PROJECT_PATH" --no-banner
else
  "$UNITY_CLI" command "$@" --project-path "$PROJECT_PATH" --no-banner
fi
