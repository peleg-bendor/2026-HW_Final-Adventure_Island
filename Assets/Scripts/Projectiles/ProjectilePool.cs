using System.Collections.Generic;
using UnityEngine;
using Zenject;

// The built projectiles, grouped by the prefab they came from. Keyed on the prefab rather than one
// pool class per projectile type, which is what four copies of Exercise 3's pool would have been.
public class ProjectilePool : IProjectilePool, IResettable, IInitializable
{
    private readonly Dictionary<GameObject, List<BaseProjectile>> byPrefab =
        new Dictionary<GameObject, List<BaseProjectile>>();

    private readonly IResetRegistry registry;

    public ProjectilePool(IResetRegistry registry)
    {
        this.registry = registry;
    }

    public void Initialize()
    {
        registry.Register(this);
    }

    public void Add(GameObject prefab, BaseProjectile projectile)
    {
        if (prefab == null || projectile == null)
            return;

        if (byPrefab.ContainsKey(prefab) == false)
            byPrefab[prefab] = new List<BaseProjectile>();

        byPrefab[prefab].Add(projectile);
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

        return null;
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
