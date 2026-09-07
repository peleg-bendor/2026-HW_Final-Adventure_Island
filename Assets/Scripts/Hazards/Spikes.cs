using UnityEngine;
using Zenject;

// The floor of a pit, and the only hazard that never asks the player's guard. Nothing destroys it
// and nothing protects him from it, which is why it is not a Hazard.
public class Spikes : MonoBehaviour
{
    private IGameFlow flow;

    [Inject]
    public void Construct(IGameFlow flow)
    {
        this.flow = flow;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        if (flow == null)
        {
            GameLog.Warning(LogCategory.Hazard, "No IGameFlow injected, the spikes cost nothing");
            return;
        }

        GameLog.Info(LogCategory.Hazard, "Spikes touched - a strike is owed");
        flow.LoseStrike();
    }
}
