using UnityEngine;
using Zenject;

// The recipes: what makes something an axe rather than a boomerang, and how many of each may be in
// the air. Builds every one of them at startup, so nothing is instantiated mid-game.
public class ProjectileDirector : IInitializable
{
    // Thrown forward and up, falling under its own weight until it lands. No range: it flies until
    // it hits something.
    private const float AxeSpeed = 8f;
    private const float AxeLift = 6f;
    private const float AxeGravity = 2f;
    private const float AxeRange = 0f;

    // His own number, watching the original: three go out, then you wait for one to come back.
    private const int AxeCount = 3;

    // A backstop rather than a design timer. An axe thrown over a pit would otherwise fall forever
    // and never come back to the pool.
    private const float AxeMaxSeconds = 6f;

    // Out on a shallow arc to six cells, then straight home to wherever he is by then. The two legs
    // taking different paths is what makes the flight read as a loop rather than as one line.
    private const float BoomerangSpeed = 10f;
    private const float BoomerangLift = 6f;
    private const float BoomerangGravity = 1.5f;
    private const float BoomerangRange = 6f;

    // One, because it comes back to him and a second in the air would mean nothing.
    private const int BoomerangCount = 1;

    // The same backstop, for a return that never arrives. A leaked boomerang would be the only one
    // there is.
    private const float BoomerangMaxSeconds = 10f;

    private readonly IProjectileBuilder builder;
    private readonly IProjectilePool pool;
    private readonly ProjectilePrefabs prefabs;

    public ProjectileDirector(IProjectileBuilder builder, IProjectilePool pool, ProjectilePrefabs prefabs)
    {
        this.builder = builder;
        this.pool = pool;
        this.prefabs = prefabs;
    }

    public void Initialize()
    {
        Fill(prefabs.axe, AxeCount, ConstructAxe);
        Fill(prefabs.boomerang, BoomerangCount, ConstructBoomerang);
    }

    // One recipe, applied as many times as that projectile has copies. The delegate is what keeps
    // this from being a switch over projectile kinds.
    private void Fill(GameObject prefab, int count, System.Action construct)
    {
        if (prefab == null)
        {
            GameLog.Warning(LogCategory.Projectile, "A projectile prefab is unassigned on GameInstaller, none of that kind was built");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            construct();
            pool.Add(prefab, builder.Build(prefab));
        }

        GameLog.Info(LogCategory.Projectile, "Pooled " + count + " of " + prefab.name);
    }

    // One method however many projectiles there are, since the caller already holds the prefab it
    // wants thrown. A method per kind would grow this class every time one was added.
    public void Throw(GameObject prefab, Vector2 origin, float direction)
    {
        if (prefab == null)
            return;

        BaseProjectile projectile = pool.Get(prefab);

        if (projectile == null)
        {
            GameLog.Verbose(LogCategory.Projectile, prefab.name + " throw ignored - every copy is already in flight");
            return;
        }

        projectile.Launch(origin, direction);
    }

    private void ConstructAxe()
    {
        builder.SetSpeed(AxeSpeed);
        builder.SetLift(AxeLift);
        builder.SetGravity(AxeGravity);
        builder.SetRange(AxeRange);
        builder.SetMaxSeconds(AxeMaxSeconds);
    }

    private void ConstructBoomerang()
    {
        builder.SetSpeed(BoomerangSpeed);
        builder.SetLift(BoomerangLift);
        builder.SetGravity(BoomerangGravity);
        builder.SetRange(BoomerangRange);
        builder.SetMaxSeconds(BoomerangMaxSeconds);
    }
}
