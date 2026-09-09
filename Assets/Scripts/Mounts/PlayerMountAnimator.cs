using UnityEngine;
using Zenject;

// The player's picture while riding, from the frames on the mount's own asset. Like PlayerAnimator
// it only reads, so deleting it would leave the game playable and silent.
public class PlayerMountAnimator : MonoBehaviour
{
    // How long each walking frame is held, in seconds. A rule of the game rather than a property of
    // any one mount, so it is not on the asset.
    [SerializeField, Min(0.01f)] private float secondsPerWalkFrame = 0.15f;

    private IMountSlot slot;
    private SpriteRenderer art;
    private PlayerMovement movement;
    private PlayerGround ground;
    private PlayerMountAttack attack;
    private Sprite shown;

    [Inject]
    public void Construct(IMountSlot slot)
    {
        this.slot = slot;
    }

    private void Awake()
    {
        art = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();
        ground = GetComponent<PlayerGround>();
        attack = GetComponent<PlayerMountAttack>();

        if (art == null || movement == null || ground == null)
            GameLog.Warning(LogCategory.Mount, "No SpriteRenderer, PlayerMovement or PlayerGround found, the mount will not animate");

        if (attack == null)
            GameLog.Warning(LogCategory.Mount, "No PlayerMountAttack found, the mount will not show its attack frames");

        if (slot == null)
            GameLog.Warning(LogCategory.Mount, "No IMountSlot injected, the mount will not animate");
    }

    private void Update()
    {
        MountDefinition mount = slot != null ? slot.Current : null;

        // Forgotten on foot, or remounting the same one would find its frame already cached and
        // leave the Animator's sprite showing.
        if (mount == null || art == null)
        {
            shown = null;
            return;
        }

        Sprite frame = Frame(mount);

        if (frame == null || frame == shown)
            return;

        shown = frame;
        art.sprite = frame;
    }

    // Attacking beats airborne beats walking, and one frame covers the whole jump where he has a
    // rise and a fall on foot.
    private Sprite Frame(MountDefinition mount)
    {
        if (attack != null && attack.IsAttacking)
            return AttackFrame(mount);

        if (ground != null && ground.IsGrounded() == false)
            return mount.Jumping;

        if (movement != null && movement.IsWalking && mount.Walking != null && mount.Walking.Length > 0)
            return mount.Walking[(int)(Time.time / secondsPerWalkFrame) % mount.Walking.Length];

        return mount.Idle;
    }

    // Spread across the attack's own length rather than a fixed interval, so a four-frame spin and
    // a one-frame swipe both finish exactly when the hit does.
    private Sprite AttackFrame(MountDefinition mount)
    {
        Sprite[] frames = mount.Attacking;

        if (frames == null || frames.Length == 0)
            return mount.Idle;

        return frames[Mathf.Clamp((int)(attack.AttackProgress * frames.Length), 0, frames.Length - 1)];
    }
}
