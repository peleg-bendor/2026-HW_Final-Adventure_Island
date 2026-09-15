using UnityEngine;

// A spider hanging in the air. The moving prefab drops to the ground beneath it and rises back to
// where it hangs; the still one stays where it was placed. How far it drops is the level's geometry.
public class Spider : Enemy
{
    // Whether it drops and rises at all. The two prefabs carry the two answers.
    [SerializeField] private bool moves = true;

    // How fast it drops and rises, in units per second.
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    private float travel;
    private float spawnedAt;
    private bool warnedNoFloor;

    // The same answer as every other enemy but the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.MountAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // Measured at every spawn rather than once, so the drop matches the floor beneath it at the time.
    protected override void OnSpawned()
    {
        // Not while its level is switched off, where the only ground a cast can find is the other
        // level's. Entering its level spawns it again before it can be seen.
        travel = moves && gameObject.activeInHierarchy ? MeasureDrop() : 0f;
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
        float nearest = Ground.DistanceTo(Home, Vector2.down, Mathf.Infinity);

        if (float.IsInfinity(nearest))
        {
            // Said once rather than at every reset: the level's shape does not change while it runs.
            if (warnedNoFloor == false)
            {
                warnedNoFloor = true;
                GameLog.Warning(LogCategory.Enemy, "No floor under " + name + " at " + Home + ", it hangs still");
            }

            return 0f;
        }

        return Mathf.Max(0f, nearest - Feet);
    }
}
