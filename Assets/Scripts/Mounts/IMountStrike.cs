using UnityEngine;

// The hit a mount's attack puts into the world, shaped when the attack begins and gone when it ends.
// Injected into the attack rather than looked up among the player's children.
public interface IMountStrike
{
    void Begin(Sprite sprite, Vector2 offset, Vector2 size);

    void End();
}
