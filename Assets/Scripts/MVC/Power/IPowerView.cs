// Draws the power bar. Owns no count of its own - the controller tells it what to show.
public interface IPowerView
{
    void ShowPower(int current, int capacity);
}
