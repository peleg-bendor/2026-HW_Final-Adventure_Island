using UnityEngine;
using Zenject;

// A snake that stands still and fires at the player while he is near, and stops when he is not. It
// turns to face him, so where it was placed never decides whether it can fight back.
public class SnakeShooter : Enemy
{
    // How long between shots, in seconds.
    [SerializeField, Min(0.1f)] private float secondsBetweenShots = 1.2f;

    // Where a shot leaves from, relative to the pivot at its feet. X mirrors with its facing.
    [SerializeField] private Vector2 muzzle = new Vector2(0.6f, 0.9f);

    // How long the firing pose is held after a shot, in seconds.
    [SerializeField, Min(0f)] private float poseSeconds = 0.3f;

    // Shown while it waits, and just after it fires. Two states rather than two frames of one.
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite shootingSprite;

    private ProjectileDirector director;
    private ProjectilePrefabs prefabs;
    private SpriteRenderer art;
    private float lastShotAt;
    private bool posing;

    // The prefabs come from the installer rather than a field of its own, since the pool is keyed on
    // the prefab and a second reference could point somewhere the pool never built from.
    [Inject]
    public void Construct(ProjectileDirector director, ProjectilePrefabs prefabs)
    {
        this.director = director;
        this.prefabs = prefabs;
    }

    // An axe, a boomerang, a mount or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.MountAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    protected override void OnAwake()
    {
        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Enemy, "No SpriteRenderer found on " + name + ", it will not change pose");
    }

    protected override void OnSpawned()
    {
        // Counted from the spawn, so it does not fire on the frame it appears; a player already in
        // range then gets shot at as soon as the interval is up.
        lastShotAt = Time.time;
        posing = false;
        Show(idleSprite);
    }

    protected override void Behave()
    {
        Face(PlayerPosition.x > transform.position.x);

        if (posing && Time.time - lastShotAt >= poseSeconds)
        {
            posing = false;
            Show(idleSprite);
        }

        if (Time.time - lastShotAt >= secondsBetweenShots)
            Fire();
    }

    private void Fire()
    {
        lastShotAt = Time.time;

        if (director == null || prefabs == null || prefabs.snakeFireball == null)
        {
            GameLog.Warning(LogCategory.Enemy, "No ProjectileDirector or no snake fireball prefab, " + name + " cannot shoot");
            return;
        }

        posing = true;
        Show(shootingSprite);

        float direction = FacesRight ? 1f : -1f;
        Vector2 origin = (Vector2)transform.position + new Vector2(muzzle.x * direction, muzzle.y);
        director.Throw(prefabs.snakeFireball, origin, direction);
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
