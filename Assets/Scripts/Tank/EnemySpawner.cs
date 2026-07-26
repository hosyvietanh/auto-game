using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Spawns the level's enemy quota over time, capped at MaxAlive concurrent tanks,
    /// rotating through the map's spawn points. Raises events consumed by GameManager.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public int TotalEnemies { get; private set; } = 20;
        public int MaxAlive { get; private set; } = 4;
        public float SpawnInterval { get; private set; } = 3f;
        public int LevelNumber { get; private set; } = 1;

        /// <summary>Fired with the score value of each killed enemy.</summary>
        public event Action<int> EnemyKilled;
        public event Action AllEnemiesDefeated;

        IReadOnlyList<Vector2> spawnPoints;
        bool configured;
        int spawned;
        int alive;
        int killed;
        float timer;

        public void Configure(IReadOnlyList<Vector2> points, int totalEnemies, int maxAlive, float spawnInterval, int levelNumber)
        {
            spawnPoints = points;
            TotalEnemies = totalEnemies;
            MaxAlive = maxAlive;
            SpawnInterval = spawnInterval;
            LevelNumber = levelNumber;
            timer = 1f; // first tank appears quickly
            configured = true;
        }

        void Update()
        {
            if (!configured || spawned >= TotalEnemies || alive >= MaxAlive)
                return;
            if (GameManager.Instance != null && GameManager.Instance.State.Phase != GamePhase.Playing)
                return;

            timer -= Time.deltaTime;
            if (timer > 0f)
                return;
            timer = SpawnInterval;

            var pos = spawnPoints[spawned % spawnPoints.Count];
            var type = WavePlan.TypeForIndex(spawned, LevelNumber);
            int score = TankData.For(type).ScoreValue;
            var enemy = TankFactory.CreateEnemy(pos, type);
            enemy.GetComponent<Destructible>().Destroyed += _ => OnEnemyDestroyed(score);
            spawned++;
            alive++;
        }

        void OnEnemyDestroyed(int score)
        {
            alive--;
            killed++;
            EnemyKilled?.Invoke(score);
            if (killed >= TotalEnemies)
                AllEnemiesDefeated?.Invoke();
        }
    }
}
