using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Factory methods for level tiles. Each builds a complete GameObject from code —
    /// this project uses no prefabs (see CLAUDE.md).
    /// Tiles are 1x1 Unity units, positioned at integer coordinates.
    /// </summary>
    public static class TileFactory
    {
        const int SortWalls = 10;
        const int SortEagle = 15;
        const int SortBush = 35; // above tanks (20) and bullets (30): tanks hide under foliage

        public static GameObject CreateBrick(Vector2 pos, Transform parent)
        {
            var go = CreateTile("Brick", pos, parent,
                ArtRegistry.Load(ArtRegistry.Names.Brick, new Color(0.65f, 0.35f, 0.15f)),
                LayerConfig.BrickWall, SortWalls);
            var destructible = go.AddComponent<Destructible>();
            destructible.Health = 1;
            return go;
        }

        public static GameObject CreateSteel(Vector2 pos, Transform parent)
        {
            return CreateTile("Steel", pos, parent,
                ArtRegistry.Load(ArtRegistry.Names.Steel, new Color(0.6f, 0.6f, 0.65f)),
                LayerConfig.SteelWall, SortWalls);
        }

        public static GameObject CreateEagle(Vector2 pos, Transform parent)
        {
            var go = CreateTile("Eagle", pos, parent,
                ArtRegistry.Load(ArtRegistry.Names.Eagle, new Color(0.95f, 0.8f, 0.2f)),
                LayerConfig.Base, SortEagle);
            var destructible = go.AddComponent<Destructible>();
            destructible.Health = 1;
            go.AddComponent<BaseController>();
            return go;
        }

        public static GameObject CreateBush(Vector2 pos, Transform parent)
        {
            // Decorative only: no collider and no Destructible, so tanks and bullets pass
            // straight through. Drawn above tanks (SortBush) so tanks appear to hide in it.
            var go = new GameObject("Bush");
            go.transform.SetParent(parent, false);
            go.transform.position = pos;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = ArtRegistry.Load(ArtRegistry.Names.Bush, new Color(0.2f, 0.55f, 0.15f));
            renderer.sortingOrder = SortBush;
            ArtRegistry.SetWorldSize(renderer, 1f);
            return go;
        }

        static GameObject CreateTile(string name, Vector2 pos, Transform parent,
            Sprite sprite, int layer, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.layer = layer;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            ArtRegistry.SetWorldSize(renderer, 1f);

            var collider = go.AddComponent<BoxCollider2D>();
            // Collider must be exactly 1 unit regardless of sprite scale.
            var scale = go.transform.localScale.x;
            collider.size = Vector2.one / (scale > 0f ? scale : 1f);

            return go;
        }
    }
}
