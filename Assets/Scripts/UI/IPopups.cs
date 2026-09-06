using System.Threading.Tasks;

// What the flow asks for when a game ends. A Task rather than an event, so the whole sequence -
// end, wait for the player, start again - stays in one place instead of splitting across a listener.
public interface IPopups
{
    Task ShowGameOverAsync();
    Task ShowCongratulationAsync();
}
