using UnityEngine;

// Hands out a projectile of a given kind that is not in flight. Nothing is ever handed back: a
// projectile that switches itself off is free again.
public interface IProjectilePool
{
    // Null when every copy of this prefab is in flight and its count is a rule of the game.
    BaseProjectile Get(GameObject prefab);
}
