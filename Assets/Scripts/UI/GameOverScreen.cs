using UnityEngine;
using UnityEngine.UI;

namespace BattleCity
{
    /// <summary>
    /// Full-screen overlay shown on win/lose. Hidden while playing.
    /// Restart (R key) is handled by GameManager; this is display-only.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        GameObject panel;
        Text title;
        Text subtitle;

        public static GameOverScreen Create(Transform canvasParent)
        {
            var screen = canvasParent.gameObject.AddComponent<GameOverScreen>();

            screen.panel = new GameObject("GameOverPanel");
            screen.panel.transform.SetParent(canvasParent, false);

            var image = screen.panel.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.65f);
            var rt = image.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            screen.title = HUD.MakeLabel(screen.panel.transform, "Title",
                new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), TextAnchor.MiddleCenter);
            screen.title.fontSize = 64;

            screen.subtitle = HUD.MakeLabel(screen.panel.transform, "Subtitle",
                new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), TextAnchor.MiddleCenter);
            screen.subtitle.fontSize = 24;

            screen.panel.SetActive(false);
            return screen;
        }

        void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Changed += Refresh;
                Refresh();
            }
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Changed -= Refresh;
        }

        void Refresh()
        {
            var gm = GameManager.Instance;
            var phase = gm.State.Phase;
            if (phase == GamePhase.Playing)
            {
                panel.SetActive(false);
                return;
            }

            switch (phase)
            {
                case GamePhase.LevelCleared:
                    title.text = $"STAGE {gm.LevelNumber} CLEARED";
                    title.color = new Color(0.4f, 1f, 0.4f);
                    subtitle.text = "Get ready...";
                    break;
                case GamePhase.Won:
                    title.text = "VICTORY!";
                    title.color = new Color(0.4f, 1f, 0.4f);
                    subtitle.text = "You cleared all levels! Press R to play again";
                    break;
                default: // Lost
                    title.text = "GAME OVER";
                    title.color = new Color(1f, 0.35f, 0.3f);
                    subtitle.text = "Press R to restart";
                    break;
            }
            panel.SetActive(true);
        }
    }
}
