using UnityEditor;
using UnityEngine;

namespace BattleCity.EditorTools
{
    /// <summary>
    /// Forces pixel-art-friendly sprite import settings on everything under
    /// Assets/Resources/Art/ so downloaded Kenney PNGs need no manual configuration.
    /// </summary>
    class ArtImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/Art/"))
                return;

            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.spritePixelsPerUnit = 64f;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        }
    }
}
