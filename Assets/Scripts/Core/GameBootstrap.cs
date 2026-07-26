using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Entry point living on the Bootstrap GameObject in Game.unity — the only
    /// behaviour referenced by the scene. Builds the entire game from code:
    /// physics config, level, camera framing, managers, player, enemies, UI.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Tooltip("Total enemies the player must destroy to win.")]
        public int TotalEnemies = 20;
        public int MaxConcurrentEnemies = 4;
        public float SpawnInterval = 3f;

        void Awake()
        {
            LayerConfig.Setup();

            var level = LevelParser.Parse(LevelDefinition.Level1);
            LevelBuilder.Build(level);
            ConfigureCamera(level);

            var gm = new GameObject("GameManager").AddComponent<GameManager>();

            var spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
            spawner.Configure(level.EnemySpawns, TotalEnemies, MaxConcurrentEnemies, SpawnInterval);

            gm.Initialize(level, spawner);

            var hud = HUD.Create();
            GameOverScreen.Create(hud.transform);
        }

        static void ConfigureCamera(ParsedLevel level)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("GameBootstrap: no MainCamera in scene");
                return;
            }

            cam.orthographic = true;
            cam.backgroundColor = new Color(0.07f, 0.07f, 0.07f);
            cam.transform.position = new Vector3((level.Width - 1) / 2f, (level.Height - 1) / 2f, -10f);

            // Fit the whole level regardless of aspect ratio, with a small margin.
            float halfHeight = level.Height / 2f;
            float halfWidthAsHeight = (level.Width / 2f) / cam.aspect;
            cam.orthographicSize = Mathf.Max(halfHeight, halfWidthAsHeight) + 1f;
        }
    }
}
