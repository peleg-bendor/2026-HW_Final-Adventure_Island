using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// The player's jump, with height set by how long Space is held. Whether he is standing on
// something is PlayerGround's answer, since the animators ask the same question.
public class PlayerJump : MonoBehaviour, IResettable
{
    // Upward speed at the moment of the jump, in units per second.
    [SerializeField] private float jumpSpeed = 14f;

    // What is left of the rise when Space is released early.
    [SerializeField] private float riseCutFactor = 0.5f;

    // Has to outlast PlayerGround's grace window, or the one press that starts a jump would still
    // find the ground underneath it a frame later and start a second.
    [SerializeField] private float jumpCooldown = 0.15f;

    private IResetRegistry registry;
    private IPlayerGround ground;
    private Rigidbody2D rigid;
    private float lastJumpTime = float.NegativeInfinity;
    private bool jumpRequested;
    private bool cutRequested;

    [Inject]
    public void Construct(IResetRegistry registry, IPlayerGround ground)
    {
        this.registry = registry;
        this.ground = ground;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will not jump");

        if (ground == null)
            GameLog.Warning(LogCategory.Player, "No IPlayerGround injected, the player will not jump");
    }

    private void OnEnable()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Player, "No IResetRegistry injected, a jump pressed under a popup will fire after the restart");
            return;
        }

        registry.Register(this);
    }

    private void OnDisable()
    {
        if (registry != null)
            registry.Unregister(this);
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

    // Both scopes, because Update keeps reading the keyboard while the game is frozen and a press
    // made under a popup would be spent on the first physics step after the restart.
    public void ResetTo(ResetScope scope)
    {
        jumpRequested = false;
        cutRequested = false;
    }
}
