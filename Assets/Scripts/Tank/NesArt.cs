using System.Collections.Generic;
using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Procedurally generates the game's sprites from small pixel grids — our own
    /// classic-NES-style pixel art drawn in code (no external assets, no prefabs; see
    /// CLAUDE.md). Each sprite is baked into a Point-filtered <see cref="Texture2D"/> once
    /// and cached. <see cref="ArtRegistry.Load"/> asks here first, falling back to Kenney
    /// PNGs and then a solid-color square if a name is unknown.
    ///
    /// Grid convention: row 0 is the VISUAL TOP. Unity textures have y=0 at the BOTTOM, so
    /// rows are written flipped. Tanks are drawn facing UP; TankMotor rotates the transform
    /// for the other three directions, so one grid per tank type is enough.
    /// </summary>
    public static class NesArt
    {
        static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        /// <summary>Generated sprite for a known name, or null so the caller can fall back.</summary>
        public static Sprite Get(string name)
        {
            if (Cache.TryGetValue(name, out var cached))
                return cached;

            Sprite sprite = Build(name);
            if (sprite != null)
                Cache[name] = sprite;
            return sprite;
        }

        static Sprite Build(string name)
        {
            switch (name)
            {
                case ArtRegistry.Names.Brick:        return BuildSprite(Brick, BrickPalette);
                case ArtRegistry.Names.Steel:        return BuildSprite(Steel, SteelPalette);
                case ArtRegistry.Names.Bullet:       return BuildSprite(Bullet, BulletPalette);
                case ArtRegistry.Names.Eagle:        return BuildSprite(Eagle, EaglePalette);
                case ArtRegistry.Names.Bush:         return BuildSprite(Bush, BushPalette);
                case ArtRegistry.Names.PlayerTank:   return BuildSprite(Tank, TankPalette(PlayerBody, PlayerTread));
                case ArtRegistry.Names.EnemyBasic:   return BuildSprite(Tank, TankPalette(BasicBody, BasicTread));
                case ArtRegistry.Names.EnemyFast:    return BuildSprite(Tank, TankPalette(FastBody, FastTread));
                case ArtRegistry.Names.EnemyArmored: return BuildSprite(Tank, TankPalette(ArmoredBody, ArmoredTread));
                default:                             return null;
            }
        }

        // --- pixel-grid baker -------------------------------------------------

        static Sprite BuildSprite(string[] rows, Dictionary<char, Color32> palette)
        {
            int height = rows.Length;
            int width = rows[0].Length;
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color32[width * height];
            var clear = new Color32(0, 0, 0, 0);

            for (int r = 0; r < height; r++)
            {
                string row = rows[r];
                int texY = height - 1 - r; // flip: grid row 0 is the visual top
                for (int x = 0; x < width; x++)
                {
                    char c = x < row.Length ? row[x] : '.';
                    pixels[texY * width + x] = palette.TryGetValue(c, out var col) ? col : clear;
                }
            }

            tex.SetPixels32(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();

            int ppu = Mathf.Max(width, height); // ⇒ sprite bounds ≈ 1 unit; SetWorldSize rescales anyway
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), ppu);
        }

        static Color32 C(int r, int g, int b) => new Color32((byte)r, (byte)g, (byte)b, 255);

        // --- palettes ---------------------------------------------------------

        static readonly Dictionary<char, Color32> BrickPalette = new Dictionary<char, Color32>
        {
            { 'r', C(176, 92, 32) },  // brick face
            { 'd', C(92, 42, 12) },   // mortar / shadow
        };

        static readonly Dictionary<char, Color32> SteelPalette = new Dictionary<char, Color32>
        {
            { 'w', C(230, 230, 235) }, // highlight
            { 'l', C(150, 150, 160) }, // plate
            { 'k', C(74, 74, 84) },    // shadow / seam
        };

        static readonly Dictionary<char, Color32> BulletPalette = new Dictionary<char, Color32>
        {
            { 'w', C(245, 245, 245) },
        };

        static readonly Dictionary<char, Color32> EaglePalette = new Dictionary<char, Color32>
        {
            { 'b', C(200, 162, 84) },  // emblem body (tan/bronze)
            { 'k', C(58, 42, 16) },    // outline / eye
        };

        static readonly Dictionary<char, Color32> BushPalette = new Dictionary<char, Color32>
        {
            { 'g', C(96, 168, 48) },   // light foliage
            { 'G', C(48, 108, 24) },   // dark foliage
        };

        // Tank body / tread colors per type.
        static readonly Color32 PlayerBody   = C(228, 196, 40),  PlayerTread   = C(150, 118, 24);
        static readonly Color32 BasicBody    = C(180, 180, 184), BasicTread    = C(112, 112, 120);
        static readonly Color32 FastBody     = C(224, 224, 232), FastTread     = C(140, 140, 150);
        static readonly Color32 ArmoredBody  = C(120, 160, 56),  ArmoredTread  = C(72, 100, 30);
        static readonly Color32 Barrel       = C(74, 74, 78);

        static Dictionary<char, Color32> TankPalette(Color32 body, Color32 tread) =>
            new Dictionary<char, Color32>
            {
                { 'B', body },
                { 'T', tread },
                { 'G', Barrel },
            };

        // --- pixel grids (16x16 unless noted) ---------------------------------

        static readonly string[] Tank =
        {
            "................",
            ".......GG.......",
            ".......GG.......",
            ".TTT...GG...TTT.",
            ".TTT.BBBBBB.TTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTTBBBBBBBBTTT.",
            ".TTT.BBBBBB.TTT.",
            ".TTT.BBBBBB.TTT.",
            "................",
            "................",
        };

        static readonly string[] Brick =
        {
            "rrrrrrrdrrrrrrrd",
            "rrrrrrrdrrrrrrrd",
            "rrrrrrrdrrrrrrrd",
            "dddddddddddddddd",
            "rrrdrrrrrrrdrrrr",
            "rrrdrrrrrrrdrrrr",
            "rrrdrrrrrrrdrrrr",
            "dddddddddddddddd",
            "rrrrrrrdrrrrrrrd",
            "rrrrrrrdrrrrrrrd",
            "rrrrrrrdrrrrrrrd",
            "dddddddddddddddd",
            "rrrdrrrrrrrdrrrr",
            "rrrdrrrrrrrdrrrr",
            "rrrdrrrrrrrdrrrr",
            "dddddddddddddddd",
        };

        static readonly string[] Steel =
        {
            "wwwwwwwwwwwwwwww",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wkkkkkkkkkkkkkkk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "wllllllklllllllk",
            "kkkkkkkkkkkkkkkk",
        };

        static readonly string[] Bush =
        {
            ".gGg.gGg.gGg.gG.",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            "gGgGgGgGgGgGgGgG",
            "GgGgGgGgGgGgGgGg",
            ".Gg.gGg.gGg.gGg.",
        };

        static readonly string[] Eagle =
        {
            "................",
            ".......k........",
            ".......bb.......",
            "......bkbb......",
            "...b..bbbb..b...",
            "..bbb.bbbb.bbb..",
            ".bbbbbbbbbbbbbb.",
            ".bbbbbbbbbbbbbb.",
            "..bbbbbbbbbbbb..",
            "...bbbbbbbbbb...",
            "....bbbbbbbb....",
            ".....bbbbbb.....",
            "......bbbb......",
            ".....b.bb.b.....",
            "................",
            "................",
        };

        static readonly string[] Bullet = // 8x8
        {
            "...ww...",
            "..wwww..",
            ".wwwwww.",
            "wwwwwwww",
            "wwwwwwww",
            ".wwwwww.",
            "..wwww..",
            "...ww...",
        };
    }
}
