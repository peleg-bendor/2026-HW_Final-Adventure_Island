using UnityEngine;
using Zenject;

// A thrown boomerang. Flies out on an arc to its range, turns, and steers at wherever the player is
// by then rather than at where it was thrown from. Destroys what it touches without being stopped.
public class ProjectileBoomerang : BaseProjectile
{
    private Player player;
    private bool returning;

    // The marker is injected rather than an interface, since all it exposes is which object he is
    // and where the middle of him is.
    [Inject]
    public void Construct(Player player)
    {
        this.player = player;
    }

    protected override void OnLaunched()
    {
        returning = false;
    }

    protected override void Fly()
    {
        if (Body == null || player == null)
            return;

        if (returning == false)
        {
            if (Vector2.Distance(transform.position, LaunchOrigin) < Range)
                return;

            returning = true;
            GameLog.Verbose(LogCategory.Projectile, "Boomerang turned");
        }

        // Aimed at his middle rather than his transform, which sits at his feet. Re-aimed every
        // frame rather than turned at a rate, so it cannot miss him however he moves.
        Vector2 toPlayer = player.Middle - (Vector2)transform.position;
        Body.linearVelocity = toPlayer.normalized * Speed;
    }

    protected override void OnHit(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
        {
            // Only counts on the way back: it is launched overlapping him and would otherwise be
            // caught the instant it is thrown.
            if (returning)
            {
                GameLog.Verbose(LogCategory.Projectile, "Boomerang caught");
                Despawn();
            }

            return;
        }

        IDestructible target = other.GetComponent<IDestructible>();

        // Destroys what it touches and flies on, which is the whole difference from the axe. Ground
        // tiles are solid and it passes through those too.
        if (target != null)
            target.TryDestroy(Destroyer.Boomerang);
    }
}
