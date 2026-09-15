using System;
using System.Collections.Generic;
using UnityEngine;

// The recipes: the numbers that make something an axe rather than a boomerang, each fed to the builder
// in order. How many of each exist and who throws them are the pool's and the shooters' business.
public class ProjectileDirector : IProjectileDirector
{
    // Thrown forward and up, falling under its own weight until it lands. No range: it flies until
    // it hits something.
    private const float AxeSpeed = 8f;
    private const float AxeLift = 6f;
    private const float AxeGravity = 2f;
    private const float AxeRange = 0f;

    // A backstop rather than a design timer. An axe thrown over a pit would otherwise fall forever
    // and never come back to the pool.
    private const float AxeMaxSeconds = 6f;

    // Out on a shallow arc to its range, then straight home to wherever he is by then. The two legs
    // taking different paths is what makes the flight read as a loop rather than as one line.
    private const float BoomerangSpeed = 10f;
    private const float BoomerangLift = 6f;
    private const float BoomerangGravity = 1.5f;
    private const float BoomerangRange = 6f;

    // The same backstop, for a return that never arrives. A leaked boomerang would be the only one
    // there is.
    private const float BoomerangMaxSeconds = 10f;

    // Flat and weightless, so a snake's reach is exactly its range.
    private const float SnakeFireballSpeed = 6f;
    private const float SnakeFireballLift = 0f;
    private const float SnakeFireballGravity = 0f;
    private const float SnakeFireballRange = 9f;

    // A backstop, for a fireball whose range is ever set to zero.
    private const float SnakeFireballMaxSeconds = 4f;

    // Flat and weightless like the snake's, and deliberately shorter: a few tiles ahead rather than
    // across the screen.
    private const float MountFireSpeed = 7f;
    private const float MountFireLift = 0f;
    private const float MountFireGravity = 0f;
    private const float MountFireRange = 5f;

    // The same backstop as the snake's, for the same reason.
    private const float MountFireMaxSeconds = 3f;

    private readonly IProjectileBuilder builder;

    // Keyed on the prefab reference the pool asks with. A delegate per kind is what keeps this from
    // being a switch over projectile kinds.
    private readonly Dictionary<GameObject, Action> recipes = new Dictionary<GameObject, Action>();

    public ProjectileDirector(IProjectileBuilder builder, ProjectilePrefabs prefabs)
    {
        this.builder = builder;

        Add(prefabs.axe, ConstructAxe);
        Add(prefabs.boomerang, ConstructBoomerang);
        Add(prefabs.snakeFireball, ConstructSnakeFireball);
        Add(prefabs.mountFire, ConstructMountFire);
    }

    // The recipe and the build in one call, so nothing can build in between with the numbers another
    // recipe left in the builder.
    public BaseProjectile Build(GameObject prefab)
    {
        if (prefab == null)
            return null;

        Action construct;

        if (recipes.TryGetValue(prefab, out construct) == false)
        {
            GameLog.Warning(LogCategory.Projectile, "No recipe for " + prefab.name + ", nothing was built");
            return null;
        }

        construct();
        return builder.Build(prefab);
    }

    // An unassigned prefab gets no recipe. The pool reports it, since the pool is what goes without.
    private void Add(GameObject prefab, Action construct)
    {
        if (prefab != null)
            recipes[prefab] = construct;
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

    private void ConstructSnakeFireball()
    {
        builder.SetSpeed(SnakeFireballSpeed);
        builder.SetLift(SnakeFireballLift);
        builder.SetGravity(SnakeFireballGravity);
        builder.SetRange(SnakeFireballRange);
        builder.SetMaxSeconds(SnakeFireballMaxSeconds);
    }

    private void ConstructMountFire()
    {
        builder.SetSpeed(MountFireSpeed);
        builder.SetLift(MountFireLift);
        builder.SetGravity(MountFireGravity);
        builder.SetRange(MountFireRange);
        builder.SetMaxSeconds(MountFireMaxSeconds);
    }
}
