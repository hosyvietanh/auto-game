namespace BattleCity
{
    /// <summary>
    /// One level's map plus its difficulty tuning. Pure data, EditMode-testable.
    /// </summary>
    public struct LevelConfig
    {
        public string[] Map;
        /// <summary>Total enemies to destroy to clear the level.</summary>
        public int TotalEnemies;
        /// <summary>Max enemies alive on screen at once.</summary>
        public int MaxConcurrent;
        /// <summary>Seconds between spawns (smaller = harder).</summary>
        public float SpawnInterval;
    }

    /// <summary>
    /// The 10 levels in play order, easy → hard. Difficulty ramps monotonically:
    /// more enemies, higher concurrency, and faster spawns as the level number rises
    /// (enforced by LevelCatalogTests). The map's own layout adds the rest of the
    /// difficulty; the enemy-type mix ramps separately in <see cref="WavePlan"/>.
    /// </summary>
    public static class LevelCatalog
    {
        public static readonly LevelConfig[] Levels =
        {
            new LevelConfig { Map = LevelDefinition.Level1,  TotalEnemies = 12, MaxConcurrent = 2, SpawnInterval = 3.5f },
            new LevelConfig { Map = LevelDefinition.Level2,  TotalEnemies = 14, MaxConcurrent = 3, SpawnInterval = 3.2f },
            new LevelConfig { Map = LevelDefinition.Level3,  TotalEnemies = 16, MaxConcurrent = 3, SpawnInterval = 3.0f },
            new LevelConfig { Map = LevelDefinition.Level4,  TotalEnemies = 16, MaxConcurrent = 4, SpawnInterval = 2.7f },
            new LevelConfig { Map = LevelDefinition.Level5,  TotalEnemies = 18, MaxConcurrent = 4, SpawnInterval = 2.5f },
            new LevelConfig { Map = LevelDefinition.Level6,  TotalEnemies = 20, MaxConcurrent = 4, SpawnInterval = 2.2f },
            new LevelConfig { Map = LevelDefinition.Level7,  TotalEnemies = 20, MaxConcurrent = 5, SpawnInterval = 2.0f },
            new LevelConfig { Map = LevelDefinition.Level8,  TotalEnemies = 22, MaxConcurrent = 5, SpawnInterval = 1.7f },
            new LevelConfig { Map = LevelDefinition.Level9,  TotalEnemies = 24, MaxConcurrent = 6, SpawnInterval = 1.4f },
            new LevelConfig { Map = LevelDefinition.Level10, TotalEnemies = 28, MaxConcurrent = 6, SpawnInterval = 1.1f },
        };

        public static int Count => Levels.Length;
    }
}
