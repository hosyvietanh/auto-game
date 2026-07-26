using NUnit.Framework;
using UnityEngine;
using BattleCity;

namespace BattleCity.EditModeTests
{
    public class LevelParserTests
    {
        [Test]
        public void Level1_HasExpectedDimensions()
        {
            var level = LevelParser.Parse(LevelDefinition.Level1);
            Assert.That(level.Width, Is.EqualTo(17));
            Assert.That(level.Height, Is.EqualTo(15));
        }

        [Test]
        public void Level1_BorderIsAllSteel()
        {
            var level = LevelParser.Parse(LevelDefinition.Level1);
            for (int x = 0; x < level.Width; x++)
            {
                Assert.That(level.Tiles[x, 0], Is.EqualTo(TileType.Steel), $"bottom border at x={x}");
                Assert.That(level.Tiles[x, level.Height - 1], Is.EqualTo(TileType.Steel), $"top border at x={x}");
            }
            for (int y = 0; y < level.Height; y++)
            {
                Assert.That(level.Tiles[0, y], Is.EqualTo(TileType.Steel), $"left border at y={y}");
                Assert.That(level.Tiles[level.Width - 1, y], Is.EqualTo(TileType.Steel), $"right border at y={y}");
            }
        }

        [Test]
        public void Level1_HasSpawnsAndEagle()
        {
            var level = LevelParser.Parse(LevelDefinition.Level1);
            Assert.That(level.PlayerSpawn, Is.EqualTo(new Vector2(11, 3)));
            Assert.That(level.EaglePosition, Is.EqualTo(new Vector2(8, 1)));
            Assert.That(level.EnemySpawns.Count, Is.EqualTo(3));
        }

        [Test]
        public void Level1_EnemySpawnsAreOnTopRow()
        {
            var level = LevelParser.Parse(LevelDefinition.Level1);
            foreach (var spawn in level.EnemySpawns)
                Assert.That(spawn.y, Is.EqualTo(level.Height - 2), $"spawn {spawn} should be just below the top border");
        }

        [Test]
        public void Parse_RejectsRaggedRows()
        {
            var map = new[] { "####", "#P#", "####" };
            Assert.Throws<System.ArgumentException>(() => LevelParser.Parse(map));
        }

        [Test]
        public void Parse_RejectsMissingPlayerOrEagle()
        {
            Assert.Throws<System.ArgumentException>(
                () => LevelParser.Parse(new[] { "####", "#1E#", "####" }), "missing player should throw");
            Assert.Throws<System.ArgumentException>(
                () => LevelParser.Parse(new[] { "####", "#1P#", "####" }), "missing eagle should throw");
        }
    }
}
