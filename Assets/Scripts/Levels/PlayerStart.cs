using UnityEngine;

// The cell a level starts the player in, and which way he faces there. Its sprite is drawn at 40%
// alpha for authoring only, so it hides itself rather than being switched off by something else.
public class PlayerStart : MonoBehaviour
{
    // Read from the marker's own flip rather than a field, so the aid in the Scene view always
    // shows the facing it actually produces.
    public bool FacesRight { get { return transform.localScale.x >= 0f; } }

    private void Awake()
    {
        SpriteRenderer marker = GetComponent<SpriteRenderer>();

        if (marker == null)
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer found, the start marker will stay visible");
        else
            marker.enabled = false;
    }
}
