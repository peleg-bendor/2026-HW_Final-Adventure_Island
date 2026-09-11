using UnityEngine;
using Zenject;

// A fairy lying in the level. All it grants is the ten seconds, which the player's own component is
// what counts down.
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
