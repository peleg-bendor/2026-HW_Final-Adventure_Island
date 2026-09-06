using System.Threading.Tasks;
using UnityEngine;

// Maps the two endings onto the two panels in the scene. The only class that knows which panel is
// which, so neither the flow nor the popups themselves have to.
public class Popups : MonoBehaviour, IPopups
{
    [SerializeField] private Popup gameOver;
    [SerializeField] private Popup congratulation;

    // Hidden from here and not from each popup's own Awake, which does not run until the object is
    // first activated - and that moment is the one where it is being shown.
    private void Awake()
    {
        if (gameOver != null)
            gameOver.Hide();

        if (congratulation != null)
            congratulation.Hide();
    }

    public Task ShowGameOverAsync()
    {
        return Show(gameOver, "game over");
    }

    public Task ShowCongratulationAsync()
    {
        return Show(congratulation, "congratulation");
    }

    private static Task Show(Popup popup, string popupName)
    {
        if (popup == null)
        {
            GameLog.Warning(LogCategory.Game, "No " + popupName + " popup assigned, the game will go on without one");
            return Task.CompletedTask;
        }

        return popup.ShowAsync();
    }
}
