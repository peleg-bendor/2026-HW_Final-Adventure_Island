using UnityEngine;
using Zenject;

// The cave door a level ends at. Reaching it finishes the level; what finishing means - the next
// level, or the end of the game - is the flow's to decide.
public class ExitDoor : MonoBehaviour
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
            GameLog.Warning(LogCategory.Game, "No IGameFlow injected, the door will not end the level");
            return;
        }

        flow.CompleteLevel();
    }
}
