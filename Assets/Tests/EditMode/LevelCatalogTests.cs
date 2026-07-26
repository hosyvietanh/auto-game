using NUnit.Framework;
using BattleCity;

namespace BattleCity.EditModeTests
{
    /// <summary>
    /// Validates every level in the catalog so a typo in a hand-authored ASCII map is caught
    /// by tests, not by the user pressing Play. Also checks the difficulty ramp is monotonic.
    /// </summary>
    public class LevelCatalogTests
    {
        [Test]
        public void Catalog_HasTenLevels()
        {
            Assert.That(LevelCatalog.Count, Is.EqualTo(10));
            Assert.That(LevelDefinition.All.Length, Is.EqualTo(10));
        }

        [Test]
        public void EveryLevel_ParsesWithExpectedDimensions()
        {
            for (int i = 0; i < LevelCatalog.Count; i++)
            {
                var level = LevelParser.Parse(LevelCatalog.Levels[i].Map);
                Assert.That(level.Width, Is.EqualTo(17), $"level {i + 1} width");
                Assert.That(level.Height, Is.EqualTo(15), $"level {i + 1} height");
            }
        }

        [Test]
        public void EveryLevel_HasFullSteelBorder()
        {
            for (int i = 0; i < LevelCatalog.Count; i++)
            {
                var level = LevelParser.Parse(LevelCatalog.Levels[i].Map);
                for (int x = 0; x < level.Width; x++)
                {
                    Assert.That(level.Tiles[x, 0], Is.EqualTo(TileType.Steel), $"level {i + 1} bottom x={x}");
                    Assert.That(level.Tiles[x, level.Height - 1], Is.EqualTo(TileType.Steel), $"level {i + 1} top x={x}");
                }
                for (int y = 0; y < level.Height; y++)
                {
                    Assert.That(level.Tiles[0, y], Is.EqualTo(TileType.Steel), $"level {i + 1} left y={y}");
                    Assert.That(level.Tiles[level.Width - 1, y], Is.EqualTo(TileType.Steel), $"level {i + 1} right y={y}");
                }
            }
        }

        [Test]
        public void EveryLevel_HasSpawnsAndEagle()
        {
            // Parse succeeding already guarantees exactly one 'P' and one 'E' and >=1 enemy
            // spawn (LevelParser throws otherwise); assert the enemy-spawn count explicitly.
            for (int i = 0; i < LevelCatalog.Count; i++)
            {
                var level = LevelParser.Parse(LevelCatalog.Levels[i].Map);
                Assert.That(level.EnemySpawns.Count, Is.GreaterThanOrEqualTo(1), $"level {i + 1} enemy spawns");
            }
        }

        [Test]
        public void CatalogMaps_MatchDefinitionOrder()
        {
            for (int i = 0; i < LevelCatalog.Count; i++)
                Assert.That(LevelCatalog.Levels[i].Map, Is.SameAs(LevelDefinition.All[i]), $"level {i + 1} map");
        }

        [Test]
        public void Difficulty_RampsMonotonically()
        {
            var levels = LevelCatalog.Levels;
            for (int i = 1; i < levels.Length; i++)
            {
                Assert.That(levels[i].TotalEnemies, Is.GreaterThanOrEqualTo(levels[i - 1].TotalEnemies),
                    $"TotalEnemies should not drop at level {i + 1}");
                Assert.That(levels[i].MaxConcurrent, Is.GreaterThanOrEqualTo(levels[i - 1].MaxConcurrent),
                    $"MaxConcurrent should not drop at level {i + 1}");
                Assert.That(levels[i].SpawnInterval, Is.LessThanOrEqualTo(levels[i - 1].SpawnInterval),
                    $"SpawnInterval should not rise at level {i + 1}");
            }
            // And the ramp should actually move between first and last level.
            Assert.That(levels[^1].TotalEnemies, Is.GreaterThan(levels[0].TotalEnemies));
            Assert.That(levels[^1].SpawnInterval, Is.LessThan(levels[0].SpawnInterval));
        }
    }
}
