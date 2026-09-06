using UnityEngine;

// Picks one of several interchangeable sprites, so a row of one prefab does not read as a row of
// one object. Chosen when the tile is placed rather than at Play, so the Scene view shows the game.
public class SpriteVariant : MonoBehaviour
{
    // Sprites that all mean the same thing in the game. Empty leaves the authored sprite alone.
    [SerializeField] private Sprite[] variants;

    // Called by the level tools when a tile is created, and available from the component's context
    // menu for one already in the scene.
    [ContextMenu("Pick One")]
    public void PickOne()
    {
        if (variants == null || variants.Length == 0)
            return;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer == null)
        {
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer found on " + name + ", the sprite variants are ignored");
            return;
        }

        renderer.sprite = variants[Random.Range(0, variants.Length)];

#if UNITY_EDITOR
        // Recorded as a prefab override, or the choice is dropped the next time the scene loads.
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
#endif
    }
}
