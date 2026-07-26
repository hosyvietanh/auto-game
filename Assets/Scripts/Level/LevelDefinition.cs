namespace BattleCity
{
    public enum TileType
    {
        Empty,
        Brick,
        Steel,
        Eagle,
    }

    /// <summary>
    /// Levels are ASCII maps: one string per row, row 0 is the TOP of the screen.
    ///   '#' steel wall (indestructible)   'B' brick wall (destructible)
    ///   'E' eagle/base (lose if destroyed)  'P' player spawn   '1'-'3' enemy spawns
    ///   '.' empty floor
    /// All rows must be the same length. Exactly one 'P' and one 'E' per level.
    /// </summary>
    public static class LevelDefinition
    {
        public static readonly string[] Level1 =
        {
            "#################",
            "#1.....2......3.#",
            "#.BB.BB.B.BB.BB.#",
            "#.BB.BB.B.BB.BB.#",
            "#.BB.BB.B.BB.BB.#",
            "#.......B.......#",
            "#B.BB.B...B.BB.B#",
            "#..BB..BBB..BB..#",
            "#.......#.......#",
            "#.BB.BB...BB.BB.#",
            "#.BB.BB.B.BB.BB.#",
            "#.......B..P....#",
            "#......BBB......#",
            "#......BEB......#",
            "#################",
        };

        public static TileType CharToTile(char c)
        {
            switch (c)
            {
                case '#': return TileType.Steel;
                case 'B': return TileType.Brick;
                case 'E': return TileType.Eagle;
                default: return TileType.Empty;
            }
        }
    }
}
