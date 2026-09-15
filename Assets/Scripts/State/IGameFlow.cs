using System;

// The game's operations and the events they raise. Where the player is depends on ILevels instead,
// and what a reset restores on IResetRegistry.
public interface IGameFlow
{
    event Action GameStarted;
    event Action StrikeLost;
    event Action FruitTaken;

    void StartGame();
    void LoseStrike();
    void TakeFruit();
    void CompleteLevel();
}
