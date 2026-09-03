using System;
using System.Collections.Generic;
using UnityEngine;

// The game's operations in one place, as a plain C# class: what starting a game, losing a strike and
// finishing a level actually do. MonoBehaviours drive it and listen to it rather than containing it.
public class GameFlow : IGameFlow, IResetRegistry
{
    private readonly SessionState session;
    private readonly List<IResettable> resettables = new List<IResettable>();

    private LevelDefinition[] levels;
    private int currentIndex = -1;

    public GameFlow(SessionState session)
    {
        this.session = session;
    }

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
        session.Restart();
        GameLog.Info(LogCategory.Game, "Game started - " + session.StrikesRemaining + " strikes");
        EnterLevel(0);
    }

    public void LoseStrike()
    {
        session.LoseStrike();
        StrikeLost?.Invoke();

        if (session.StrikesRemaining == 0)
        {
            GameLog.Info(LogCategory.Game, "Game over - no strikes left");
            GameOver?.Invoke();
            return;
        }

        GameLog.Info(LogCategory.Game, "Strike lost - " + session.StrikesRemaining + " remaining");
        ResetAll(ResetScope.AfterStrike);
    }

    // The only branch in the transition is whether another level exists, so nothing here has to
    // change when a level is added or removed.
    public void CompleteLevel()
    {
        GameLog.Info(LogCategory.Game, "Level complete");
        LevelComplete?.Invoke();

        EnsureLevels();

        if (currentIndex + 1 < levels.Length)
        {
            EnterLevel(currentIndex + 1);
            return;
        }

        // Stops here rather than restarting. Both endings wait for something to call StartGame,
        // because 2.3 and 2.4 both put a button between the ending and the next game.
        GameLog.Info(LogCategory.Game, "Game complete - every level finished");
        GameComplete?.Invoke();
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
