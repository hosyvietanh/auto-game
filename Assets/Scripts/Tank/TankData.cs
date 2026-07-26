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

    /// <summary>Decides which enemy type spawns at each index of the wave. Pure C#.</summary>
    public static class WavePlan
    {
        public static TankType TypeForIndex(int index)
        {
            if ((index + 1) % 4 == 0) return TankType.ArmoredEnemy;
            if ((index + 1) % 3 == 0) return TankType.FastEnemy;
            return TankType.BasicEnemy;
        }
    }
}
