using UnityEngine;

// Identifies the player to anything that has to ask what touched it, and answers where the middle
// of his body is. A marker rather than a tag, since most callers want the object and not the answer.
public class Player : MonoBehaviour
{
    private Collider2D body;

    private void Awake()
    {
        body = GetComponent<Collider2D>();

        if (body == null)
            GameLog.Warning(LogCategory.Player, "No Collider2D found, throws will leave from his feet");
    }

    // Read off the collider rather than held as an offset, so a throw and a boomerang's return
    // cannot drift apart or from wherever his body actually is.
    public Vector2 Middle
    {
        get { return body != null ? (Vector2)body.bounds.center : (Vector2)transform.position; }
    }
}
