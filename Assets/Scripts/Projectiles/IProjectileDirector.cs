using UnityEngine;

// Builds one projectile of the kind a prefab stands for, from that kind's recipe. The pool asks for
// one without learning any of the numbers that make it an axe or a boomerang.
public interface IProjectileDirector
{
    // Null for a prefab with no recipe.
    BaseProjectile Build(GameObject prefab);
}
