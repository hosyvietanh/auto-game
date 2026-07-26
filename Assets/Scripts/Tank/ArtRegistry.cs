using System.Collections.Generic;
using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Resolves sprites by name with a three-tier fallback chain so the game always runs:
    ///   1. <see cref="NesArt"/> — procedural classic-NES pixel art generated in code (default).
    ///   2. Kenney PNGs under Resources/Art/Kenney/ — silent legacy fallback if present.
    ///   3. A generated solid-color square — last resort so nothing is ever invisible.
    /// Sprite names are centralized in the Names class; NesArt keys its grids off these.
    /// </summary>
    public static class ArtRegistry
    {
        public static class Names
        {
            public const string PlayerTank = "tankBody_green";
            public const string EnemyBasic = "tankBody_sand";
            public const string EnemyFast = "tankBody_red";
            public const string EnemyArmored = "tankBody_dark";
            public const string Bullet = "bulletDark1";
            public const string Brick = "crateWood";
            public const string Steel = "crateMetal";
            public const string Eagle = "barrelRed_top";
            public const string Bush = "bush";
        }

        static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

        public static Sprite Load(string name, Color fallbackColor)
        {
            if (Cache.TryGetValue(name, out var cached))
                return cached;

            var sprite = NesArt.Get(name);
            if (sprite == null)
                sprite = Resources.Load<Sprite>("Art/Kenney/" + name);
            if (sprite == null)
            {
                Debug.LogWarning($"ArtRegistry: sprite '{name}' has no NesArt grid and is not under Resources/Art/Kenney — using solid-color fallback.");
                sprite = MakeSolidSprite(fallbackColor);
            }

            Cache[name] = sprite;
            return sprite;
        }

        /// <summary>A 1x1-unit solid color sprite (4x4 texture, PPU 4).</summary>
        public static Sprite MakeSolidSprite(Color color)
        {
            const int size = 4;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>Scales a transform so its sprite renders exactly worldSize units wide.</summary>
        public static void SetWorldSize(SpriteRenderer renderer, float worldSize)
        {
            var spriteSize = renderer.sprite.bounds.size;
            float max = Mathf.Max(spriteSize.x, spriteSize.y);
            if (max > 0f)
                renderer.transform.localScale = Vector3.one * (worldSize / max);
        }
    }
}
