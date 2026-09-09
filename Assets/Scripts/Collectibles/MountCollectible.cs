using UnityEngine;
using Zenject;

// A token lying in the level. All it grants is the mount definition it hands to the slot, which
// replaces whatever the player was already riding.
public class MountCollectible : Collectible
{
    // Which mount this token grants. The asset is the mount, as far as anything else is concerned.
    [SerializeField] private MountDefinition mount;

    private IMountSlot slot;

    [Inject]
    public void Construct(IMountSlot slot)
    {
        this.slot = slot;
    }

    protected override void PickUp()
    {
        if (slot == null || mount == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No IMountSlot injected or no mount set, the token gives nothing");
            return;
        }

        slot.Take(mount);
    }
}
