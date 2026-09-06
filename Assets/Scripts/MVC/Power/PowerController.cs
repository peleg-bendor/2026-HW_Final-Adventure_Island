using UnityEngine;
using Zenject;

// Drives power: drains it as time passes, applies what fruit gives, and spends a strike when it
// empties. A plain C# class on ITickable, so none of the rules need a MonoBehaviour.
public class PowerController : ITickable, IInitializable, IResettable, IPower
{
    private readonly IPowerModel model;
    private readonly IPowerView view;
    private readonly IGameFlow flow;
    private readonly ILevels levels;
    private readonly IResetRegistry registry;
    private readonly float drainSeconds;

    private float sinceLastDrain;

    public PowerController(IPowerModel model, IPowerView view, IGameFlow flow, ILevels levels, IResetRegistry registry, float drainSeconds)
    {
        this.model = model;
        this.view = view;
        this.flow = flow;
        this.levels = levels;
        this.registry = registry;
        this.drainSeconds = drainSeconds;
    }

    // Run from the scene kernel's Start, at execution order -9997, so this registers before
    // anything at the default order can start a game.
    public void Initialize()
    {
        registry.Register(this);
    }

    // No check for whether the game is running: a popup freezes time, so Time.deltaTime is zero
    // and the drain stops without being told to.
    public void Tick()
    {
        if (drainSeconds <= 0f)
            return;

        sinceLastDrain += Time.deltaTime;

        if (sinceLastDrain < drainSeconds)
            return;

        // Subtracted rather than zeroed, so a long frame keeps its overshoot instead of drifting
        // the drain slower than it should be.
        sinceLastDrain -= drainSeconds;
        Spend(1);
    }

    public int Gain(int amount)
    {
        int gained = model.Add(amount);

        if (gained > 0)
            Publish();

        return gained;
    }

    public void Spend(int amount)
    {
        if (model.Remove(amount) <= 0)
            return;

        Publish();

        // Reaching zero costs a strike. That is a game rule, which is why the model has no opinion
        // about zero.
        if (model.Current == 0)
            flow.LoseStrike();
    }

    public void ResetTo(ResetScope scope)
    {
        LevelDefinition level = levels.Current;

        model.SetTo(level != null ? level.StartingPower : model.Capacity);
        sinceLastDrain = 0f;
        Publish();
    }

    private void Publish()
    {
        view.ShowPower(model.Current, model.Capacity);
        GameLog.Verbose(LogCategory.Player, "Power " + model.Current + " of " + model.Capacity);
    }
}
