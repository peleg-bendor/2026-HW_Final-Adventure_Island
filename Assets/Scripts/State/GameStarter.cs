using UnityEngine;
using Zenject;

// Starts the game when the scene runs. There is no menu, so pressing Play is the whole of it.
public class GameStarter : MonoBehaviour
{
    private IGameFlow flow;

    [Inject]
    public void Construct(IGameFlow flow)
    {
        this.flow = flow;
    }

    private void Start()
    {
        if (flow == null)
        {
            GameLog.Warning(LogCategory.Game, "No IGameFlow injected, the game will not start");
            return;
        }

        flow.StartGame();
    }
}
