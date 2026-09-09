using UnityEngine;

// A frog that watches the player and leaps at him on a timer he cannot read. It jumps where it
// likes and is stopped by whatever it runs into; it never works out a way around the terrain.
public class Frog : Enemy
{
    // How far inside its own edges the sweep sits, so it never starts overlapping the floor it is
    // standing on and never comes to rest inside what it hit.
    private const float SkinWidth = 0.02f;

    // A flight longer than this has nothing under it to land on, which means a broken level.
    private const float MaxFlightSeconds = 5f;

    // How far from level a surface has to be before it counts as a floor or a ceiling rather than
    // a wall.
    private const float FloorNormal = 0.5f;

    // How high the arc rises above the take-off, in units. Never zero: it also sets the weight.
    [SerializeField, Min(0.1f)] private float jumpHeight = 3f;

    // The shortest and furthest it will leap, in units. It aims at the player between the two, so
    // standing next to one makes it jump clean past rather than creep up in halves.
    [SerializeField, Min(0f)] private float minJumpDistance = 3f;
    [SerializeField, Min(0f)] private float maxJumpDistance = 6f;

    // How long a clear leap takes, in seconds. One that hits something takes as long as it takes.
    [SerializeField, Min(0.05f)] private float jumpSeconds = 0.9f;

    // The shortest and longest wait between jumps, in seconds. Rolled fresh every time, which is the
    // whole of what makes it unreadable.
    [SerializeField, Min(0f)] private float minWaitSeconds = 1f;
    [SerializeField, Min(0f)] private float maxWaitSeconds = 2.5f;

    // How long it crouches before launching, in seconds. Long enough to see, too short to react to.
    [SerializeField, Min(0f)] private float crouchSeconds = 0.12f;

    // Watching, about to jump, and in the air. Three states rather than three frames of one.
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite crouchSprite;
    [SerializeField] private Sprite jumpingSprite;

    private SpriteRenderer art;
    private bool crouching;
    private bool jumping;
    private float waitUntil;
    private float crouchUntil;
    private float jumpStartedAt;
    private Vector2 jumpFrom;
    private float landX;
    private Vector2 velocity;
    private bool warnedLost;

    // An axe, a boomerang, a mount or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.MountAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // A crouch and a jump both finish even if he has run out of range, or the frog would stop in
    // mid-air.
    protected override bool IsMidAction
    {
        get { return crouching || jumping; }
    }

    // Its own body, held a hair inside its edges so a sweep does not catch on what it rests against.
    private Vector2 SweptSize
    {
        get { return new Vector2((HalfWidth - SkinWidth) * 2f, (HalfHeight - SkinWidth) * 2f); }
    }

    // Taken from the arc, so an undisturbed leap traces the same parabola the old one did.
    private float Gravity
    {
        get { return 8f * jumpHeight / (jumpSeconds * jumpSeconds); }
    }

    protected override void OnAwake()
    {
        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Enemy, "No SpriteRenderer found on " + name + ", it will not change pose");
    }

    protected override void OnSpawned()
    {
        crouching = false;
        jumping = false;
        velocity = Vector2.zero;
        Show(idleSprite);
        WaitAgain();
    }

    protected override void Behave()
    {
        if (jumping)
        {
            if (Time.time - jumpStartedAt > MaxFlightSeconds)
            {
                WarnLost();
                Land(transform.position);
                return;
            }

            Move();
            return;
        }

        if (crouching)
        {
            if (Time.time >= crouchUntil)
                Launch();

            return;
        }

        Face(PlayerPosition.x > transform.position.x);

        if (Time.time >= waitUntil)
            Crouch();
    }

    // A fresh roll every time rather than a fixed interval, so no two waits are the same and the
    // jump cannot be timed.
    private void WaitAgain()
    {
        waitUntil = Time.time + Random.Range(minWaitSeconds, maxWaitSeconds);
    }

    private void Crouch()
    {
        crouching = true;
        crouchUntil = Time.time + crouchSeconds;
        Show(crouchSprite);
    }

    // Aimed his way but never for less than a full leap, or the distance left would halve with every
    // jump and the frog would end up hopping on the spot. Nothing is checked about what lies between.
    private void Launch()
    {
        crouching = false;
        jumping = true;
        jumpStartedAt = Time.time;
        jumpFrom = transform.position;

        float direction = PlayerPosition.x > jumpFrom.x ? 1f : -1f;
        float distance = Mathf.Clamp(Mathf.Abs(PlayerPosition.x - jumpFrom.x), minJumpDistance, maxJumpDistance);
        landX = jumpFrom.x + direction * distance;
        velocity = new Vector2((landX - jumpFrom.x) / jumpSeconds, 4f * jumpHeight / jumpSeconds);

        Show(jumpingSprite);
    }

    // Its whole flight, swept a frame at a time. Twice per frame, so what is left of the frame after
    // a bump is spent going the new way rather than being lost against the surface.
    private void Move()
    {
        float remaining = Time.deltaTime;
        velocity.y -= Gravity * remaining;

        for (int pass = 0; pass < 2 && remaining > 0f; pass++)
        {
            Vector2 from = transform.position;
            Vector2 step = velocity * remaining;

            if (step.sqrMagnitude <= 0f)
                return;

            RaycastHit2D hit;

            if (SweepToTerrain(from, SweptSize, step.normalized, step.magnitude, out hit) == false)
            {
                transform.position = new Vector3(from.x + step.x, from.y + step.y, transform.position.z);
                return;
            }

            float travelled = Mathf.Max(0f, hit.distance - SkinWidth);
            Vector2 contact = from + step.normalized * travelled;
            transform.position = new Vector3(contact.x, contact.y, transform.position.z);

            if (hit.normal.y > FloorNormal)
            {
                Land(contact);
                return;
            }

            Deflect(hit);
            remaining *= 1f - travelled / step.magnitude;
        }
    }

    // A ceiling takes its climb and leaves its travel; a wall takes its travel and leaves its fall.
    private void Deflect(RaycastHit2D hit)
    {
        if (hit.normal.y < -FloorNormal)
        {
            velocity.y = Mathf.Min(velocity.y, 0f);
            return;
        }

        velocity.x = 0f;
    }

    // One line a jump, carrying both what it wanted and what it got, so a leap cut short by a wall
    // or a ceiling still shows without the contacts themselves being logged.
    private void Land(Vector2 at)
    {
        transform.position = new Vector3(at.x, at.y, transform.position.z);
        jumping = false;
        velocity = Vector2.zero;
        Show(idleSprite);
        WaitAgain();

        GameLog.Verbose(LogCategory.Enemy, name + " landed at x " + at.x.ToString("0.0") +
            " - leapt from " + jumpFrom.x.ToString("0.0") + ", aimed at " + landX.ToString("0.0"));
    }

    private void WarnLost()
    {
        if (warnedLost)
            return;

        warnedLost = true;
        GameLog.Warning(LogCategory.Enemy, "No ground under " + name + " at x " +
            transform.position.x.ToString("0.0") + ", its jump had nowhere to land");
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
