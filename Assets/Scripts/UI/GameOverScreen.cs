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

            var subtitle = HUD.MakeLabel(screen.panel.transform, "Subtitle",
                new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), TextAnchor.MiddleCenter);
            subtitle.fontSize = 24;
            subtitle.text = "Press R to restart";

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
            var phase = GameManager.Instance.State.Phase;
            if (phase == GamePhase.Playing)
            {
                panel.SetActive(false);
                return;
            }

            title.text = phase == GamePhase.Won ? "VICTORY!" : "GAME OVER";
            title.color = phase == GamePhase.Won ? new Color(0.4f, 1f, 0.4f) : new Color(1f, 0.35f, 0.3f);
            panel.SetActive(true);
        }
    }
}
