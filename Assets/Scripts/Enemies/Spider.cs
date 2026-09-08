using UnityEngine;

// A spider hanging in the air. The moving prefab drops to the ground beneath it and rises back to
// where it hangs; the still one stays where it was placed. How far it drops is the level's geometry.
public class Spider : Enemy
{
    // Whether it drops and rises at all. The two prefabs carry the two answers.
    [SerializeField] private bool moves = true;

    // How fast it drops and rises, in units per second.
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    private float feet;
    private float travel;
    private float spawnedAt;

    // An axe, a boomerang, an animal or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    protected override void OnAwake()
    {
        Collider2D body = GetComponent<Collider2D>();

        if (body == null)
        {
            GameLog.Warning(LogCategory.Enemy, "No Collider2D found on " + name + ", it will stop with its middle in the floor");
            return;
        }

        // How far the pivot sits above the sprite's feet. A constant of the prefab, so it is read
        // here rather than at every spawn.
        feet = transform.position.y - body.bounds.min.y;
    }

    // Measured at every spawn rather than once, so a spider that came back somewhere else still
    // stops at the floor under it.
    protected override void OnSpawned()
    {
        travel = moves ? MeasureDrop() : 0f;
        spawnedAt = Time.time;
    }

    protected override void Behave()
    {
        if (travel <= 0f)
            return;

        // Timed from its own spawn rather than off the shared clock, so it always starts at the top
        // of its swing and a reset looks like one.
        float drop = Mathf.PingPong((Time.time - spawnedAt) * moveSpeed, travel);
        transform.position = new Vector3(Home.x, Home.y - drop, transform.position.z);
    }

    // How far the pivot can fall before the sprite's feet reach whatever is underneath.
    private float MeasureDrop()
    {
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(Home, Vector2.down))
        {
            // Terrain is the only solid collider in this game, so triggers are every hazard, pickup
            // and door - and this spider's own.
            if (hit.collider == null || hit.collider.isTrigger)
                continue;

            // The player is the one solid thing that is not terrain, and a reset may not have moved
            // him out from under this spider yet.
            if (hit.collider.GetComponent<Player>() != null)
                continue;

            nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
        {
            GameLog.Warning(LogCategory.Enemy, "No floor under " + name + ", it hangs still");
            return 0f;
        }

        return Mathf.Max(0f, nearest - feet);
    }
}
