using UnityEngine;
using Zenject;

// Accumulates a projectile's numbers across Set calls, then stamps them onto a fresh instance of
// whatever prefab it is given. What those numbers should be is the director's business.
public class ProjectileBuilder : IProjectileBuilder
{
    private readonly DiContainer container;
    private readonly Transform parent;

    private float speed;
    private float lift;
    private float gravityScale;
    private float maxSeconds;

    public ProjectileBuilder(DiContainer container, Transform parent)
    {
        this.container = container;
        this.parent = parent;
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

        GameObject instance = container.InstantiatePrefab(prefab);
        BaseProjectile projectile = instance.GetComponent<BaseProjectile>();

        if (projectile == null)
        {
            GameLog.Warning(LogCategory.Projectile, "No BaseProjectile found on " + prefab.name + ", it cannot be launched");
            Object.Destroy(instance);
            return null;
        }

        if (parent != null)
            instance.transform.SetParent(parent);

        projectile.Configure(speed, lift, gravityScale, maxSeconds);
        instance.SetActive(false);
        return projectile;
    }
}
