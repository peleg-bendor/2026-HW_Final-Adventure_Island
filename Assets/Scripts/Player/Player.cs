using UnityEngine;

// Identifies the player to anything that has to ask what touched it, and answers where the middle of
// his body is and which way he is looking. A marker rather than a tag, since most callers want the
// object and not the answer.
public class Player : MonoBehaviour
{
    private float middleOffset;

    private void Awake()
    {
        Collider2D body = GetComponent<Collider2D>();

        if (body == null)
        {
            GameLog.Warning(LogCategory.Player, "No Collider2D found, throws will leave from his feet");
            return;
        }

        middleOffset = body.bounds.center.y - transform.position.y;
    }

    // Measured off the collider once and applied to the live transform, rather than read from bounds
    // every time, which is only as current as the last physics sync and a reset teleports him.
    public Vector2 Middle
    {
        get { return (Vector2)transform.position + Vector2.up * middleOffset; }
    }

    // Read off his scale, which is what PlayerMovement.Face writes, so the two cannot disagree.
    public bool FacesRight
    {
        get { return transform.localScale.x > 0f; }
    }
}
