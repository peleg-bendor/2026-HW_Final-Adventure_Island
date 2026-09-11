// Whether the player is holding a fairy. Its own interface rather than a field on the guard, so the
// guard consults it the same way it already consults the mount slot.
public interface IPlayerFairy
{
    bool Active { get; }

    // Replaces whatever is left of the last one, since a second fairy restarts the ten seconds.
    void Take();
}
