using UnityEngine;
using Zenject;

// A fairy lying in the level. Picking it up hands it to the player, whose own component holds it and
// counts it down.
public class FairyCollectible : Collectible
{
    private IPlayerFairy fairy;

    [Inject]
    public void Construct(IPlayerFairy fairy)
    {
        this.fairy = fairy;
    }

    protected override void PickUp()
    {
        if (fairy == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No IPlayerFairy injected, the fairy gives nothing");
            return;
        }

        fairy.Take();
    }
}
