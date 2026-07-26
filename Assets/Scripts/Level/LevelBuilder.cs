using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Instantiates a parsed level's tiles under a single "Level" parent GameObject.
    /// </summary>
    public static class LevelBuilder
    {
        public static GameObject Build(ParsedLevel level)
        {
            var root = new GameObject("Level");
            for (int x = 0; x < level.Width; x++)
            {
                for (int y = 0; y < level.Height; y++)
                {
                    var pos = new Vector2(x, y);
                    switch (level.Tiles[x, y])
                    {
                        case TileType.Brick:
                            TileFactory.CreateBrick(pos, root.transform);
                            break;
                        case TileType.Steel:
                            TileFactory.CreateSteel(pos, root.transform);
                            break;
                        case TileType.Eagle:
                            TileFactory.CreateEagle(pos, root.transform);
                            break;
                        case TileType.Bush:
                            TileFactory.CreateBush(pos, root.transform);
                            break;
                    }
                }
            }
            return root;
        }
    }
}
