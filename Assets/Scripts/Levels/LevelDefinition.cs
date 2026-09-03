using UnityEngine;

// A level's authored size, on its root object. Authored rather than measured: the tiles describe the
// art rather than the level, and the level file's own size only ever grows.
public class LevelDefinition : MonoBehaviour
{
    [SerializeField] private int widthInCells = 32;
    [SerializeField] private int heightInCells = 15;

    private PlayerStart playerStart;

    // Half a cell out from the outermost cell centres, since every sprite is anchored at the middle
    // of its bottom cell.
    public Rect PlayableArea
    {
        get
        {
            Vector3 origin = transform.position;
            return new Rect(origin.x - 0.5f, origin.y - 0.5f, widthInCells, heightInCells);
        }
    }

    // Found among the children rather than serialized, since a rebuild replaces the marker. Looked
    // up again when the cached one has been destroyed, which is what makes that safe.
    public PlayerStart PlayerStart
    {
        get
        {
            if (playerStart == null)
            {
                playerStart = GetComponentInChildren<PlayerStart>(true);

                if (playerStart == null)
                    GameLog.Warning(LogCategory.Game, "No PlayerStart found under " + name + ", nothing knows where the player begins");
            }

            return playerStart;
        }
    }

    // Drawn so the authored size can be checked against the tiles without pressing Play.
    private void OnDrawGizmosSelected()
    {
        Rect area = PlayableArea;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(area.center, new Vector3(area.width, area.height, 0f));
    }
}
