#!/usr/bin/env bash
# Publishes Build/WebGL/ to the repo's gh-pages branch (GitHub Pages serves it from root).
# Re-runnable: force-pushes a single-commit gh-pages branch built from the current output.
# Does NOT change repo visibility or enable Pages — those are one-time setup steps done
# separately (see the migration notes / README).
set -euo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$PROJECT_PATH/Build/WebGL"
REMOTE_URL="$(git -C "$PROJECT_PATH" remote get-url origin)"

if [ ! -f "$OUT/index.html" ]; then
  echo "ERROR: no build at Build/WebGL/index.html — run scripts/build-web.sh first." >&2
  exit 1
fi

# .nojekyll stops GitHub Pages' Jekyll from dropping files/folders it considers special.
touch "$OUT/.nojekyll"

# Build a throwaway git repo in the output dir and force-push it as gh-pages.
TMPGIT="$(mktemp -d)"
trap 'rm -rf "$TMPGIT"' EXIT
export GIT_DIR="$TMPGIT" GIT_WORK_TREE="$OUT"

git init -q
git checkout -q -b gh-pages
git add -A
git -c user.name="deploy-bot" -c user.email="deploy@local" commit -q -m "Deploy WebGL build"
echo "Pushing WebGL build to gh-pages on $REMOTE_URL ..."
git push -f "$REMOTE_URL" gh-pages:gh-pages

echo "Pushed. GitHub Pages will publish from the gh-pages branch shortly."
