using UnityEngine;

// Pushes the player's state into the Animator and does nothing else. It only reads, so deleting
// it would leave the game playable and silent.
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int GroundedParameter = Animator.StringToHash("Grounded");
    private static readonly int WalkingParameter = Animator.StringToHash("Walking");
    private static readonly int RisingParameter = Animator.StringToHash("Rising");

    private Animator animator;
    private Rigidbody2D rigid;
    private PlayerMovement movement;
    private PlayerGround ground;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        ground = GetComponent<PlayerGround>();

        if (animator == null)
            GameLog.Warning(LogCategory.Player, "No Animator found, the player will not animate");

        if (rigid == null || movement == null || ground == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D, PlayerMovement or PlayerGround found, the player will not animate");
    }

    private void Update()
    {
        if (animator == null || rigid == null || movement == null || ground == null)
            return;

        animator.SetBool(GroundedParameter, ground.IsGrounded());
        animator.SetBool(WalkingParameter, movement.IsWalking);

        // Vertical velocity rather than asking PlayerJump, so walking off a ledge picks the
        // falling frame too.
        animator.SetBool(RisingParameter, rigid.linearVelocity.y > 0f);
    }
}
