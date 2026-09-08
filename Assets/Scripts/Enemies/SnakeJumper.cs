using UnityEngine;

// A snake that stands, hops a short way in the direction it faces, and stands again. The hop is
// always level and any drop is a separate fall, so it cannot pass through what it took off from.
public class SnakeJumper : Enemy
{
    // A hop shorter than this fraction of a full one is a twitch rather than a bump, so it turns
    // instead of taking it.
    private const float LeastWorthwhileHop = 0.25f;

    // How far each hop carries it, in units.
    [SerializeField, Min(0f)] private float hopDistance = 2f;

    // How high the arc rises above the take-off, in units.
    [SerializeField, Min(0f)] private float hopHeight = 1f;

    // How long the jump takes, in seconds. A fall past the end of it takes as long as it takes.
    [SerializeField, Min(0.05f)] private float hopSeconds = 0.6f;

    // How long it stands still between hops, in seconds.
    [SerializeField, Min(0f)] private float standSeconds = 1f;

    // Shown while it stands, and while it is in the air. Two states rather than two frames of one.
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private Sprite jumpingSprite;

    private SpriteRenderer art;
    private bool authoredRight;

    private bool hopping;
    private bool falling;
    private bool turnOnLanding;
    private float hopStartedAt;
    private float fallStartedAt;
    private Vector2 hopFrom;
    private float landX;
    private float landY;
    private float standingSince;

    // An axe, a boomerang, an animal or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // A hop and the fall after it both finish even if he has run out of range, or the snake would
    // stop in mid-air.
    protected override bool IsMidAction
    {
        get { return hopping || falling; }
    }

    protected override void OnAwake()
    {
        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Enemy, "No SpriteRenderer found on " + name + ", it will not change pose");

        // Read before anything flips it, so a snake that turned at a wall still comes back facing
        // the way it was placed.
        authoredRight = FacesRight;
    }

    protected override void OnSpawned()
    {
        Face(authoredRight);
        hopping = false;
        falling = false;
        turnOnLanding = false;
        standingSince = Time.time;
        Show(standingSprite);
    }

    protected override void Behave()
    {
        if (falling)
        {
            Fall();
            return;
        }

        if (hopping)
        {
            Hop();
            return;
        }

        if (Time.time - standingSince >= standSeconds)
            StartHop();
    }

    private void StartHop()
    {
        Vector2 from = transform.position;
        float direction = FacesRight ? 1f : -1f;

        // Cut short by a wall rather than refused by one, so it bumps into it instead of stopping
        // short of something it never touched.
        float reach = ReachBefore(from, direction);
        float x = from.x + direction * reach;
        float surface;

        // Already against the wall, so there is nothing left to bump into.
        if (reach < hopDistance * LeastWorthwhileHop)
        {
            Face(FacesRight == false);
            standingSince = Time.time;
            return;
        }

        if (FindGround(new Vector2(x, from.y + hopHeight), out surface) == false)
        {
            // Nothing to come down on is the one thing that refuses a hop, since there would be no
            // end to the fall.
            Face(FacesRight == false);
            standingSince = Time.time;
            return;
        }

        hopFrom = from;
        landX = x;
        landY = surface + Feet;
        turnOnLanding = reach < hopDistance;
        hopping = true;
        hopStartedAt = Time.time;
        Show(jumpingSprite);
    }

    // Level from end to end whatever is below, so the arc cannot cut down through the ledge it left.
    private void Hop()
    {
        float t = (Time.time - hopStartedAt) / hopSeconds;

        if (t < 1f)
        {
            float x = Mathf.Lerp(hopFrom.x, landX, t);
            float y = hopFrom.y + hopHeight * 4f * t * (1f - t);
            transform.position = new Vector3(x, y, transform.position.z);
            return;
        }

        hopping = false;
        transform.position = new Vector3(landX, hopFrom.y, transform.position.z);

        if (landY < hopFrom.y)
        {
            falling = true;
            fallStartedAt = Time.time;
            return;
        }

        Land();
    }

    // Straight down the one column the ground ray already proved is clear, carrying on from the
    // speed and acceleration the arc ended with.
    private void Fall()
    {
        float speed = 4f * hopHeight / hopSeconds;

        if (speed <= 0f)
        {
            Land();
            return;
        }

        float elapsed = Time.time - fallStartedAt;
        float gravity = 8f * hopHeight / (hopSeconds * hopSeconds);
        float y = hopFrom.y - (speed * elapsed + 0.5f * gravity * elapsed * elapsed);

        if (y <= landY)
        {
            falling = false;
            Land();
            return;
        }

        transform.position = new Vector3(landX, y, transform.position.z);
    }

    private void Land()
    {
        transform.position = new Vector3(landX, landY, transform.position.z);
        standingSince = Time.time;
        Show(standingSprite);

        if (turnOnLanding)
        {
            turnOnLanding = false;
            Face(FacesRight == false);
        }
    }

    // How far it can travel before it meets something solid, never more than a whole hop.
    private float ReachBefore(Vector2 from, float direction)
    {
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(from, Vector2.right * direction, hopDistance + HalfWidth))
        {
            if (IsTerrain(hit.collider))
                nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
            return hopDistance;

        // Measured from the middle, so its front stops at the wall face rather than inside it.
        return Mathf.Clamp(nearest - HalfWidth, 0f, hopDistance);
    }

    // The nearest surface below a point, and whether there is one at all.
    private bool FindGround(Vector2 above, out float surfaceY)
    {
        surfaceY = above.y;
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(above, Vector2.down))
        {
            if (IsTerrain(hit.collider))
                nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
            return false;

        surfaceY = above.y - nearest;
        return true;
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
