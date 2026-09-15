using UnityEngine;
using Zenject;

// Accumulates a projectile's numbers across Set calls, then stamps them onto a fresh instance of
// whatever prefab it is given. What those numbers should be is the director's business.
public class ProjectileBuilder : IProjectileBuilder
{
    // The container's instantiating half rather than the whole DiContainer, which could resolve
    // anything at all.
    private readonly IInstantiator instantiator;

    private float speed;
    private float lift;
    private float gravityScale;
    private float range;
    private float maxSeconds;

    public ProjectileBuilder(IInstantiator instantiator)
    {
        this.instantiator = instantiator;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetLift(float lift)
    {
        this.lift = lift;
    }

    public void SetGravity(float gravityScale)
    {
        this.gravityScale = gravityScale;
    }

    public void SetRange(float range)
    {
        this.range = range;
    }

    public void SetMaxSeconds(float maxSeconds)
    {
        this.maxSeconds = maxSeconds;
    }

    // Instantiated through the container rather than with Object.Instantiate, so a projectile is
    // injected like anything else - a boomerang needs the player and a fireball needs the flow.
    public BaseProjectile Build(GameObject prefab)
    {
        if (prefab == null)
        {
            GameLog.Warning(LogCategory.Projectile, "No prefab given, nothing was built");
            return null;
        }

        GameObject instance = instantiator.InstantiatePrefab(prefab);
        BaseProjectile projectile = instance.GetComponent<BaseProjectile>();

        if (projectile == null)
        {
            GameLog.Warning(LogCategory.Projectile, "No BaseProjectile found on " + prefab.name + ", it cannot be launched");
            Object.Destroy(instance);
            return null;
        }

        projectile.Configure(speed, lift, gravityScale, range, maxSeconds);
        return projectile;
    }
}
