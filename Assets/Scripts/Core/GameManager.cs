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

        public void Initialize(ParsedLevel parsedLevel, EnemySpawner spawner)
        {
            level = parsedLevel;
            State.SetEnemiesRemaining(spawner.TotalEnemies);
            spawner.EnemyKilled += OnEnemyKilled;
            spawner.AllEnemiesDefeated += TriggerWin;
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

        public void TriggerWin()
        {
            State.SetPhase(GamePhase.Won);
            Changed?.Invoke();
        }

        public void TriggerLose()
        {
            State.SetPhase(GamePhase.Lost);
            Changed?.Invoke();
        }

        void Update()
        {
            if (State.Phase == GamePhase.Playing)
                return;

            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
