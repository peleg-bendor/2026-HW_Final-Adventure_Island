using UnityEngine;
using Zenject;

// A fruit lying in the level. Gives power and counts towards the every-twenty rule; how much power
// is on the prefab, since that is the only thing separating the two kinds.
public class FruitCollectible : Collectible
{
    // How much power this fruit gives.
    [SerializeField] private int powerGiven = 1;

    private IPower power;
    private IGameFlow flow;

    [Inject]
    public void Construct(IPower power, IGameFlow flow)
    {
        this.power = power;
        this.flow = flow;
    }

    protected override void PickUp()
    {
        if (power == null || flow == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No IPower or IGameFlow injected, the fruit gives nothing");
            return;
        }

        int gained = power.Gain(powerGiven);
        flow.TakeFruit();

        GameLog.Info(LogCategory.Collectible, gained > 0
            ? "Fruit taken - " + gained + " power"
            : "Fruit taken - power already full");
    }
}
