using Zenject;

// Keeps the fruit display in step with the session. ISessionState is the model and already owns the
// count, so all this decides is when the view is told and how far the next strike is.
public class FruitController : IInitializable
{
    private readonly ISessionState session;
    private readonly IFruitView view;
    private readonly IGameFlow flow;
    private readonly int fruitPerStrike;

    public FruitController(ISessionState session, IFruitView view, IGameFlow flow, int fruitPerStrike)
    {
        this.session = session;
        this.view = view;
        this.flow = flow;
        this.fruitPerStrike = fruitPerStrike;
    }

    // Run from the scene kernel's Start, at execution order -9997, so both events are subscribed
    // before anything at the default order can start a game.
    public void Initialize()
    {
        flow.GameStarted += Publish;
        flow.FruitTaken += Publish;
    }

    private void Publish()
    {
        int collected = session.FruitCount;

        view.ShowFruit(collected, fruitPerStrike - collected % fruitPerStrike);
    }
}
