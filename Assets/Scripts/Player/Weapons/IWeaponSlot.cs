using UnityEngine;

// What the player is carrying. A projectile prefab rather than a name for it: nothing in the game
// needs to know which weapon he has, and a name would only need mapping back to a prefab.
public interface IWeaponSlot
{
    // Null when he is empty-handed.
    GameObject Held { get; }

    // Replaces whatever was there, since he carries one weapon at a time.
    void Take(GameObject projectilePrefab);
}
