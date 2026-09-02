using UnityEngine;
using UnityEngine.InputSystem;

// The player's jump, with height set by how long Space is held. Grounded is answered from the
// body's own contacts rather than a probe, so no tile needs a marker component.
public class PlayerJump : MonoBehaviour
{
    // Upward speed at the moment of the jump, in units per second.
    [SerializeField] private float jumpSpeed = 14f;

    // What is left of the rise when Space is released early.
    [SerializeField] private float riseCutFactor = 0.5f;

    // A rounded bottom perched on a tile corner reports a tilted normal, so ground is anything
    // within about 45 degrees of vertical.
    private const float GroundNormalMinimum = 0.7f;

    // Reused rather than allocated, since the contacts are read every physics step.
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[8];

    private Rigidbody2D rigid;
    private bool jumpRequested;
    private bool cutRequested;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will not jump");
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
        if (rigid == null)
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
        if (IsGrounded() == false)
        {
            GameLog.Verbose(LogCategory.Player, "Jump ignored - not on the ground");
            return;
        }

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

    // Ground is a contact whose normal points up. A wall's points sideways and a trigger has no
    // contact at all, so the hazards and pickups are excluded without naming any of them.
    private bool IsGrounded()
    {
        int count = rigid.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            if (contacts[i].normal.y > GroundNormalMinimum)
                return true;
        }

        return false;
    }
}
