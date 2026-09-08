using UnityEngine;

// A spider hanging in the air, dropping to the ground beneath it and rising back to where it hangs.
// How far it drops is the level's geometry rather than a number, so it cannot be sent through a floor.
public class Spider : Enemy
{
    // Whether it drops and rises at all. A still spider hangs where it was placed.
    [SerializeField] private bool moves = true;

    // How fast it drops and rises, in units per second.
    [SerializeField, Min(0f)] private float moveSpeed = 2f;

    private Collider2D body;
    private float travel;

    // An axe, a boomerang, an animal or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // A still spider shows the one frame its prefab carries; the cycling pair is what reads as
    // movement.
    protected override void OnAwake()
    {
        body = GetComponent<Collider2D>();

        if (moves)
            return;

        SpriteCycleAnimator cycle = GetComponent<SpriteCycleAnimator>();

        if (cycle != null)
            cycle.enabled = false;
    }

    protected override void OnSpawned()
    {
        travel = moves ? MeasureDrop() : 0f;
    }

    protected override void Behave()
    {
        if (travel <= 0f)
            return;

        // Read off the clock rather than counted per instance, so every spider in a level drops in
        // step with the others.
        float drop = Mathf.PingPong(Time.time * moveSpeed, travel);
        transform.position = new Vector3(Home.x, Home.y - drop, transform.position.z);
    }

    // How far the pivot can fall before the sprite's feet reach whatever is underneath.
    private float MeasureDrop()
    {
        float nearest = Mathf.Infinity;

        // Terrain is the only solid collider in this game, so skipping triggers finds the ground and
        // nothing else - including this spider's own.
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(Home, Vector2.down))
        {
            if (hit.collider != null && hit.collider.isTrigger == false)
                nearest = Mathf.Min(nearest, hit.distance);
        }

        if (float.IsInfinity(nearest))
        {
            GameLog.Warning(LogCategory.Enemy, "No floor under " + name + ", it hangs still");
            return 0f;
        }

        float feet = body != null ? transform.position.y - body.bounds.min.y : 0f;
        return Mathf.Max(0f, nearest - feet);
    }
}
