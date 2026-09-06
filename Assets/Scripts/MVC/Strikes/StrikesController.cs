using Zenject;

// Keeps the strikes display in step with the session. ISessionState is the model and already owns
// the count, so all this decides is when the view is told.
public class StrikesController : IInitializable
{
    private readonly ISessionState session;
    private readonly IStrikesView view;
    private readonly IGameFlow flow;

    public StrikesController(ISessionState session, IStrikesView view, IGameFlow flow)
    {
        this.session = session;
        this.view = view;
        this.flow = flow;
    }

    // Run from the scene kernel's Start, at execution order -9997, so both events are subscribed
    // before anything at the default order can start a game.
    public void Initialize()
    {
        flow.GameStarted += Publish;
        flow.StrikeLost += Publish;
    }

    private void Publish()
    {
        view.ShowStrikes(session.StrikesRemaining);
    }
}
