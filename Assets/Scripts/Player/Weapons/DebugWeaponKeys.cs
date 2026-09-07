using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// A and B hand the player a weapon, standing in for the eggs that will drop them. Development
// speed only, and the eggs are what replace it.
public class DebugWeaponKeys : MonoBehaviour
{
    private IWeaponSlot slot;
    private ProjectilePrefabs prefabs;

    [Inject]
    public void Construct(IWeaponSlot slot, ProjectilePrefabs prefabs)
    {
        this.slot = slot;
        this.prefabs = prefabs;
    }

    private void Update()
    {
        if (slot == null || prefabs == null || Keyboard.current == null)
            return;

        if (Keyboard.current.aKey.wasPressedThisFrame)
            slot.Take(prefabs.axe);

        if (Keyboard.current.bKey.wasPressedThisFrame)
            slot.Take(prefabs.boomerang);
    }
}
