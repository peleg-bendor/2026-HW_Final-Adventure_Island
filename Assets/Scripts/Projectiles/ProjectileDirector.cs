using System.Collections.Generic;
using UnityEngine;
using Zenject;

// The recipes: what makes something an axe rather than a boomerang, and how many of each the pool
// starts with. It keeps every recipe, so a kind whose count is not a game rule can be added to.
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

    // Flat and weightless, so a snake's reach is exactly its range.
    private const float SnakeFireballSpeed = 6f;
    private const float SnakeFireballLift = 0f;
    private const float SnakeFireballGravity = 0f;
    private const float SnakeFireballRange = 9f;

    // Two snakes' worth to start with. How many a level really wants depends on how many shooters
    // are in it, which is not knowable from here, so this one grows.
    private const int SnakeFireballCount = 4;

    // A backstop, for a fireball whose range is ever set to zero.
    private const float SnakeFireballMaxSeconds = 4f;

    // A recipe, and whether the count it came with is a rule of the game or only a starting size.
    // The axe's three is 6.7; a snake's flames are however many its level turns out to need.
    private class Recipe
    {
        public System.Action Construct;
        public bool CountIsARule;
    }

    private readonly Dictionary<GameObject, Recipe> recipes = new Dictionary<GameObject, Recipe>();

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
        Fill(prefabs.axe, AxeCount, ConstructAxe, true);
        Fill(prefabs.boomerang, BoomerangCount, ConstructBoomerang, true);
        Fill(prefabs.snakeFireball, SnakeFireballCount, ConstructSnakeFireball, false);
    }

    // One recipe, applied as many times as that projectile starts with. The delegate is what keeps
    // this from being a switch over projectile kinds.
    private void Fill(GameObject prefab, int count, System.Action construct, bool countIsARule)
    {
        if (prefab == null)
        {
            GameLog.Warning(LogCategory.Projectile, "A projectile prefab is unassigned on GameInstaller, none of that kind was built");
            return;
        }

        recipes[prefab] = new Recipe { Construct = construct, CountIsARule = countIsARule };

        for (int i = 0; i < count; i++)
            Build(prefab);

        GameLog.Info(LogCategory.Projectile, "Pooled " + count + " of " + prefab.name);
    }

    private BaseProjectile Build(GameObject prefab)
    {
        recipes[prefab].Construct();
        BaseProjectile projectile = builder.Build(prefab);
        pool.Add(prefab, projectile);
        return projectile;
    }

    // One method however many projectiles there are, since the caller already holds the prefab it
    // wants thrown. A method per kind would grow this class every time one was added.
    public void Throw(GameObject prefab, Vector2 origin, float direction)
    {
        if (prefab == null)
            return;

        BaseProjectile projectile = pool.Get(prefab);

        if (projectile == null)
            projectile = AddOne(prefab);

        if (projectile == null)
        {
            GameLog.Verbose(LogCategory.Projectile, prefab.name + " throw ignored - every copy is already in flight");
            return;
        }

        projectile.Launch(origin, direction);
    }

    // Refused for a kind whose count is a rule, so a fourth axe is impossible. For the rest this
    // settles in the first seconds of a level and never happens again, since pools do not shrink.
    private BaseProjectile AddOne(GameObject prefab)
    {
        Recipe recipe;

        if (recipes.TryGetValue(prefab, out recipe) == false || recipe.CountIsARule)
            return null;

        BaseProjectile extra = Build(prefab);
        GameLog.Verbose(LogCategory.Projectile, "Pooled one more " + prefab.name + " - this level wanted more");
        return extra;
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
}
