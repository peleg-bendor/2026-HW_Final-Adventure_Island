using UnityEngine;
using Zenject;

// The recipes: what makes something an axe rather than a boomerang, and how many of each may be in
// the air. Builds every one of them at startup, so nothing is instantiated mid-game.
public class ProjectileDirector : IInitializable
{
    // Thrown forward and up, falling under its own weight until it lands.
    private const float AxeSpeed = 8f;
    private const float AxeLift = 6f;
    private const float AxeGravity = 2f;

    // His own number, watching the original: three go out, then you wait for one to come back.
    private const int AxeCount = 3;

    // A backstop rather than a design timer. An axe thrown over a pit would otherwise fall forever
    // and never come back to the pool.
    private const float AxeMaxSeconds = 6f;

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
    }

    public void ThrowAxe(Vector2 origin, float direction)
    {
        BaseProjectile axe = pool.Get(prefabs.axe);

        if (axe == null)
        {
            GameLog.Verbose(LogCategory.Projectile, "Axe throw ignored - all " + AxeCount + " are already in flight");
            return;
        }

        axe.Launch(origin, direction);
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

    private void ConstructAxe()
    {
        builder.SetSpeed(AxeSpeed);
        builder.SetLift(AxeLift);
        builder.SetGravity(AxeGravity);
        builder.SetMaxSeconds(AxeMaxSeconds);
    }
}
