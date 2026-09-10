using UnityEngine;

// Makes the pickup a configured drop type stands for. A caller says what and where; which prefab
// that is, how it is built and where it lands in the hierarchy are the factory's alone.
public interface IDropFactory
{
    // Null for None, and for a type whose prefab is missing from the installer's list.
    Collectible Create(DropType type, Vector2 at);
}
