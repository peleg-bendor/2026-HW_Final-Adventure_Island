using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// The game's operations in one place, as a plain C# class: what starting a game, losing a strike
// and finishing a level actually do. MonoBehaviours drive it and listen to it, not contain it.
public class GameFlow : IGameFlow, IResetRegistry
{
    private readonly SessionState session;
    private readonly IPopups popups;
    private readonly List<IResettable> resettables = new List<IResettable>();

    private LevelDefinition[] levels;
    private int currentIndex = -1;

    // True from the moment an ending starts until a new game begins. Nothing outside this class
    // reads it, which is what keeps it from being a state machine everything has to consult.
    private bool ending;

    public GameFlow(SessionState session, IPopups popups)
    {
        this.session = session;
        this.popups = popups;
    }

    public event Action GameStarted;
    public event Action StrikeLost;
    public event Action GameOver;
    public event Action LevelComplete;
    public event Action GameComplete;

    public LevelDefinition CurrentLevel
    {
        get
        {
            EnsureLevels();
            return currentIndex >= 0 && currentIndex < levels.Length ? levels[currentIndex] : null;
        }
    }

    public void Register(IResettable resettable)
    {
        if (resettable != null && resettables.Contains(resettable) == false)
            resettables.Add(resettable);
    }

    public void Unregister(IResettable resettable)
    {
        resettables.Remove(resettable);
    }

    public void StartGame()
    {
        // Restored unconditionally, so a restart always unfreezes whatever left the game frozen.
        Time.timeScale = 1f;
        ending = false;

        session.Restart();
        GameLog.Info(LogCategory.Game, "Game started - " + session.StrikesRemaining + " strikes");
        GameStarted?.Invoke();
        EnterLevel(0);
    }

    public void LoseStrike()
    {
        if (ending)
        {
            GameLog.Info(LogCategory.Game, "Strike ignored - the game is already over");
            return;
        }

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
        ResetAll(ResetScope.AfterStrike);
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

        EnsureLevels();

        if (currentIndex + 1 < levels.Length)
        {
            EnterLevel(currentIndex + 1);
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

    private void EnterLevel(int index)
    {
        EnsureLevels();

        if (index < 0 || index >= levels.Length)
        {
            GameLog.Warning(LogCategory.Game, "No level to enter, the scene holds no LevelDefinition");
            return;
        }

        for (int i = 0; i < levels.Length; i++)
            levels[i].gameObject.SetActive(i == index);

        currentIndex = index;
        ResetAll(ResetScope.Full);
        GameLog.Info(LogCategory.Game, "Level started: " + levels[index].name);
    }

    // Copied before walking, so a resettable that registers or unregisters while being reset cannot
    // change the list underneath the loop.
    private void ResetAll(ResetScope scope)
    {
        IResettable[] snapshot = resettables.ToArray();

        foreach (IResettable resettable in snapshot)
            resettable.ResetTo(scope);
    }

    // Scanned once, inactive roots included, and ordered by each level's own number. Whichever root
    // is already switched on counts as current, so asking before a level is entered still answers.
    private void EnsureLevels()
    {
        if (levels != null)
            return;

        levels = UnityEngine.Object.FindObjectsByType<LevelDefinition>(FindObjectsInactive.Include);
        Array.Sort(levels, (first, second) => first.LevelNumber.CompareTo(second.LevelNumber));

        for (int i = 0; i < levels.Length; i++)
        {
            if (i > 0 && levels[i].LevelNumber == levels[i - 1].LevelNumber)
                GameLog.Warning(LogCategory.Game, "Two levels share number " + levels[i].LevelNumber + ", their order is arbitrary");

            if (currentIndex < 0 && levels[i].gameObject.activeInHierarchy)
                currentIndex = i;
        }
    }
}
