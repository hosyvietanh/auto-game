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

        // Fraction of screen width reserved for the classic right-side sidebar HUD.
        // HUD.cs draws an opaque gray panel over this strip; we shift the camera so the
        // playfield centers in the remaining space instead of hiding behind the panel.
        public const float SidebarFraction = 0.16f;

        static void ConfigureCamera(ParsedLevel level)
        {
            var cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("GameBootstrap: no MainCamera in scene");
                return;
            }

            cam.orthographic = true;
            cam.backgroundColor = Color.black;

            // Fit the whole level regardless of aspect ratio, with a small margin.
            float halfHeight = level.Height / 2f;
            float halfWidthAsHeight = (level.Width / 2f) / cam.aspect;
            cam.orthographicSize = Mathf.Max(halfHeight, halfWidthAsHeight) + 1f;

            // Center on the level, then nudge right so the playfield sits in the left
            // (1 - SidebarFraction) of the screen, clear of the sidebar panel.
            float visibleWorldWidth = cam.orthographicSize * 2f * cam.aspect;
            float shiftX = (SidebarFraction / 2f) * visibleWorldWidth;
            cam.transform.position = new Vector3(
                (level.Width - 1) / 2f + shiftX, (level.Height - 1) / 2f, -10f);
        }
    }
}
