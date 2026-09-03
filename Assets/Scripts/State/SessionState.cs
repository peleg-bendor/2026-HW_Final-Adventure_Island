// What survives a death and a level change: the פסילות left and the fruit taken so far. Cleared
// only by restarting the game, which is why it is not an IResettable.
public class SessionState
{
    private readonly int startingStrikes;

    public SessionState(int startingStrikes)
    {
        this.startingStrikes = startingStrikes;
        StrikesRemaining = startingStrikes;
    }

    public int StrikesRemaining { get; private set; }

    public int FruitTaken { get; private set; }

    public void Restart()
    {
        StrikesRemaining = startingStrikes;
        FruitTaken = 0;
    }

    // Never below zero, so a second call after the last strike cannot make the count negative and
    // hide a game over that has already happened.
    public void LoseStrike()
    {
        if (StrikesRemaining > 0)
            StrikesRemaining--;
    }

    public void TakeFruit()
    {
        FruitTaken++;
    }
}
