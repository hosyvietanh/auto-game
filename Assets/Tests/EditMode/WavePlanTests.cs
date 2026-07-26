using NUnit.Framework;
using BattleCity;

namespace BattleCity.EditModeTests
{
    /// <summary>
    /// The enemy-type mix should ramp with the level: early levels all Basic, Fast tanks in
    /// the mid levels, Armored tanks in the later ones.
    /// </summary>
    public class WavePlanTests
    {
        static bool AnyOfType(int levelNumber, TankType type, int count = 40)
        {
            for (int i = 0; i < count; i++)
                if (WavePlan.TypeForIndex(i, levelNumber) == type)
                    return true;
            return false;
        }

        [Test]
        public void EarlyLevels_AreAllBasic()
        {
            foreach (int lvl in new[] { 1, 2 })
            {
                Assert.That(AnyOfType(lvl, TankType.FastEnemy), Is.False, $"level {lvl} should have no Fast tanks");
                Assert.That(AnyOfType(lvl, TankType.ArmoredEnemy), Is.False, $"level {lvl} should have no Armored tanks");
            }
        }

        [Test]
        public void MidLevels_IntroduceFastButNotArmored()
        {
            foreach (int lvl in new[] { 3, 4 })
            {
                Assert.That(AnyOfType(lvl, TankType.FastEnemy), Is.True, $"level {lvl} should have Fast tanks");
                Assert.That(AnyOfType(lvl, TankType.ArmoredEnemy), Is.False, $"level {lvl} should have no Armored tanks yet");
            }
        }

        [Test]
        public void LateLevels_IncludeFastAndArmored()
        {
            foreach (int lvl in new[] { 5, 7, 10 })
            {
                Assert.That(AnyOfType(lvl, TankType.FastEnemy), Is.True, $"level {lvl} should have Fast tanks");
                Assert.That(AnyOfType(lvl, TankType.ArmoredEnemy), Is.True, $"level {lvl} should have Armored tanks");
            }
        }

        [Test]
        public void AlwaysReturnsAnEnemyType()
        {
            for (int lvl = 1; lvl <= 10; lvl++)
                for (int i = 0; i < 30; i++)
                    Assert.That(WavePlan.TypeForIndex(i, lvl), Is.Not.EqualTo(TankType.Player));
        }

        [Test]
        public void TankData_ArmoredIsToughestAndSlowest()
        {
            var armored = TankData.For(TankType.ArmoredEnemy);
            var fast = TankData.For(TankType.FastEnemy);
            Assert.That(armored.Health, Is.GreaterThan(fast.Health));
            Assert.That(armored.Speed, Is.LessThan(fast.Speed));
            Assert.That(armored.ScoreValue, Is.GreaterThan(fast.ScoreValue));
        }
    }
}
