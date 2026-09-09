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

        if (art == null || movement == null || ground == null)
            GameLog.Warning(LogCategory.Mount, "No SpriteRenderer, PlayerMovement or PlayerGround found, the mount will not animate");

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

    // Airborne beats walking, and one frame covers the whole jump where he has a rise and a fall on
    // foot.
    private Sprite Frame(MountDefinition mount)
    {
        if (ground != null && ground.IsGrounded() == false)
            return mount.Jumping;

        if (movement != null && movement.IsWalking && mount.Walking != null && mount.Walking.Length > 0)
            return mount.Walking[(int)(Time.time / secondsPerWalkFrame) % mount.Walking.Length];

        return mount.Idle;
    }
}
