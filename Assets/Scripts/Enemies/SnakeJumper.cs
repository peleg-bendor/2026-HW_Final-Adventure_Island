using UnityEngine;

// A snake that stands, hops a short way in the direction it faces, and stands again. A wall cuts a
// hop short and turns it on landing; a lower floor is only a longer arc.
public class SnakeJumper : Enemy
{
    // How far each hop carries it, in units.
    [SerializeField, Min(0f)] private float hopDistance = 2f;

    // How high the arc rises above a level hop, in units.
    [SerializeField, Min(0f)] private float hopHeight = 1f;

    // How long one hop takes, in seconds.
    [SerializeField, Min(0.05f)] private float hopSeconds = 0.6f;

    // How long it stands still between hops, in seconds.
    [SerializeField, Min(0f)] private float standSeconds = 1f;

    // Shown while it stands, and while it is in the air. Two states rather than two frames of one.
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private Sprite jumpingSprite;

    private SpriteRenderer art;
    private float halfWidth;
    private bool authoredRight;

    private bool hopping;
    private bool turnOnLanding;
    private float hopStartedAt;
    private Vector2 hopFrom;
    private Vector2 hopTo;
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

    // A hop finishes even if he has run out of range, or the snake would stop in mid-air.
    protected override bool IsMidAction
    {
        get { return hopping; }
    }

    protected override void OnAwake()
    {
        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Enemy, "No SpriteRenderer found on " + name + ", it will not change pose");

        Collider2D body = GetComponent<Collider2D>();
        halfWidth = body != null ? body.bounds.extents.x : 0f;

        // Read before anything flips it, so a snake that turned at a wall still comes back facing
        // the way it was placed.
        authoredRight = FacesRight;
    }

    protected override void OnSpawned()
    {
        Face(authoredRight);
        hopping = false;
        turnOnLanding = false;
        standingSince = Time.time;
        Show(standingSprite);
    }

    protected override void Behave()
    {
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

        // Cut short by a wall rather than refused by one, so it visibly bumps into it instead of
        // stopping two cells away from something it never touched.
        float reach = ReachBefore(from, direction);
        Vector2 landing;

        if (FindGround(new Vector2(from.x + direction * reach, from.y + hopHeight), out landing) == false)
        {
            // Nothing to come down on is the one thing that refuses a hop, since there would be no
            // end to the arc.
            Face(FacesRight == false);
            standingSince = Time.time;
            return;
        }

        turnOnLanding = reach < hopDistance;
        hopFrom = from;
        hopTo = landing;
        hopping = true;
        hopStartedAt = Time.time;
        Show(jumpingSprite);
    }

    private void Hop()
    {
        float t = (Time.time - hopStartedAt) / hopSeconds;

        if (t >= 1f)
        {
            transform.position = new Vector3(hopTo.x, hopTo.y, transform.position.z);
            hopping = false;
            standingSince = Time.time;
            Show(standingSprite);

            if (turnOnLanding)
            {
                turnOnLanding = false;
                Face(FacesRight == false);
            }

            return;
        }

        // A straight line between the two ends with a parabola added on top, so one expression
        // covers a level hop and a hop down onto something lower.
        float x = Mathf.Lerp(hopFrom.x, hopTo.x, t);
        float y = Mathf.Lerp(hopFrom.y, hopTo.y, t) + hopHeight * 4f * t * (1f - t);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    // How far it can travel before its body meets something solid, never more than a whole hop.
    private float ReachBefore(Vector2 from, float direction)
    {
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(from, Vector2.right * direction, hopDistance + halfWidth))
        {
            if (IsTerrain(hit.collider))
                nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
            return hopDistance;

        // Measured from the middle, so its front stops at the wall face rather than inside it.
        return Mathf.Clamp(nearest - halfWidth, 0f, hopDistance);
    }

    // The nearest thing to come down on below a point, and where the pivot sits once it has landed.
    private bool FindGround(Vector2 above, out Vector2 landing)
    {
        landing = above;
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(above, Vector2.down))
        {
            if (IsTerrain(hit.collider))
                nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
            return false;

        landing = new Vector2(above.x, above.y - nearest + Feet);
        return true;
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
