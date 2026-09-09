using UnityEngine;

// The red mount's fire. Leaves the mouth small and swells, flies flat to its range, kills what a
// mount's attack may kill, and is gone the moment it touches the ground.
public class ProjectileMountFire : BaseProjectile
{
    // Small as it leaves the mouth, then grown for the rest of the way. Two frames rather than a
    // cycler, since this happens once a flight instead of on a loop.
    [SerializeField] private Sprite starting;
    [SerializeField] private Sprite grown;

    // How far it travels before it swells, in units.
    [SerializeField, Min(0f)] private float growAfter = 0.4f;

    private SpriteRenderer art;

    protected override void Awake()
    {
        base.Awake();

        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Projectile, "No SpriteRenderer found on " + name + ", the flame will not grow");
    }

    // Back to the small frame for every flight, since a pooled projectile is handed out carrying
    // whatever the last one left on it.
    protected override void OnLaunched()
    {
        Show(starting);
    }

    // One distance, two uses: it swells at the near end of it and is gone at the far end, so how
    // far the red mount reaches stays a number in the recipe.
    protected override void Fly()
    {
        float travelled = Vector2.Distance(transform.position, LaunchOrigin);

        if (travelled >= growAfter)
            Show(grown);

        if (Range > 0f && travelled >= Range)
            Despawn();
    }

    protected override void OnHit(Collider2D other)
    {
        // He is standing where this was launched from, so he is flown through rather than treated
        // as terrain.
        if (other.GetComponent<Player>() != null)
            return;

        IDestructible target = other.GetComponent<IDestructible>();

        if (target != null && target.TryDestroy(Destroyer.MountAttack))
        {
            Despawn();
            return;
        }

        // A solid collider is a ground tile: every hazard, pickup and door in this game is a
        // trigger. A fire it cannot put out is flown through like anything else.
        if (other.isTrigger == false)
            Despawn();
    }

    private void Show(Sprite sprite)
    {
        if (art == null || sprite == null || art.sprite == sprite)
            return;

        art.sprite = sprite;
    }
}
