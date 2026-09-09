using System;
using System.Threading.Tasks;
using UnityEngine;

// The game's operations in one place, as a plain C# class: what starting a game, losing a strike,
// taking fruit and finishing a level actually do. MonoBehaviours drive it and listen to it.
public class GameFlow : IGameFlow
{
    private readonly SessionState session;
    private readonly ILevels levels;
    private readonly IResetRunner resets;
    private readonly IPopups popups;

    // How much fruit costs a strike. The count is not cleared when it fires.
    private readonly int fruitPerStrike;

    // True from the moment an ending starts until a new game begins. Nothing outside this class
    // reads it, which is what keeps it from being a state machine everything has to consult.
    private bool ending;

    private int lastStrikeFrame = -1;

    public GameFlow(SessionState session, ILevels levels, IResetRunner resets, IPopups popups, int fruitPerStrike)
    {
        this.session = session;
        this.levels = levels;
        this.resets = resets;
        this.popups = popups;
        this.fruitPerStrike = fruitPerStrike;
    }

    public event Action GameStarted;
    public event Action StrikeLost;
    public event Action FruitTaken;
    public event Action GameOver;
    public event Action LevelComplete;
    public event Action GameComplete;

    public void StartGame()
    {
        // Restored unconditionally, so a restart always unfreezes whatever left the game frozen.
        Time.timeScale = 1f;
        ending = false;

        session.Restart();
        GameLog.Info(LogCategory.Game, "Game started - " + session.StrikesRemaining + " strikes");
        GameStarted?.Invoke();

        if (levels.EnterFirst() == false)
        {
            GameLog.Warning(LogCategory.Game, "No level to enter, the scene holds no LevelDefinition");
            return;
        }

        resets.ResetAll(ResetScope.Full);
    }

    public void LoseStrike()
    {
        if (ending)
        {
            GameLog.Info(LogCategory.Game, "Strike ignored - the game is already over");
            return;
        }

        // One a frame at most, since losing a strike puts him back at the level start: a second
        // charge in the same frame is for a place he has already left.
        if (lastStrikeFrame == Time.frameCount)
        {
            GameLog.Info(LogCategory.Game, "Strike ignored - one was already lost this frame");
            return;
        }

        lastStrikeFrame = Time.frameCount;
        session.LoseStrike();
        StrikeLost?.Invoke();

        if (session.StrikesRemaining == 0)
        {
            GameLog.Info(LogCategory.Game, "Game over - no strikes left");
            GameOver?.Invoke();
            EndGame(popups.ShowGameOverAsync);
            return;
        }

        GameLog.Info(LogCategory.Game, "Strike lost - " + session.StrikesRemaining + " remaining");
        resets.ResetAll(ResetScope.AfterStrike);
    }

    // No guard of its own: the only thing it can start is a strike, and LoseStrike already refuses
    // one while an ending is running.
    public void TakeFruit()
    {
        session.TakeFruit();
        FruitTaken?.Invoke();

        if (session.FruitCount % fruitPerStrike != 0)
            return;

        GameLog.Info(LogCategory.Game, "Fruit reached " + session.FruitCount + " - a strike is owed");
        LoseStrike();
    }

    // The only branch in the transition is whether another level exists, so nothing here has to
    // change when a level is added or removed.
    public void CompleteLevel()
    {
        if (ending)
        {
            GameLog.Info(LogCategory.Game, "Level completion ignored - the game is already over");
            return;
        }

        GameLog.Info(LogCategory.Game, "Level complete");
        LevelComplete?.Invoke();

        if (levels.EnterNext())
        {
            resets.ResetAll(ResetScope.Full);
            return;
        }

        GameLog.Info(LogCategory.Game, "Game complete - every level finished");
        GameComplete?.Invoke();
        EndGame(popups.ShowCongratulationAsync);
    }

    // Started rather than awaited, since nothing that ends a game can await. The popup is passed
    // unstarted so that time is already frozen by the time it appears.
    private async void EndGame(Func<Task> showPopup)
    {
        ending = true;

        try
        {
            Time.timeScale = 0f;
            await showPopup();
            StartGame();
        }
        catch (Exception error)
        {
            // Unfrozen rather than left as it was, since a hard-locked game hides the error above it.
            GameLog.Error(LogCategory.Game, "Ending failed - " + error.Message);
            Time.timeScale = 1f;
            ending = false;
        }
    }
}
