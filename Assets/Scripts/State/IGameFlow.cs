using System;

// The game's operations and the events they raise. Which level is on is ILevels' to answer, and
// joining a reset is IResetRegistry's.
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
