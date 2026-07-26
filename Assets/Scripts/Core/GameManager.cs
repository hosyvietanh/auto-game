using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace BattleCity
{
    /// <summary>
    /// Owns the GameState, the player's life cycle, and win/lose transitions.
    /// Created and initialized by GameBootstrap; UI reads Instance/State and
    /// listens to Changed.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState State { get; private set; } = new GameState();

        /// <summary>1-based number of the level currently being played.</summary>
        public int LevelNumber { get; private set; } = 1;

        /// <summary>Raised whenever score/lives/phase change.</summary>
        public event Action Changed;

        ParsedLevel level;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Initialize(ParsedLevel parsedLevel, EnemySpawner spawner, int levelNumber)
        {
            level = parsedLevel;
            LevelNumber = levelNumber;

            // Carry score/lives forward from the previous level (no-op on a fresh game).
            if (GameSession.Carrying)
                State.Seed(GameSession.Score, GameSession.Lives);

            State.SetEnemiesRemaining(spawner.TotalEnemies);
            spawner.EnemyKilled += OnEnemyKilled;
            spawner.AllEnemiesDefeated += OnAllEnemiesDefeated;
            SpawnPlayer();
        }

        void SpawnPlayer()
        {
            var player = TankFactory.CreatePlayer(level.PlayerSpawn);
            player.GetComponent<Destructible>().Destroyed += _ => OnPlayerDied();
        }

        void OnPlayerDied()
        {
            bool hasLivesLeft = State.LoseLife();
            if (hasLivesLeft)
                StartCoroutine(RespawnAfterDelay());
            else
                State.SetPhase(GamePhase.Lost);
            Changed?.Invoke();
        }

        IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(1f);
            if (State.Phase == GamePhase.Playing)
                SpawnPlayer();
        }

        void OnEnemyKilled(int score)
        {
            State.AddScore(score);
            State.SetEnemiesRemaining(State.EnemiesRemaining - 1);
            Changed?.Invoke();
        }

        /// <summary>
        /// All enemies cleared. On the last level this wins the whole game; otherwise the
        /// level is marked cleared and, after a short pause, the next level is loaded with
        /// score and lives carried forward.
        /// </summary>
        void OnAllEnemiesDefeated()
        {
            if (GameSession.IsLastLevel)
            {
                State.SetPhase(GamePhase.Won);
                Changed?.Invoke();
            }
            else
            {
                State.SetPhase(GamePhase.LevelCleared);
                Changed?.Invoke();
                StartCoroutine(AdvanceToNextLevel());
            }
        }

        IEnumerator AdvanceToNextLevel()
        {
            yield return new WaitForSeconds(2.5f);
            GameSession.AdvanceLevel(State.Score, State.Lives);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void TriggerLose()
        {
            State.SetPhase(GamePhase.Lost);
            Changed?.Invoke();
        }

        void Update()
        {
            // While playing, or during the brief auto-advance after clearing a level,
            // there is nothing to restart.
            if (State.Phase == GamePhase.Playing || State.Phase == GamePhase.LevelCleared)
                return;

            // Won (whole game) or Lost: R restarts a fresh game from level 1.
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            {
                GameSession.ResetToStart();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
