using UnityEngine;

// A thrown axe. Arcs under its own weight, kills what an axe is allowed to kill, and is gone the
// moment it touches the ground - there is no landing and no picking it back up.
public class ProjectileAxe : BaseProjectile
{
    protected override void OnHit(Collider2D other)
    {
        // The thrower is standing where this was launched from, so he is flown through rather than
        // treated as terrain.
        if (other.GetComponent<Player>() != null)
            return;

        IDestructible target = other.GetComponent<IDestructible>();

        if (target != null && target.TryDestroy(Destroyer.Axe))
        {
            Despawn();
            return;
        }

        // A solid collider is a ground tile: every hazard, pickup and door in this game is a
        // trigger. A rock refuses an axe, so it is flown through like anything else it cannot kill.
        if (other.isTrigger == false)
        {
            GameLog.Verbose(LogCategory.Projectile, "Axe hit the ground");
            Despawn();
        }
    }
}
