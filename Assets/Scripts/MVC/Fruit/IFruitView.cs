// Draws how much fruit has been collected, and how close the next strike is. Owns no count of its
// own - the controller tells it what to show.
public interface IFruitView
{
    void ShowFruit(int collected, int untilStrike);
}
