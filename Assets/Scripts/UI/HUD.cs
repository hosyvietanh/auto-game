using UnityEngine;
using UnityEngine.UI;

namespace BattleCity
{
    /// <summary>
    /// Score / lives / enemies-remaining labels. The whole canvas is built from code —
    /// no scene or prefab UI. Uses legacy uGUI Text (no TMP dependency).
    /// </summary>
    public class HUD : MonoBehaviour
    {
        Text scoreText;
        Text livesText;
        Text enemiesText;

        public static HUD Create()
        {
            var go = new GameObject("HUD");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            go.AddComponent<GraphicRaycaster>();

            var hud = go.AddComponent<HUD>();
            hud.scoreText = MakeLabel(go.transform, "Score", new Vector2(0f, 1f), new Vector2(20f, -16f), TextAnchor.UpperLeft);
            hud.livesText = MakeLabel(go.transform, "Lives", new Vector2(0.5f, 1f), new Vector2(0f, -16f), TextAnchor.UpperCenter);
            hud.enemiesText = MakeLabel(go.transform, "Enemies", new Vector2(1f, 1f), new Vector2(-20f, -16f), TextAnchor.UpperRight);
            return hud;
        }

        internal static Text MakeLabel(Transform parent, string name, Vector2 anchor, Vector2 offset, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.fontStyle = FontStyle.Bold;
            text.color = Color.white;
            text.alignment = alignment;

            var rt = text.rectTransform;
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = offset;
            rt.sizeDelta = new Vector2(360f, 40f);
            return text;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            scoreText.text = $"SCORE {gm.State.Score}";
            livesText.text = $"LIVES {gm.State.Lives}";
            enemiesText.text = $"ENEMIES {gm.State.EnemiesRemaining}";
        }
    }
}
