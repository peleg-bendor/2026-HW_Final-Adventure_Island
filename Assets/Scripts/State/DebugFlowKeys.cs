using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// Drives the flow from the keyboard while nothing in the game can. Key 3 stands in for the button
// on the game over and congratulation popups, which is the only thing that restarts a game.
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

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            flow.StartGame();
    }
}
