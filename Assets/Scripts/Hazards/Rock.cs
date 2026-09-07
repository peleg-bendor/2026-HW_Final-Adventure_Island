using UnityEngine;
using Zenject;

// A rock lying in the level. Touching it costs power and flings the player forward, which can carry
// him into a fire; by itself it never costs a strike.
public class Rock : Hazard
{
    // How much power touching this costs.
    [SerializeField, Min(1)] private int powerCost = 3;

    // How fast the player is flung, in units per second.
    [SerializeField] private float shoveSpeed = 10f;

    // How long the shove lasts, and how long he is immune to rocks for.
    [SerializeField] private float shoveSeconds = 0.3f;

    private IPower power;
    private IPlayerShove shove;

    [Inject]
    public void Construct(IPower power, IPlayerShove shove)
    {
        this.power = power;
        this.shove = shove;
    }

    // A boomerang, an animal or a fairy. An axe explicitly does not.
    protected override Destroyer DestroyedBy
    {
        get { return Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding | Destroyer.Fairy; }
    }

    protected override void Hurt()
    {
        if (power == null || shove == null)
        {
            GameLog.Warning(LogCategory.Hazard, "No IPower or IPlayerShove injected, the rock costs nothing");
            return;
        }

        // Read off the player rather than kept per rock, or a shove that lands him on the next one
        // charges him again. Nothing but a rock asks, so the fire he is flung into still kills him.
        if (shove.IsShoving)
        {
            GameLog.Verbose(LogCategory.Hazard, "Rock contact ignored - a shove is still running");
            return;
        }

        shove.Shove(shoveSpeed, shoveSeconds);
        GameLog.Info(LogCategory.Hazard, "Rock touched - costs " + powerCost + " power");
        power.Spend(powerCost);
    }
}
