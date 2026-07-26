using System;

namespace BattleCity
{
    public enum GamePhase
    {
        Playing,
        LevelCleared,
        Won,
        Lost,
    }

    /// <summary>Pure game state — no Unity types, fully EditMode-testable.</summary>
    public class GameState
    {
        public const int StartingLives = 3;

        public int Score { get; private set; }
        public int Lives { get; private set; } = StartingLives;
        public int EnemiesRemaining { get; private set; }
        public GamePhase Phase { get; private set; } = GamePhase.Playing;

        /// <summary>
        /// Carry score and lives forward from a previously-cleared level. Only valid before
        /// play begins (Phase == Playing); ignored otherwise so mid-game calls can't cheat.
        /// </summary>
        public void Seed(int score, int lives)
        {
            if (Phase != GamePhase.Playing)
                return;
            Score = Math.Max(0, score);
            Lives = Math.Max(0, lives);
        }

        public void AddScore(int points)
        {
            if (Phase == GamePhase.Playing && points > 0)
                Score += points;
        }

        /// <summary>Returns true if the player still has lives left.</summary>
        public bool LoseLife()
        {
            Lives = Math.Max(0, Lives - 1);
            return Lives > 0;
        }

        public void SetEnemiesRemaining(int count)
        {
            EnemiesRemaining = Math.Max(0, count);
        }

        /// <summary>Phase transitions are one-way: only Playing can move to Won/Lost.</summary>
        public void SetPhase(GamePhase phase)
        {
            if (Phase == GamePhase.Playing)
                Phase = phase;
        }
    }
}
