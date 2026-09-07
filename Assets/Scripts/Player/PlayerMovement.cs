using UnityEngine;
using UnityEngine.InputSystem;

// The player's horizontal movement and which way he faces, whether the keys asked for it or a
// hazard did. Jump and attack are separate components, because they own state that outlives a frame.
public class PlayerMovement : MonoBehaviour, IPlayerShove
{
    [SerializeField] private float speed = 6f;

    // How hard he brakes with no key held, in units per second squared.
    [SerializeField] private float deceleration = 40f;

    // Intent rather than motion, so pressing into a wall still animates as walking.
    public bool IsWalking { get; private set; }

    // True while a shove is still running, which is what stops a run of rocks charging once each.
    public bool IsShoving { get { return Time.time < shoveUntil; } }


    private Rigidbody2D rigid;
    private float shoveUntil = float.NegativeInfinity;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will not move");
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

    // Scale rather than the renderer's flipX, so a child spawn point mirrors with him. Public so a
    // reset can restore his facing without knowing how facing is represented.
    public void Face(bool right)
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

    // Cleared on a reset, or he arrives at the start with the tail of a shove still running and no
    // control until it expires.
    public void ClearShove()
    {
        shoveUntil = float.NegativeInfinity;
    }
}
