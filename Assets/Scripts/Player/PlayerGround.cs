using UnityEngine;

// Answers whether the player is standing on something. Ground is a contact whose normal points
// up, so a wall reads as a wall and a trigger is not ground at all.
public class PlayerGround : MonoBehaviour
{
    // A rounded bottom perched on a tile corner reports a tilted normal, so ground is anything
    // within about 45 degrees of vertical.
    private const float GroundNormalMinimum = 0.7f;

    // How long after the last contact he still counts as standing. The capsule separates from a
    // flat floor for a frame or two whenever the solver pushes it back out, and both the walk
    // animation and a jump press fall through that gap without this.
    [SerializeField] private float contactGrace = 0.1f;

    // Reused rather than allocated, since the contacts are read every physics step.
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[8];

    private Rigidbody2D rigid;
    private float lastContactTime = float.NegativeInfinity;
    private bool wasGrounded;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will always read as airborne");
    }

    // Sampled on demand rather than cached in FixedUpdate, where execution order across
    // components would leave one caller reading a value a physics step old.
    public bool IsGrounded()
    {
        if (rigid == null)
            return false;

        if (HasGroundContact())
            lastContactTime = Time.time;

        bool grounded = Time.time - lastContactTime <= contactGrace;

        if (grounded != wasGrounded)
        {
            wasGrounded = grounded;
            GameLog.Verbose(LogCategory.Player, grounded ? "Landed" : "Left the ground");
        }

        return grounded;
    }

    private bool HasGroundContact()
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
