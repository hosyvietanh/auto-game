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

        public static readonly string[] Level2 =
        {
            "#################",
            "#1......2......3#",
            "#..BBB...BBB....#",
            "#..B.B...B.B....#",
            "#..BBB...BBB....#",
            "#...............#",
            "#.BB.BB.BB.BB.B.#",
            "#...............#",
            "#.BB.BB.BB.BB.B.#",
            "#...............#",
            "#......BBB......#",
            "#..P...B.B......#",
            "#......BBB......#",
            "#......BEB......#",
            "#################",
        };

        public static readonly string[] Level3 =
        {
            "#################",
            "#1......2......3#",
            "#.###.....###...#",
            "#.BBB.BBB.BBB.B.#",
            "#.....B.B.......#",
            "#.BB..B.B..BB.B.#",
            "#.BB.......BB.B.#",
            "#....#####.....##",
            "#.BB.......BB.B.#",
            "#.BB..B.B..BB.B.#",
            "#.....B.B......P#",
            "#.BBB.BBB.BBB.B.#",
            "#......BBB......#",
            "#......BEB......#",
            "#################",
        };

        public static readonly string[] Level4 =
        {
            "#################",
            "#1.....2.....3..#",
            "#.BB.#####.BB...#",
            "#.BB.#...#.BB.BB#",
            "#....#.#.#.....B#",
            "#.####.#.####..B#",
            "#......#........#",
            "#.####.#.#.####.#",
            "#.B....#.#....B.#",
            "#.B.####.####.BP#",
            "#.B..........B..#",
            "#.BBBB.BBB.BBBB.#",
            "#......B.B......#",
            "#......BEB......#",
            "#################",
        };

        public static readonly string[] Level5 =
        {
            "#################",
            "#1.....2.....3..#",
            "#.#.#.#.#.#.#.#.#",
            "#...............#",
            "#B#B#B#B#B#B#B#B#",
            "#...............#",
            "#.###.###.###.#.#",
            "#.B.B.B.B.B.B.B.#",
            "#.###.###.###.#.#",
            "#...............#",
            "#.#.#.#.#.#.#.#.#",
            "#....P.....B....#",
            "#.....BB.BB.....#",
            "#......BEB......#",
            "#################",
        };

        public static readonly string[] Level6 =
        {
            "#################",
            "#1.....2.....3..#",
            "#BBB.BBB.BBB.BBB#",
            "#B.B.B.B.B.B.B.B#",
            "#B.B.B.B.B.B.B.B#",
            "#...............#",
            "###.#####.#####.#",
            "#..............P#",
            "###.#####.#####.#",
            "#...............#",
            "#.BBB.BBB.BBB.B.#",
            "#.B.........B..##",
            "#.B...BBB...B...#",
            "#.....BEB......##",
            "#################",
        };

        public static readonly string[] Level7 =
        {
            "#################",
            "#1..2..3.....2.1#",
            "#.#############.#",
            "#.#...........#.#",
            "#.#.#########.#.#",
            "#.#.#.......#.#.#",
            "#.#.#.#####.#.#.#",
            "#...#.#P..#.#...#",
            "#.#.#.#####.#.#.#",
            "#.#.#.......#.#.#",
            "#.#.#########.#.#",
            "#.#...........#.#",
            "#.###.BBB.#####.#",
            "#.....BEB.......#",
            "#################",
        };

        public static readonly string[] Level8 =
        {
            "#################",
            "#1.2.3.....3.2.1#",
            "#B#B#B#B#B#B#B#B#",
            "#...............#",
            "#.###.#.#.#.###.#",
            "#.#.........#..P#",
            "#.#.#.###.#.#...#",
            "#...#.#.#.#...###",
            "#.#.#.###.#.#...#",
            "#.#.........#.#.#",
            "#.###.#.#.#.###.#",
            "#....BBB.BBB....#",
            "#..B..B.B..B...##",
            "#.....BEB.......#",
            "#################",
        };

        public static readonly string[] Level9 =
        {
            "#################",
            "#1.2.3.1.2.3.1.2#",
            "#B#B#B#B#B#B#B#B#",
            "#...............#",
            "#B#B#B#B#B#B#B#B#",
            "#...............#",
            "#.#####.#.#####.#",
            "#.....#.P.#.....#",
            "#.###.#.#.#.###.#",
            "#.#...........#.#",
            "#.#.#########.#.#",
            "#...#.BBB.#...###",
            "#.#...B.B...#...#",
            "#.....BEB.......#",
            "#################",
        };

        public static readonly string[] Level10 =
        {
            "#################",
            "#1.2.3.1.2.3.1.2#",
            "#B#B#B#B#B#B#B#B#",
            "#B#B#B#B#B#B#B#B#",
            "#...............#",
            "#.#.#.#.#.#.#.#.#",
            "#.#.#.#.#.#.#.#.#",
            "#...............#",
            "#B#B#B#B#B#B#B#B#",
            "#.......P.......#",
            "#.#####.#.#####.#",
            "#.#...#.#.#...#.#",
            "#.#.BBB...BBB.#.#",
            "#...B.BBEBB.B...#",
            "#################",
        };

        /// <summary>All levels in play order, level 1 first. Index 0 = level 1.</summary>
        public static readonly string[][] All =
        {
            Level1, Level2, Level3, Level4, Level5,
            Level6, Level7, Level8, Level9, Level10,
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
