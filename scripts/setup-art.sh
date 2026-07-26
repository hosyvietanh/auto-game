#!/usr/bin/env bash
# Downloads Kenney's CC0 "Top-down Tanks Redux" pack and copies the sprites the game
# uses into Assets/Resources/Art/Kenney/. Safe to re-run (idempotent).
# The game falls back to generated solid-color sprites if art is missing, so a failed
# download never blocks development.
set -uo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
DEST="$PROJECT_PATH/Assets/Resources/Art/Kenney"
TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

PAGE_URL="https://kenney.nl/assets/top-down-tanks-redux"

echo "Resolving download URL from $PAGE_URL ..."
ZIP_URL=$(curl -sL --fail "$PAGE_URL" | grep -oE 'https?://[^"]*top-down-tanks-redux[^"]*\.zip|/media/pages/assets/top-down-tanks-redux/[^"]*\.zip' | head -n 1 || true)

if [ -z "$ZIP_URL" ]; then
  echo "ERROR: Could not find a .zip download link on $PAGE_URL"
  echo "Download the pack manually from that page and unzip its PNG folder into: $DEST"
  exit 1
fi

case "$ZIP_URL" in
  /*) ZIP_URL="https://kenney.nl$ZIP_URL" ;;
esac

echo "Downloading $ZIP_URL ..."
curl -sL --fail -o "$TMP/pack.zip" "$ZIP_URL"
unzip -q "$TMP/pack.zip" -d "$TMP/pack"

mkdir -p "$DEST"
# The pack layout is PNG/Default size/<sprites>.png (plus Retina). Copy the default-size PNGs.
PNG_DIR=$(find "$TMP/pack" -type d -name "Default size" | head -n 1)
if [ -z "$PNG_DIR" ]; then
  PNG_DIR=$(find "$TMP/pack" -type d -iname "PNG" | head -n 1)
fi
if [ -z "$PNG_DIR" ]; then
  echo "ERROR: No PNG directory found in the downloaded pack. Contents:"
  find "$TMP/pack" -maxdepth 3 -type d
  exit 1
fi

find "$PNG_DIR" -name "*.png" -exec cp {} "$DEST/" \;
COUNT=$(ls "$DEST"/*.png 2>/dev/null | wc -l | tr -d ' ')
echo "Copied $COUNT PNGs to $DEST"
echo "License: CC0 (https://kenney.nl) — no attribution required."
