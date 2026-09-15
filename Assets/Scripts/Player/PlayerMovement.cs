using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// The player's horizontal movement and which way he faces, whether the keys asked for it or a
// hazard did. Jumping and attacking are components of their own.
public class PlayerMovement : MonoBehaviour, IPlayerShove, IPlayerMotion, IResettable
{
    [SerializeField] private float speed = 6f;

    // How hard he brakes with no key held, in units per second squared.
    [SerializeField] private float deceleration = 40f;

    // Intent rather than motion, so pressing into a wall still animates as walking.
    public bool IsWalking { get; private set; }

    // True while a shove is still running, which is what stops a run of rocks charging once each.
    public bool IsShoving { get { return Time.time < shoveUntil; } }

    private IResetRegistry registry;
    private ILevels levels;
    private Rigidbody2D rigid;
    private float shoveUntil = float.NegativeInfinity;

    [Inject]
    public void Construct(IResetRegistry registry, ILevels levels)
    {
        this.registry = registry;
        this.levels = levels;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will not move");
    }

    private void OnEnable()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Player, "No IResetRegistry injected, the player will keep his facing and a running shove across a reset");
            return;
        }

        registry.Register(this);
    }

    private void OnDisable()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    private void FixedUpdate()
    {
        if (rigid == null)
            return;

        // Neither the keys nor the brake run while a shove is in progress, or the walk speed would
        // erase it inside a step.
        if (IsShoving)
        {
            IsWalking = false;
            return;
        }

        float direction = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed)
                direction = -1f;
            if (Keyboard.current.rightArrowKey.isPressed)
                direction = 1f;
        }

        IsWalking = direction != 0f;

        if (IsWalking)
        {
            rigid.linearVelocity = new Vector2(direction * speed, rigid.linearVelocity.y);
            Face(direction > 0f);
        }
        else
        {
            // MoveTowards rather than linear damping, which decays towards zero without arriving
            // and leaves him drifting.
            float braked = Mathf.MoveTowards(rigid.linearVelocity.x, 0f, deceleration * Time.fixedDeltaTime);
            rigid.linearVelocity = new Vector2(braked, rigid.linearVelocity.y);
        }
    }

    // Scale rather than the renderer's flipX, so the mount's hit, a child of his, mirrors with him.
    private void Face(bool right)
    {
        transform.localScale = new Vector3(right ? 1f : -1f, 1f, 1f);
    }

    // Horizontal only, so a shove taken mid-jump does not cancel a rise he has already paid for.
    public void Shove(float shoveSpeed, float seconds)
    {
        if (rigid == null)
        {
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player cannot be shoved");
            return;
        }

        shoveUntil = Time.time + seconds;
        rigid.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * shoveSpeed, rigid.linearVelocity.y);
    }

    // The same for both scopes, since both put him at the level start.
    public void ResetTo(ResetScope scope)
    {
        // Cleared, or he arrives with the tail of a shove still running and no control until it expires.
        shoveUntil = float.NegativeInfinity;

        PlayerStart start = levels != null && levels.Current != null ? levels.Current.PlayerStart : null;

        // No warning of its own without a start, since LevelDefinition reports a missing marker.
        if (start != null)
            Face(start.FacesRight);
    }
}
