using UnityEngine;

// The hit an attack puts into the world: a shape that appears for as long as the attack lasts and
// destroys what it touches. One object switched on and off, rather than a pool of one.
public class MountStrike : MonoBehaviour
{
    private CapsuleCollider2D shape;
    private SpriteRenderer art;

    private void Awake()
    {
        shape = GetComponent<CapsuleCollider2D>();
        art = GetComponent<SpriteRenderer>();

        if (shape == null || art == null)
        {
            GameLog.Warning(LogCategory.Mount, "No CapsuleCollider2D or SpriteRenderer found on " + name + ", a mount attack will hit nothing");
            return;
        }

        // Switched off here as well as in the Inspector, since a hit left on would destroy whatever
        // he walks past.
        End();
    }

    // Shaped at the moment it is used rather than when he mounts, so a swap cannot leave the last
    // mount's hit behind.
    public void Begin(Sprite sprite, Vector2 offset, Vector2 size)
    {
        if (shape == null || art == null)
            return;

        transform.localPosition = offset;

        // A size of zero is a mount whose attack is not a shape at all, which is the one that spits
        // fire.
        shape.enabled = size.x > 0f && size.y > 0f;

        if (shape.enabled)
            shape.size = size;

        art.sprite = sprite;
        art.enabled = sprite != null;
    }

    public void End()
    {
        if (shape != null)
            shape.enabled = false;

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
