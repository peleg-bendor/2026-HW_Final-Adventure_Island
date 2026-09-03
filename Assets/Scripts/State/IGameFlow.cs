using System;

// The game's operations and the events they raise. What drives or watches the game depends on this,
// while what only needs restoring depends on IResetRegistry instead.
public interface IGameFlow
{
    event Action StrikeLost;
    event Action GameOver;
    event Action LevelComplete;
    event Action GameComplete;

    LevelDefinition CurrentLevel { get; }

    void StartGame();
    void LoseStrike();
    void CompleteLevel();
}
