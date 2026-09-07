using UnityEngine;

// Hands out projectiles that already exist. Everything is built once at startup, so the game never
// instantiates a projectile while it is running.
public interface IProjectilePool
{
    void Add(GameObject prefab, BaseProjectile projectile);

    // Null when every copy of this prefab is already in flight, which is what caps how many can be.
    BaseProjectile Get(GameObject prefab);
}
