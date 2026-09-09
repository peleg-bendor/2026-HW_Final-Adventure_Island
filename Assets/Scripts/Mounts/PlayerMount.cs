using UnityEngine;
using Zenject;

// The player's body while riding: how wide his capsule is, and which animator is drawing him. What
// he is riding is the slot's answer, and what that looks like is PlayerMountAnimator's.
public class PlayerMount : MonoBehaviour
{
    private IMountSlot slot;

    private CapsuleCollider2D body;
    private Animator onFoot;
    private float onFootWidth;

    [Inject]
    public void Construct(IMountSlot slot)
    {
        this.slot = slot;
    }

    private void Awake()
    {
        body = GetComponent<CapsuleCollider2D>();
        onFoot = GetComponent<Animator>();

        // Read off the collider rather than repeated as a number here, so the scene stays the one
        // place his size is authored.
        if (body != null)
            onFootWidth = body.size.x;
        else
            GameLog.Warning(LogCategory.Mount, "No CapsuleCollider2D found, the player will not change size when he mounts");

        if (onFoot == null)
            GameLog.Warning(LogCategory.Mount, "No Animator found, his on-foot frames will not come back when he dismounts");

        if (slot == null)
        {
            GameLog.Warning(LogCategory.Mount, "No IMountSlot injected, the player will never mount");
            return;
        }

        slot.Changed += Apply;
    }

    private void OnDestroy()
    {
        if (slot != null)
            slot.Changed -= Apply;
    }

    // The Animator is switched off rather than given three controllers of its own, which would be a
    // controller and four clips per mount.
    private void Apply()
    {
        MountDefinition mount = slot.Current;

        if (body != null)
            body.size = new Vector2(mount != null ? mount.BodyWidth : onFootWidth, body.size.y);

        if (onFoot != null)
            onFoot.enabled = mount == null;
    }
}
