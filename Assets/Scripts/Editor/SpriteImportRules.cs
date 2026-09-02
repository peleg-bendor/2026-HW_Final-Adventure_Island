using UnityEditor;
using UnityEngine;

// Applies the import settings this project's art depends on to everything under Assets/Sprites/.
// A rule rather than a one-off fixer: settings changed by hand go back on the next reimport.
public class SpriteImportRules : AssetPostprocessor
{
    const string SpriteFolder = "Assets/Sprites/";

    // The source art sits on a 16px grid and is stored upscaled 3x, so one cell is one world unit.
    const int PixelsPerUnit = 48;

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(SpriteFolder))
            return;

        TextureImporter importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;

        // Unity defaults to Bilinear and compresses, which softens every edge of pixel art.
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;

        importer.GetSourceTextureWidthAndHeight(out _, out int height);

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        // Full Rect rather than Tight, whose mesh crops the padding that sizes frames alike.
        settings.spriteMeshType = SpriteMeshType.FullRect;

        // Centred across, so mirroring turns a sprite about its middle rather than sideways.
        settings.spriteAlignment = (int)SpriteAlignment.Custom;
        settings.spritePivot = new Vector2(0.5f, PivotY(height));

        importer.SetTextureSettings(settings);
    }

    // Anchors a sprite at the middle of its bottom cell, so feet land on a cell boundary whatever
    // the height. Center leaves every even-height sprite half a cell into the floor.
    static float PivotY(int textureHeight)
    {
        // A sprite shorter than a cell has no bottom cell to sit in, so it stays centred.
        if (textureHeight <= PixelsPerUnit)
            return 0.5f;

        return PixelsPerUnit * 0.5f / textureHeight;
    }
}
