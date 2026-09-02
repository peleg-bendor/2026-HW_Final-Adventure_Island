using UnityEngine;
using UnityEngine.InputSystem;

// The player's jump, with height set by how long Space is held. Whether he is standing on
// something is PlayerGround's answer, since the animator asks the same question.
public class PlayerJump : MonoBehaviour
{
    // Upward speed at the moment of the jump, in units per second.
    [SerializeField] private float jumpSpeed = 14f;

    // What is left of the rise when Space is released early.
    [SerializeField] private float riseCutFactor = 0.5f;

    // Has to outlast PlayerGround's grace window, or the one press that starts a jump would still
    // find the ground underneath it a frame later and start a second.
    [SerializeField] private float jumpCooldown = 0.15f;

    private Rigidbody2D rigid;
    private PlayerGround ground;
    private float lastJumpTime = float.NegativeInfinity;
    private bool jumpRequested;
    private bool cutRequested;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        ground = GetComponent<PlayerGround>();

        if (rigid == null || ground == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D or PlayerGround found, the player will not jump");
    }

    // Update rather than FixedUpdate, where a press shorter than one physics step is missed.
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            cutRequested = true;
    }

    private void FixedUpdate()
    {
        if (rigid == null || ground == null)
            return;

        // Jump before cut, so a tap inside one physics step gives the shortest jump rather than
        // being swallowed.
        if (jumpRequested)
        {
            jumpRequested = false;
            Jump();
        }

        if (cutRequested)
        {
            cutRequested = false;
            CutRise();
        }
    }

    private void Jump()
    {
        if (Time.time - lastJumpTime < jumpCooldown)
        {
            GameLog.Verbose(LogCategory.Player, "Jump ignored - too soon after the last one");
            return;
        }

        if (ground.IsGrounded() == false)
        {
            GameLog.Verbose(LogCategory.Player, "Jump ignored - not on the ground");
            return;
        }

        lastJumpTime = Time.time;
        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, jumpSpeed);
        GameLog.Verbose(LogCategory.Player, "Jumped");
    }

    private void CutRise()
    {
        // Only while rising, or letting go during the fall would slow it.
        if (rigid.linearVelocity.y <= 0f)
            return;

        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, rigid.linearVelocity.y * riseCutFactor);
    }
}
