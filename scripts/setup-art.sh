#!/usr/bin/env bash
# Downloads Kenney's CC0 "Top-down Tanks Redux" pack and copies the sprites the game
# uses into Assets/Resources/Art/Kenney/. Safe to re-run (idempotent).
#
# The game falls back to generated solid-color sprites if art is missing, so a failed
# download never blocks development.
#
# Canonical source (CC0): https://kenney.nl/assets/top-down-tanks-redux
# We download from the OpenGameArt mirror because kenney.nl's download is behind a
# donation interstitial that isn't scriptable. Both are the same CC0 pack.
set -uo pipefail

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
DEST="$PROJECT_PATH/Assets/Resources/Art/Kenney"
ZIP_URL="https://opengameart.org/sites/default/files/kenney_topdownTanksRedux.zip"
UA="Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36"

# Sprites referenced by ArtRegistry.Names — keep this list in sync with that file.
SPRITES=(
  tankBody_green
  tankBody_sand
  tankBody_red
  tankBody_dark
  bulletDark1
  crateWood
  crateMetal
  barrelRed_top
)

TMP="$(mktemp -d)"
trap 'rm -rf "$TMP"' EXIT

echo "Downloading $ZIP_URL ..."
if ! curl -sL -A "$UA" --fail -o "$TMP/pack.zip" "$ZIP_URL"; then
  echo "ERROR: download failed. The game still runs with solid-color fallback sprites."
  echo "To add art manually, download the pack from https://kenney.nl/assets/top-down-tanks-redux"
  echo "and copy these PNGs into $DEST :"
  printf '  %s.png\n' "${SPRITES[@]}"
  exit 1
fi

unzip -q "$TMP/pack.zip" -d "$TMP/pack"

PNG_DIR="$TMP/pack/PNG/Default size"
if [ ! -d "$PNG_DIR" ]; then
  PNG_DIR="$(find "$TMP/pack" -type d -iname 'Default size' | head -n 1)"
fi
if [ ! -d "$PNG_DIR" ]; then
  echo "ERROR: could not locate the PNG folder inside the pack. Layout:"
  find "$TMP/pack" -maxdepth 2 -type d
  exit 1
fi

mkdir -p "$DEST"
copied=0
for name in "${SPRITES[@]}"; do
  if [ -f "$PNG_DIR/$name.png" ]; then
    cp "$PNG_DIR/$name.png" "$DEST/"
    copied=$((copied + 1))
  else
    echo "WARN: '$name.png' not found in pack (game will use a fallback for it)"
  fi
done

echo "Copied $copied/${#SPRITES[@]} sprites to $DEST"
echo "License: CC0 (https://kenney.nl) — attribution not required."
