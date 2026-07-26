using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Progression state that must survive the scene reload used to move between levels.
    /// The single Game scene is reloaded for each level; this static holder carries the
    /// current level index, accumulated score, and remaining lives across those reloads.
    ///
    /// It is reset once per Play-mode session by <see cref="ResetForNewSession"/>, which
    /// runs via [RuntimeInitializeOnLoadMethod] on every entry to Play mode — even when
    /// Unity's "Enter Play Mode" domain reload is disabled. A SceneManager.LoadScene during
    /// progression does NOT re-fire that hook, so carried state persists exactly as intended.
    /// </summary>
    public static class GameSession
    {
        /// <summary>0-based index into <see cref="LevelCatalog.Levels"/>.</summary>
        public static int LevelIndex { get; private set; }
        public static int Score { get; private set; }
        public static int Lives { get; private set; }

        /// <summary>
        /// True when the next-built level should inherit <see cref="Score"/> and
        /// <see cref="Lives"/> (i.e. we advanced from a cleared level). False on a fresh
        /// game so the new level starts with default score/lives.
        /// </summary>
        public static bool Carrying { get; private set; }

        public static int LevelNumber => LevelIndex + 1;
        public static bool IsLastLevel => LevelIndex >= LevelCatalog.Count - 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetForNewSession()
        {
            ResetToStart();
        }

        /// <summary>Back to level 1 with a fresh score/lives — used for a full restart.</summary>
        public static void ResetToStart()
        {
            LevelIndex = 0;
            Score = 0;
            Lives = GameState.StartingLives;
            Carrying = false;
        }

        /// <summary>
        /// Advance to the next level, carrying the given score and lives forward.
        /// No-op past the last level. After this, the reloaded scene builds the next level.
        /// </summary>
        public static void AdvanceLevel(int score, int lives)
        {
            if (IsLastLevel)
                return;
            LevelIndex++;
            Score = score;
            Lives = lives;
            Carrying = true;
        }
    }
}
