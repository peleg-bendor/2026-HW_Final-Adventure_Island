using UnityEngine;
using UnityEngine.InputSystem;

// The player's horizontal movement and which way he faces. Jump and attack are separate
// components, because they answer to different keys and own state that outlives a frame.
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    // How hard he brakes with no key held, in units per second squared.
    [SerializeField] private float deceleration = 40f;

    // Intent rather than motion, so pressing into a wall still animates as walking.
    public bool IsWalking { get; private set; }

    private Rigidbody2D rigid;

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

            // Scale rather than the renderer's flipX, so a child spawn point mirrors with him.
            transform.localScale = new Vector3(direction > 0f ? 1f : -1f, 1f, 1f);
        }
        else
        {
            // MoveTowards rather than linear damping, which decays towards zero without arriving
            // and leaves him drifting.
            float braked = Mathf.MoveTowards(rigid.linearVelocity.x, 0f, deceleration * Time.fixedDeltaTime);
            rigid.linearVelocity = new Vector2(braked, rigid.linearVelocity.y);
        }
    }
}
