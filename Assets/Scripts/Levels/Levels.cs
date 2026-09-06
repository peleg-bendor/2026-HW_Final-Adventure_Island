using System;
using UnityEngine;

// Finds the levels in the scene, orders them, and switches which one is active. Nothing here knows
// why a level is being entered - that decision is the flow's.
public class Levels : ILevels
{
    private LevelDefinition[] levels;
    private int currentIndex = -1;

    public LevelDefinition Current
    {
        get
        {
            EnsureFound();
            return currentIndex >= 0 && currentIndex < levels.Length ? levels[currentIndex] : null;
        }
    }

    public bool EnterFirst()
    {
        return Enter(0);
    }

    public bool EnterNext()
    {
        EnsureFound();
        return Enter(currentIndex + 1);
    }

    private bool Enter(int index)
    {
        EnsureFound();

        if (index < 0 || index >= levels.Length)
            return false;

        for (int i = 0; i < levels.Length; i++)
            levels[i].gameObject.SetActive(i == index);

        currentIndex = index;
        GameLog.Info(LogCategory.Game, "Level started: " + levels[index].name);
        return true;
    }

    // Scanned once, inactive roots included, and ordered by each level's own number. Whichever root
    // is already switched on counts as current, so asking before a level is entered still answers.
    private void EnsureFound()
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
