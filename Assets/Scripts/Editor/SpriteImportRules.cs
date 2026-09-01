using UnityEditor;
using UnityEngine;

// Applies the import settings this project's art depends on to everything under Assets/Sprites/,
// every time one of them is imported. It is a rule rather than a one-off fixer: a sprite whose
// settings get changed by hand goes back to these on its next reimport. It deliberately says
// nothing about the pivot, which belongs with the level tools rather than here.
public class SpriteImportRules : AssetPostprocessor
{
    const string SpriteFolder = "Assets/Sprites/";

    // The source art sits on a 16px grid and is stored upscaled 3x, so one grid cell is 48px
    // and one world unit.
    const int PixelsPerUnit = 48;

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(SpriteFolder))
            return;

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;

        // Unity defaults to Bilinear and compresses, which softens every edge and mangles flat
        // colour. Point sampling and no compression are what keep pixel art looking drawn.
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;

        // Full Rect rather than Tight. The transparent padding around each sprite is what holds
        // every frame of one character to the same size, and a Tight mesh crops it straight off.
        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);
    }
}
