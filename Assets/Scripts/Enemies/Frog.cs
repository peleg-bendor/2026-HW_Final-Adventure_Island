using UnityEngine;

// A frog that watches the player and leaps at him on a timer he cannot read. It aims at where he
// was when it took off, so it lands on him when he holds still and near him when he does not.
public class Frog : Enemy
{
    // How high the arc rises above the take-off, in units. Never zero: the arc is also the fall.
    [SerializeField, Min(0.1f)] private float jumpHeight = 3f;

    // The furthest it will leap, in units. It aims at the player and stops at this.
    [SerializeField, Min(0f)] private float maxJumpDistance = 6f;

    // How long the level part of a jump takes, in seconds. A drop past it takes as long as it takes.
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
    private float landY;
    private bool warnedNoGround;

    // An axe, a boomerang, an animal or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // A crouch and a jump both finish even if he has run out of range, or the frog would stop in
    // mid-air.
    protected override bool IsMidAction
    {
        get { return crouching || jumping; }
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
        Show(idleSprite);
        WaitAgain();
    }

    protected override void Behave()
    {
        if (jumping)
        {
            Jump();
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

    // Aimed at where he is now, not where he will be. He moves during the flight, which is what
    // makes it land on him sometimes and beside him the rest of the time.
    private void Launch()
    {
        crouching = false;
        jumping = true;
        jumpStartedAt = Time.time;
        jumpFrom = transform.position;

        float toPlayer = PlayerPosition.x - jumpFrom.x;
        landX = jumpFrom.x + Mathf.Clamp(toPlayer, -maxJumpDistance, maxJumpDistance);
        landY = GroundUnderTarget();

        Show(jumpingSprite);
    }

    // Wherever it comes down, however far below. A level with nothing under the target is broken,
    // so it says so once and lands in the air rather than falling out of the world.
    private float GroundUnderTarget()
    {
        float apex = jumpFrom.y + jumpHeight;
        float drop = DistanceToTerrain(new Vector2(landX, apex), Vector2.down, Mathf.Infinity);

        if (float.IsInfinity(drop) == false)
            return apex - drop + Feet;

        if (warnedNoGround == false)
        {
            warnedNoGround = true;
            GameLog.Warning(LogCategory.Enemy, "No ground under " + name + "'s target at x " + landX.ToString("0.0") + ", it lands in the air");
        }

        return jumpFrom.y;
    }

    // One arc all the way down. Past the end of the jump the same parabola turns downward and
    // accelerates, which is what carries it into a pit rather than stopping at the edge.
    private void Jump()
    {
        float t = (Time.time - jumpStartedAt) / jumpSeconds;
        float y = jumpFrom.y + jumpHeight * 4f * t * (1f - t);

        if (t >= 1f && y <= landY)
        {
            transform.position = new Vector3(landX, landY, transform.position.z);
            jumping = false;
            Show(idleSprite);
            WaitAgain();
            return;
        }

        float x = Mathf.Lerp(jumpFrom.x, landX, Mathf.Min(t, 1f));
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
