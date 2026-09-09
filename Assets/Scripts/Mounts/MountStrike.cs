using UnityEngine;

// The hit an attack puts into the world: a box that appears for as long as the attack lasts and
// destroys what it touches. One object switched on and off, rather than a pool of one.
public class MountStrike : MonoBehaviour
{
    private BoxCollider2D box;
    private SpriteRenderer art;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        art = GetComponent<SpriteRenderer>();

        if (box == null || art == null)
        {
            GameLog.Warning(LogCategory.Mount, "No BoxCollider2D or SpriteRenderer found on " + name + ", a mount attack will hit nothing");
            return;
        }

        // Switched off here as well as in the Inspector, since a box left on would destroy whatever
        // he walks past.
        End();
    }

    // Shaped at the moment it is used rather than when he mounts, so a swap cannot leave the last
    // mount's box behind.
    public void Begin(Sprite sprite, Vector2 offset, Vector2 size)
    {
        if (box == null || art == null)
            return;

        transform.localPosition = offset;

        // A size of zero is a mount whose attack is not a box at all, which is the one that spits
        // fire.
        box.enabled = size.x > 0f && size.y > 0f;

        if (box.enabled)
            box.size = size;

        art.sprite = sprite;
        art.enabled = sprite != null;
    }

    public void End()
    {
        if (box != null)
            box.enabled = false;

        if (art != null)
            art.enabled = false;
    }

    // Whether it was destroyed is never asked: the mount hits everything it may, and what may be
    // hit is each target's own answer.
    private void OnTriggerEnter2D(Collider2D other)
    {
        IDestructible target = other.GetComponent<IDestructible>();

        if (target != null)
            target.TryDestroy(Destroyer.MountAttack);
    }
}
