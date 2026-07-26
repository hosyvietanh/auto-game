namespace BattleCity
{
    public enum TankType
    {
        Player,
        BasicEnemy,
        FastEnemy,
        ArmoredEnemy,
    }

    /// <summary>Per-tank-type stats. Pure C#, EditMode-testable.</summary>
    public class TankData
    {
        public TankType Type;
        public float Speed;
        public float FireInterval;
        public int Health;
        public int ScoreValue;

        public static TankData For(TankType type)
        {
            switch (type)
            {
                case TankType.Player:
                    return new TankData { Type = type, Speed = 3.5f, FireInterval = 0.4f, Health = 1, ScoreValue = 0 };
                case TankType.FastEnemy:
                    return new TankData { Type = type, Speed = 4.0f, FireInterval = 1.8f, Health = 1, ScoreValue = 200 };
                case TankType.ArmoredEnemy:
                    return new TankData { Type = type, Speed = 2.0f, FireInterval = 2.2f, Health = 3, ScoreValue = 300 };
                default:
                    return new TankData { Type = TankType.BasicEnemy, Speed = 2.5f, FireInterval = 2.0f, Health = 1, ScoreValue = 100 };
            }
        }
    }

    /// <summary>
    /// Decides which enemy type spawns at each index of a level's wave. Pure C#.
    /// The mix ramps with the level number: early levels are all Basic tanks; Fast tanks
    /// join in the mid levels and Armored tanks in the later ones, both growing more
    /// frequent toward level 10.
    /// </summary>
    public static class WavePlan
    {
        public static TankType TypeForIndex(int index, int levelNumber)
        {
            int n = index + 1;

            // Levels 1-2: gentle introduction — only Basic tanks.
            if (levelNumber <= 2)
                return TankType.BasicEnemy;

            // Levels 3-4: Fast tanks appear; still no Armored.
            if (levelNumber <= 4)
                return (n % 4 == 0) ? TankType.FastEnemy : TankType.BasicEnemy;

            // Levels 5-6: Armored tanks join a steadier stream of Fast tanks.
            if (levelNumber <= 6)
            {
                if (n % 5 == 0) return TankType.ArmoredEnemy;
                if (n % 3 == 0) return TankType.FastEnemy;
                return TankType.BasicEnemy;
            }

            // Levels 7-8: tougher mix.
            if (levelNumber <= 8)
            {
                if (n % 4 == 0) return TankType.ArmoredEnemy;
                if (n % 3 == 0) return TankType.FastEnemy;
                return TankType.BasicEnemy;
            }

            // Levels 9-10: relentless — every other tank Fast, every third Armored.
            if (n % 3 == 0) return TankType.ArmoredEnemy;
            if (n % 2 == 0) return TankType.FastEnemy;
            return TankType.BasicEnemy;
        }
    }
}
