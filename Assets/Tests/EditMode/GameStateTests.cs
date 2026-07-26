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
}
