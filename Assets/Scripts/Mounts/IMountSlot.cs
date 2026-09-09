using System;

// What the player is riding. A definition asset rather than a name for it, the way the weapon slot
// holds a prefab: nothing else in the game needs to ask which mount he is on.
public interface IMountSlot
{
    // Null when he is on foot.
    MountDefinition Current { get; }

    // Raised on every change, so his body and his picture follow one event rather than polling.
    event Action Changed;

    // Replaces whatever he was riding, since taking a second token swaps.
    void Take(MountDefinition mount);

    // Public because a mount is lost two ways: with a strike, which this class hears for itself,
    // and by absorbing a hit, which is the guard's to decide.
    void Clear();
}
