using UnityEngine;
using Zenject;

// The floor of a pit, and the only hazard that never asks the player's guard. Nothing destroys it
// and nothing protects him from it, which is why it is not a Hazard.
public class Spikes : MonoBehaviour
{
    // Shared by every tile rather than kept per tile, since a pit floor is several of them and one fall
    // touches more than one in the same frame.
    private static int lastTouchFrame = -1;

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

        // The rest of the pit stays quiet this frame: the flow would only refuse a second strike.
        if (lastTouchFrame == Time.frameCount)
            return;

        lastTouchFrame = Time.frameCount;

        if (flow == null)
        {
            GameLog.Warning(LogCategory.Hazard, "No IGameFlow injected, the spikes cost nothing");
            return;
        }

        GameLog.Info(LogCategory.Hazard, "Spikes touched - a strike is owed");
        flow.LoseStrike();
    }
}
