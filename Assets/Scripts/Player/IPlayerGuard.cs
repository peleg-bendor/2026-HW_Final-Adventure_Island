// Whether the player survives touching something dangerous. The one place the riding rule and the
// fairy rule live, rather than repeated in every hazard and every enemy.
public interface IPlayerGuard
{
    // Survived is not the same as destroyed: riding into a ghost costs the mount and leaves the
    // ghost standing.
    bool TryAbsorb(IDestructible source);
}
