using UnityEngine;
using Zenject;

// Pushes the player's state into the Animator and does nothing else. It only reads, so deleting
// it would leave the game playable and the player unanimated.
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int GroundedParameter = Animator.StringToHash("Grounded");
    private static readonly int WalkingParameter = Animator.StringToHash("Walking");
    private static readonly int RisingParameter = Animator.StringToHash("Rising");

    private IPlayerGround ground;
    private IPlayerMotion motion;
    private Animator animator;
    private Rigidbody2D rigid;

    [Inject]
    public void Construct(IPlayerGround ground, IPlayerMotion motion)
    {
        this.ground = ground;
        this.motion = motion;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();

        if (animator == null || rigid == null)
            GameLog.Warning(LogCategory.Player, "No Animator or Rigidbody2D found, the player will not animate");

        if (ground == null || motion == null)
            GameLog.Warning(LogCategory.Player, "No IPlayerGround or IPlayerMotion injected, the player will not animate");
    }

    private void Update()
    {
        if (animator == null || rigid == null || ground == null || motion == null)
            return;

        animator.SetBool(GroundedParameter, ground.IsGrounded());
        animator.SetBool(WalkingParameter, motion.IsWalking);

        // Vertical velocity rather than asking PlayerJump, so walking off a ledge picks the
        // falling frame too.
        animator.SetBool(RisingParameter, rigid.linearVelocity.y > 0f);
    }
}
