using System.Collections.Generic;
using UnityEngine;
using Zenject;

// The built projectiles, grouped by the prefab they came from, and built through the director when the
// game starts. Keyed on the prefab rather than one pool class per projectile type.
public class ProjectilePool : IProjectilePool, IResettable, IInitializable
{
    private readonly IProjectileDirector director;
    private readonly ProjectilePrefabs prefabs;
    private readonly ProjectileCounts counts;
    private readonly IResetRegistry registry;
    private readonly Transform parent;

    private readonly Dictionary<GameObject, List<BaseProjectile>> byPrefab =
        new Dictionary<GameObject, List<BaseProjectile>>();

    // The kinds that may be built past their starting count. None of the player's, whose counts are
    // rules; the snake's, since no one can know how many shooters a level holds.
    private readonly HashSet<GameObject> growable = new HashSet<GameObject>();

    public ProjectilePool(IProjectileDirector director, ProjectilePrefabs prefabs, ProjectileCounts counts,
        IResetRegistry registry, Transform parent)
    {
        this.director = director;
        this.prefabs = prefabs;
        this.counts = counts;
        this.registry = registry;
        this.parent = parent;
    }

    // Filled here, once, so after startup the only projectiles ever built are a snake's extra ones.
    public void Initialize()
    {
        registry.Register(this);

        Fill(prefabs.axe, counts.axe, grows: false);
        Fill(prefabs.boomerang, counts.boomerang, grows: false);
        Fill(prefabs.snakeFireball, counts.snakeFireball, grows: true);
        Fill(prefabs.mountFire, counts.mountFire, grows: false);
    }

    private void Fill(GameObject prefab, int count, bool grows)
    {
        if (prefab == null)
        {
            GameLog.Warning(LogCategory.Projectile, "A projectile prefab is unassigned on GameInstaller, none of that kind was built");
            return;
        }

        byPrefab[prefab] = new List<BaseProjectile>();

        if (grows)
            growable.Add(prefab);

        int built = 0;

        for (int i = 0; i < count; i++)
        {
            if (AddOne(prefab) != null)
                built++;
        }

        GameLog.Info(LogCategory.Projectile, "Pooled " + built + " of " + prefab.name);
    }

    // Parked as it is built: under the projectiles root and switched off, which is what free means.
    private BaseProjectile AddOne(GameObject prefab)
    {
        BaseProjectile projectile = director.Build(prefab);

        if (projectile == null)
            return null;

        if (parent != null)
            projectile.transform.SetParent(parent);

        projectile.gameObject.SetActive(false);
        byPrefab[prefab].Add(projectile);
        return projectile;
    }

    // A free one is an inactive one, so nothing has to be handed back and nothing can be handed
    // back twice.
    public BaseProjectile Get(GameObject prefab)
    {
        List<BaseProjectile> built;

        if (prefab == null || byPrefab.TryGetValue(prefab, out built) == false)
        {
            GameLog.Warning(LogCategory.Projectile, "Nothing was built for that prefab, the pool has none to give");
            return null;
        }

        foreach (BaseProjectile projectile in built)
        {
            if (projectile.gameObject.activeSelf == false)
                return projectile;
        }

        // Refused for a kind whose count is a rule, so a fourth axe and a fourth flame are both
        // impossible.
        if (growable.Contains(prefab) == false)
        {
            GameLog.Verbose(LogCategory.Projectile, prefab.name + " throw ignored - every copy is already in flight");
            return null;
        }

        // Stops once the pool holds as many as have ever been in the air at once, since pools do not
        // shrink.
        BaseProjectile extra = AddOne(prefab);

        if (extra != null)
            GameLog.Verbose(LogCategory.Projectile, "Pooled one more " + prefab.name + " - this level wanted more");

        return extra;
    }

    // Anything still in the air when a level restarts belongs to the run that just ended.
    public void ResetTo(ResetScope scope)
    {
        int recalled = 0;

        foreach (List<BaseProjectile> built in byPrefab.Values)
        {
            foreach (BaseProjectile projectile in built)
            {
                if (projectile.gameObject.activeSelf == false)
                    continue;

                projectile.gameObject.SetActive(false);
                recalled++;
            }
        }

        if (recalled > 0)
            GameLog.Verbose(LogCategory.Projectile, "Recalled " + recalled + " in flight");
    }
}
