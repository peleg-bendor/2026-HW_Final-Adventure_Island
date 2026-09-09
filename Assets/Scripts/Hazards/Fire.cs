using Zenject;

// A fire burning in the level. Touching it costs a strike, and it is the hazard that stands through
// everything a weapon can do to it.
public class Fire : Hazard
{
    private IGameFlow flow;

    [Inject]
    public void Construct(IGameFlow flow)
    {
        this.flow = flow;
    }

    // Not an axe, not a boomerang, not a mount's attack.
    protected override Destroyer DestroyedBy
    {
        get { return Destroyer.Fairy | Destroyer.Riding; }
    }

    protected override void Hurt()
    {
        if (flow == null)
        {
            GameLog.Warning(LogCategory.Hazard, "No IGameFlow injected, the fire costs nothing");
            return;
        }

        GameLog.Info(LogCategory.Hazard, "Fire touched - a strike is owed");
        flow.LoseStrike();
    }
}
