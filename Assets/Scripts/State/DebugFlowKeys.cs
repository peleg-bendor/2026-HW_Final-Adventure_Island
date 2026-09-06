using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// Forces a strike and a level completion from the keyboard, so the loop can be tested without
// waiting out the power drain or walking to a door.
public class DebugFlowKeys : MonoBehaviour
{
    private IGameFlow flow;

    [Inject]
    public void Construct(IGameFlow flow)
    {
        this.flow = flow;
    }

    private void Update()
    {
        if (flow == null || Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            flow.LoseStrike();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            flow.CompleteLevel();
    }
}
