using UnityEngine;

namespace BattleCity
{
    /// <summary>
    /// Entry point living on the Bootstrap GameObject in Game.unity — the only
    /// behaviour referenced by the scene. Builds the entire game from code:
    /// physics config, level, camera framing, managers, player, enemies, UI.
    ///
    /// Which level is built comes from <see cref="GameSession.LevelIndex"/>: the scene is
    /// reloaded per level, and GameSession carries the current level + score + lives across
    /// those reloads.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        void Awake()
        {
            LayerConfig.Setup();

            int levelIndex = Mathf.Clamp(GameSession.LevelIndex, 0, LevelCatalog.Count - 1);
            int levelNumber = levelIndex + 1;
            var config = LevelCatalog.Levels[levelIndex];

            var level = LevelParser.Parse(config.Map);
            LevelBuilder.Build(level);
            ConfigureCamera(level);

            var gm = new GameObject("GameManager").AddComponent<GameManager>();

            var spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
            spawner.Configure(level.EnemySpawns, config.TotalEnemies, config.MaxConcurrent, config.SpawnInterval, levelNumber);

            gm.Initialize(level, spawner, levelNumber);

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
