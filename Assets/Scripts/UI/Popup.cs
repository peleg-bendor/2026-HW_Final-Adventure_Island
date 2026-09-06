using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

// One panel with one button: shows itself, and completes a Task when the button is clicked. Carries
// no text of its own, since what it says is authored in the scene.
public class Popup : MonoBehaviour
{
    [SerializeField] private Button button;

    private TaskCompletionSource<bool> click;

    // Completes immediately when there is no button, rather than handing back a wait that nothing
    // could ever end.
    public Task ShowAsync()
    {
        if (button == null)
        {
            GameLog.Warning(LogCategory.Game, "No Button assigned to " + name + ", the popup cannot be closed");
            return Task.CompletedTask;
        }

        click = new TaskCompletionSource<bool>();
        button.onClick.AddListener(Close);
        gameObject.SetActive(true);

        return click.Task;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Close()
    {
        button.onClick.RemoveListener(Close);
        Hide();
        click.SetResult(true);
    }
}
