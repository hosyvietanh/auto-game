using UnityEditor;

namespace BattleCity.EditorTools
{
    /// <summary>
    /// Writes the physics layer names used by the game into ProjectSettings/TagManager.asset.
    /// Runs automatically whenever the editor (or a batch-mode run) loads this assembly,
    /// so the committed TagManager stays in sync with LayerConfig's hard-coded indices.
    /// </summary>
    public static class LayerSetup
    {
        static readonly (int index, string name)[] Layers =
        {
            (8, "PlayerTank"),
            (9, "EnemyTank"),
            (10, "PlayerBullet"),
            (11, "EnemyBullet"),
            (12, "BrickWall"),
            (13, "SteelWall"),
            (14, "Base"),
        };

        [InitializeOnLoadMethod]
        [MenuItem("Tools/BattleCity/Setup Layers")]
        public static void EnsureLayers()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (assets.Length == 0)
                return;

            var tagManager = new SerializedObject(assets[0]);
            var layersProp = tagManager.FindProperty("layers");
            bool changed = false;
            foreach (var (index, name) in Layers)
            {
                var prop = layersProp.GetArrayElementAtIndex(index);
                if (prop.stringValue != name)
                {
                    prop.stringValue = name;
                    changed = true;
                }
            }

            if (changed)
                tagManager.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
