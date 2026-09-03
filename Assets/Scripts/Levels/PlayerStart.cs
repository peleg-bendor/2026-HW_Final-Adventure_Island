using UnityEngine;

// The cell a level starts the player in. Its sprite is drawn at 40% alpha for authoring only, so it
// hides itself rather than being switched off by whatever starts the level.
public class PlayerStart : MonoBehaviour
{
    private void Awake()
    {
        SpriteRenderer marker = GetComponent<SpriteRenderer>();

        if (marker == null)
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer found, the start marker will stay visible");
        else
            marker.enabled = false;
    }
}
