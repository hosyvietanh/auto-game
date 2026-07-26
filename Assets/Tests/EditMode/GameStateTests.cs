using NUnit.Framework;
using BattleCity;

namespace BattleCity.EditModeTests
{
    public class GameStateTests
    {
        [Test]
        public void AddScore_Accumulates()
        {
            var state = new GameState();
            state.AddScore(100);
            state.AddScore(300);
            Assert.That(state.Score, Is.EqualTo(400));
        }

        [Test]
        public void AddScore_IgnoredAfterGameEnds()
        {
            var state = new GameState();
            state.SetPhase(GamePhase.Lost);
            state.AddScore(100);
            Assert.That(state.Score, Is.EqualTo(0));
        }

        [Test]
        public void LoseLife_CountsDownAndReportsGameOver()
        {
            var state = new GameState();
            Assert.That(state.Lives, Is.EqualTo(GameState.StartingLives));
            Assert.That(state.LoseLife(), Is.True);
            Assert.That(state.LoseLife(), Is.True);
            Assert.That(state.LoseLife(), Is.False, "third death exhausts all lives");
            Assert.That(state.Lives, Is.EqualTo(0));
        }

        [Test]
        public void Phase_TransitionsAreOneWay()
        {
            var state = new GameState();
            state.SetPhase(GamePhase.Won);
            state.SetPhase(GamePhase.Lost);
            Assert.That(state.Phase, Is.EqualTo(GamePhase.Won), "phase must not change once the game ended");
        }

        [Test]
        public void EnemiesRemaining_NeverNegative()
        {
            var state = new GameState();
            state.SetEnemiesRemaining(-5);
            Assert.That(state.EnemiesRemaining, Is.EqualTo(0));
        }
    }

    public class WavePlanTests
    {
        [Test]
        public void TwentyEnemyWave_HasExpectedTypeMix()
        {
            int basic = 0, fast = 0, armored = 0;
            for (int i = 0; i < 20; i++)
            {
                switch (WavePlan.TypeForIndex(i))
                {
                    case TankType.BasicEnemy: basic++; break;
                    case TankType.FastEnemy: fast++; break;
                    case TankType.ArmoredEnemy: armored++; break;
                }
            }
            Assert.That(armored, Is.EqualTo(5), "every 4th tank is armored");
            Assert.That(fast, Is.EqualTo(5), "every 3rd (non-armored) tank is fast");
            Assert.That(basic, Is.EqualTo(10));
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
