using UnityEngine;
using UnityEngine.UI;

namespace BattleCity
{
    /// <summary>
    /// Classic Battle City right-side sidebar HUD, built entirely from code (no scene/prefab
    /// UI, legacy uGUI — no TMP dependency). Shows the remaining-enemy tank icons, the score,
    /// player lives (label + tank icon + count) and the current stage number, on an opaque
    /// gray panel. The camera is shifted left in <see cref="GameBootstrap"/> so the playfield
    /// sits clear of this panel.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        const int MaxEnemyIcons = 20; // classic cap: 2 columns x 10 rows
        static readonly Color PanelColor = new Color(0.42f, 0.42f, 0.42f);
        static readonly Color IconTint = new Color(0.12f, 0.12f, 0.12f); // dark tank silhouettes
        static readonly Color LabelColor = Color.black;

        Text scoreText;
        Text livesText;
        Text stageText;
        Image[] enemyIcons;

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
            float panelWidth = GameBootstrap.SidebarFraction * 1280f;
            hud.BuildSidebar(go.transform, panelWidth);
            return hud;
        }

        void BuildSidebar(Transform canvas, float panelWidth)
        {
            // Opaque gray panel pinned to the right edge, full height.
            var panel = new GameObject("Sidebar").AddComponent<Image>();
            panel.transform.SetParent(canvas, false);
            panel.color = PanelColor;
            var prt = panel.rectTransform;
            prt.anchorMin = new Vector2(1f, 0f);
            prt.anchorMax = new Vector2(1f, 1f);
            prt.pivot = new Vector2(1f, 0.5f);
            prt.sizeDelta = new Vector2(panelWidth, 0f);
            prt.anchoredPosition = Vector2.zero;

            Transform p = panel.transform;

            // Enemy tank icons: 2-column grid near the top, one per enemy left to defeat.
            var enemySprite = ArtRegistry.Load(ArtRegistry.Names.EnemyBasic, Color.gray);
            enemyIcons = new Image[MaxEnemyIcons];
            const float icon = 26f, stepX = 34f, stepY = 30f, gridTop = -24f;
            float colCenter = panelWidth / 2f;
            for (int i = 0; i < MaxEnemyIcons; i++)
            {
                int col = i % 2;
                int row = i / 2;
                float x = (col == 0 ? -stepX / 2f : stepX / 2f);
                float y = gridTop - row * stepY;
                var img = MakeIcon(p, "Enemy" + i, enemySprite, icon, new Vector2(0.5f, 1f),
                    new Vector2(x, y), IconTint);
                enemyIcons[i] = img;
            }

            // Score.
            scoreText = MakeLabel(p, "Score", 20, new Vector2(0.5f, 1f),
                new Vector2(0f, gridTop - 10f * stepY - 24f), TextAnchor.UpperCenter);

            // Player lives: "IP" label, a tank icon, and the count.
            float livesY = gridTop - 10f * stepY - 78f;
            MakeLabel(p, "IPLabel", 22, new Vector2(0.5f, 1f), new Vector2(0f, livesY),
                TextAnchor.UpperCenter).text = "IP";
            MakeIcon(p, "LifeIcon", ArtRegistry.Load(ArtRegistry.Names.PlayerTank, Color.yellow),
                30f, new Vector2(0.5f, 1f), new Vector2(-18f, livesY - 34f), IconTint);
            livesText = MakeLabel(p, "Lives", 26, new Vector2(0.5f, 1f),
                new Vector2(22f, livesY - 30f), TextAnchor.UpperLeft);

            // Stage / flag number, anchored to the bottom of the panel.
            MakeLabel(p, "StageLabel", 20, new Vector2(0.5f, 0f), new Vector2(0f, 66f),
                TextAnchor.LowerCenter).text = "STAGE";
            stageText = MakeLabel(p, "Stage", 34, new Vector2(0.5f, 0f), new Vector2(0f, 20f),
                TextAnchor.LowerCenter);
        }

        static Image MakeIcon(Transform parent, string name, Sprite sprite, float size,
            Vector2 anchor, Vector2 offset, Color tint)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = tint;
            img.preserveAspect = true;
            var rt = img.rectTransform;
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = offset;
            rt.sizeDelta = new Vector2(size, size);
            return img;
        }

        static Text MakeLabel(Transform parent, string name, int fontSize, Vector2 anchor,
            Vector2 offset, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.color = LabelColor;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var rt = text.rectTransform;
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = offset;
            rt.sizeDelta = new Vector2(180f, 40f);
            return text;
        }

        void Update()
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            scoreText.text = gm.State.Score.ToString();
            livesText.text = gm.State.Lives.ToString();
            stageText.text = gm.LevelNumber.ToString();

            int remaining = Mathf.Clamp(gm.State.EnemiesRemaining, 0, MaxEnemyIcons);
            for (int i = 0; i < enemyIcons.Length; i++)
                enemyIcons[i].enabled = i < remaining;
        }
    }
}
