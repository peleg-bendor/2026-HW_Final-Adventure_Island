using UnityEditor;
using UnityEngine;

// Applies the import settings this project's art depends on to everything under Assets/Sprites/,
// every time one of them is imported. It is a rule rather than a one-off fixer: a sprite whose
// settings get changed by hand goes back to these on its next reimport.
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

        importer.GetSourceTextureWidthAndHeight(out _, out int height);

        TextureImporterSettings settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);

        // Full Rect rather than Tight. The transparent padding around each sprite is what holds
        // every frame of one character to the same size, and a Tight mesh crops it straight off.
        settings.spriteMeshType = SpriteMeshType.FullRect;

        // Centred across, so mirroring a sprite to face left turns it about its own middle
        // instead of moving it a whole box width sideways.
        settings.spriteAlignment = (int)SpriteAlignment.Custom;
        settings.spritePivot = new Vector2(0.5f, PivotY(height));

        importer.SetTextureSettings(settings);
    }

    // Anchors a sprite at the middle of its bottom cell, so one integer coordinate means the same
    // for a 1x1 tile as for a 3x4 animal and feet land on a cell boundary whatever the height.
    // Center was the alternative and it leaves every even-height sprite half a cell into the floor.
    static float PivotY(int textureHeight)
    {
        // A sprite shorter than a cell has no bottom cell to sit in, so it stays centred rather
        // than anchored above its own top edge. The two HUD pieces are the only ones.
        if (textureHeight <= PixelsPerUnit)
            return 0.5f;

        return PixelsPerUnit * 0.5f / textureHeight;
    }
}
