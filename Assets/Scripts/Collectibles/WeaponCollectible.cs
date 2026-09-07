using UnityEngine;
using Zenject;

// A weapon lying in the level. All it grants is the projectile prefab it hands to the slot, which
// replaces whatever the player was already carrying.
public class WeaponCollectible : Collectible
{
    // What this weapon throws. The prefab is the weapon, as far as anything else is concerned.
    [SerializeField] private GameObject projectilePrefab;

    private IWeaponSlot slot;

    [Inject]
    public void Construct(IWeaponSlot slot)
    {
        this.slot = slot;
    }

    protected override void PickUp()
    {
        if (slot == null || projectilePrefab == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No IWeaponSlot injected or no projectile set, the weapon gives nothing");
            return;
        }

        slot.Take(projectilePrefab);
    }
}
