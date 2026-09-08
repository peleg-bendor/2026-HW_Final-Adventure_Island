using UnityEngine;

// A snake that stands, hops a short way in the direction it faces, and stands again. The hop is
// level from end to end and carries on falling past it, so it cannot cut through what it left.
public class SnakeJumper : Enemy
{
    // A hop shorter than this fraction of a full one is a twitch rather than a bump, so it turns
    // instead of taking it.
    private const float LeastWorthwhileHop = 0.25f;

    // How far each hop carries it, in units.
    [SerializeField, Min(0f)] private float hopDistance = 2f;

    // How high the arc rises above the take-off, in units. Never zero: the arc is also the fall.
    [SerializeField, Min(0.1f)] private float hopHeight = 1f;

    // How long the level part of a hop takes, in seconds. A drop past it takes as long as it takes.
    [SerializeField, Min(0.05f)] private float hopSeconds = 0.6f;

    // How long it stands still between hops, in seconds.
    [SerializeField, Min(0f)] private float standSeconds = 1f;

    // Shown while it stands, and while it is in the air. Two states rather than two frames of one.
    [SerializeField] private Sprite standingSprite;
    [SerializeField] private Sprite jumpingSprite;

    private SpriteRenderer art;
    private bool authoredRight;

    private bool hopping;
    private bool turnOnLanding;
    private float hopStartedAt;
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

        // Cut short by a wall rather than refused by one, so it bumps into it instead of stopping
        // short of something it never touched.
        float reach = ReachBefore(from, direction);

        // Already against the wall, so there is nothing left to bump into.
        if (reach < hopDistance * LeastWorthwhileHop)
        {
            Turn();
            return;
        }

        float x = from.x + direction * reach;
        float drop = DistanceToTerrain(new Vector2(x, from.y + hopHeight), Vector2.down, Mathf.Infinity);

        if (float.IsInfinity(drop))
        {
            // Nothing to come down on is the one thing that refuses a hop, since the fall would
            // never end.
            Turn();
            return;
        }

        hopFrom = from;
        landX = x;
        landY = from.y + hopHeight - drop + Feet;
        turnOnLanding = reach < hopDistance;
        hopping = true;
        hopStartedAt = Time.time;
        Show(jumpingSprite);
    }

    // One arc all the way down. Past the end of the hop the same parabola turns downward and
    // accelerates, over the column the ground ray already proved is clear.
    private void Hop()
    {
        float t = (Time.time - hopStartedAt) / hopSeconds;
        float y = hopFrom.y + hopHeight * 4f * t * (1f - t);

        if (t >= 1f && y <= landY)
        {
            Land();
            return;
        }

        float x = Mathf.Lerp(hopFrom.x, landX, Mathf.Min(t, 1f));
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void Land()
    {
        transform.position = new Vector3(landX, landY, transform.position.z);
        hopping = false;
        standingSince = Time.time;
        Show(standingSprite);

        if (turnOnLanding)
        {
            turnOnLanding = false;
            Face(FacesRight == false);
        }
    }

    private void Turn()
    {
        Face(FacesRight == false);
        standingSince = Time.time;
    }

    // How far it can travel before its front meets something solid, never more than a whole hop.
    private float ReachBefore(Vector2 from, float direction)
    {
        float wall = DistanceToTerrain(from, Vector2.right * direction, hopDistance + HalfWidth);

        if (float.IsInfinity(wall))
            return hopDistance;

        // Measured from the middle, so its front stops at the wall face rather than inside it.
        return Mathf.Clamp(wall - HalfWidth, 0f, hopDistance);
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
