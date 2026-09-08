using UnityEngine;
using Zenject;

// The shooting snake's fireball. Flies straight, costs the player a strike, and passes through every
// other enemy - which is the whole of the rule that an enemy's shot can never kill another enemy.
public class ProjectileFireball : BaseProjectile, IDestructible
{
    private IGameFlow flow;
    private IPlayerGuard guard;

    [Inject]
    public void Construct(IGameFlow flow, IPlayerGuard guard)
    {
        this.flow = flow;
        this.guard = guard;
    }

    // Riding into it and a fairy are the only things that take it out of the air. An axe and a
    // boomerang fly straight through.
    public bool TryDestroy(Destroyer by)
    {
        if ((by & (Destroyer.Riding | Destroyer.Fairy)) == 0)
            return false;

        Despawn();
        GameLog.Verbose(LogCategory.Projectile, "Fireball put out - " + by);
        return true;
    }

    // Gone at its range rather than left to time out, so how far a snake can reach is a number in
    // the recipe.
    protected override void Fly()
    {
        if (Range > 0f && Vector2.Distance(transform.position, LaunchOrigin) >= Range)
            Despawn();
    }

    protected override void OnHit(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            // Gone before the effect, since a strike runs the level reset from inside this call.
            Despawn();

            if (guard != null && guard.TryAbsorb(this))
                return;

            if (flow == null)
            {
                GameLog.Warning(LogCategory.Projectile, "No IGameFlow injected, the fireball costs nothing");
                return;
            }

            GameLog.Info(LogCategory.Projectile, "Fireball hit the player - a strike is owed");
            flow.LoseStrike();
            return;
        }

        // A solid collider is a ground tile. Every enemy, hazard and pickup is a trigger, so it
        // flies through all of them and never asks any of them to be destroyed.
        if (other.isTrigger == false)
            Despawn();
    }
}
