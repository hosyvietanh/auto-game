using NUnit.Framework;
using BattleCity;

namespace BattleCity.EditModeTests
{
    /// <summary>
    /// GameSession carries level/score/lives across the per-level scene reload. Static state
    /// is shared between tests, so each test resets it first (mirrors the per-Play reset that
    /// [RuntimeInitializeOnLoadMethod] performs at runtime).
    /// </summary>
    public class GameSessionTests
    {
        [SetUp]
        public void Reset() => GameSession.ResetToStart();

        [Test]
        public void ResetToStart_IsLevelOneFullLivesNoCarry()
        {
            GameSession.AdvanceLevel(999, 1); // dirty it
            GameSession.ResetToStart();

            Assert.That(GameSession.LevelIndex, Is.EqualTo(0));
            Assert.That(GameSession.LevelNumber, Is.EqualTo(1));
            Assert.That(GameSession.Score, Is.EqualTo(0));
            Assert.That(GameSession.Lives, Is.EqualTo(GameState.StartingLives));
            Assert.That(GameSession.Carrying, Is.False);
            Assert.That(GameSession.IsLastLevel, Is.False);
        }

        [Test]
        public void AdvanceLevel_IncrementsAndCarriesScoreAndLives()
        {
            GameSession.AdvanceLevel(1500, 2);

            Assert.That(GameSession.LevelIndex, Is.EqualTo(1));
            Assert.That(GameSession.LevelNumber, Is.EqualTo(2));
            Assert.That(GameSession.Score, Is.EqualTo(1500));
            Assert.That(GameSession.Lives, Is.EqualTo(2));
            Assert.That(GameSession.Carrying, Is.True);
        }

        [Test]
        public void AdvanceLevel_ReachesLastLevelThenStops()
        {
            int score = 0;
            for (int i = 0; i < LevelCatalog.Count - 1; i++)
                GameSession.AdvanceLevel(score += 100, 3);

            Assert.That(GameSession.LevelIndex, Is.EqualTo(LevelCatalog.Count - 1));
            Assert.That(GameSession.IsLastLevel, Is.True);

            int scoreAtLast = GameSession.Score;
            GameSession.AdvanceLevel(99999, 1); // past the end — no-op
            Assert.That(GameSession.LevelIndex, Is.EqualTo(LevelCatalog.Count - 1));
            Assert.That(GameSession.Score, Is.EqualTo(scoreAtLast));
        }

        [Test]
        public void GameStateSeed_CarriesScoreAndLivesBeforePlay()
        {
            var state = new GameState();
            state.Seed(2500, 2);
            Assert.That(state.Score, Is.EqualTo(2500));
            Assert.That(state.Lives, Is.EqualTo(2));
        }

        [Test]
        public void GameStateSeed_IgnoredOnceNotPlaying()
        {
            var state = new GameState();
            state.SetPhase(GamePhase.Lost);
            state.Seed(2500, 2);
            Assert.That(state.Score, Is.EqualTo(0));
            Assert.That(state.Lives, Is.EqualTo(GameState.StartingLives));
        }
    }
}
