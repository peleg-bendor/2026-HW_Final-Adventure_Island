// Draws how many strikes are left. Owns no count of its own - the controller tells it what to show.
public interface IStrikesView
{
    void ShowStrikes(int remaining);
}
