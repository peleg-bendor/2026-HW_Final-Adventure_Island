using System;

// Orders the level roots it is given and switches which one is active. Nothing here knows why a
// level is being entered - that decision is the flow's.
public class Levels : ILevels
{
    private readonly LevelDefinition[] levels;
    private int currentIndex = -1;

    // Given every level root by the installer, inactive ones included, rather than searching the
    // scene for them. Ordered by each level's own number, since the scene's order means nothing.
    public Levels(LevelDefinition[] levels)
    {
        this.levels = levels;
        Array.Sort(this.levels, (first, second) => first.LevelNumber.CompareTo(second.LevelNumber));

        for (int i = 0; i < this.levels.Length; i++)
        {
            if (i > 0 && this.levels[i].LevelNumber == this.levels[i - 1].LevelNumber)
                GameLog.Warning(LogCategory.Game, "Two levels share number " + this.levels[i].LevelNumber + ", their order is arbitrary");

            // Whichever root is already switched on counts as current, so asking before a level is
            // entered still answers.
            if (currentIndex < 0 && this.levels[i].gameObject.activeInHierarchy)
                currentIndex = i;
        }
    }

    public LevelDefinition Current
    {
        get { return currentIndex >= 0 && currentIndex < levels.Length ? levels[currentIndex] : null; }
    }

    public bool EnterFirst()
    {
        return Enter(0);
    }

    public bool EnterNext()
    {
        return Enter(currentIndex + 1);
    }

    private bool Enter(int index)
    {
        if (index < 0 || index >= levels.Length)
            return false;

        GameLog.Info(LogCategory.Game, "Level started: " + levels[index].name);

        for (int i = 0; i < levels.Length; i++)
            levels[i].gameObject.SetActive(i == index);

        currentIndex = index;
        return true;
    }
}
