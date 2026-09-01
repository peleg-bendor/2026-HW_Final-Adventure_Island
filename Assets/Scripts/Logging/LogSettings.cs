using UnityEngine;

// Holds one log level per category and pushes them into GameLog when the level loads. A
// MonoBehaviour rather than a ScriptableObject: GameLog is static and would have to reach an asset
// through Resources.Load, and this project deliberately has no Resources folder.
[DefaultExecutionOrder(-100)]
public class LogSettings : MonoBehaviour
{
    // Named rows rather than a bare LogLevel array, so the Inspector says which category each
    // value belongs to instead of numbering them.
    [System.Serializable]
    public class CategoryLevel
    {
        public LogCategory category;
        public LogLevel level;
    }

    [SerializeField] private CategoryLevel[] levels;

    public CategoryLevel[] Levels { get { return levels; } }

    // Beats the Awake calls that log, by way of this class's own execution order - Unity's order
    // across objects is arbitrary. A category left out of the list keeps GameLog's default.
    private void Awake()
    {
        Apply();
    }

    // An Inspector edit during Play has to reach GameLog's own copy of the levels, or moving a
    // dropdown mid-session does nothing at all. Editor-only, which is where that editing happens.
    private void OnValidate()
    {
        Apply();
    }

    // Public so the Tools > Logs window can push a change while the game is running, rather than
    // holding a second copy of the settings that a build would never read.
    public void Apply()
    {
        if (levels == null)
            return;

        foreach (CategoryLevel entry in levels)
        {
            if (entry != null)
                GameLog.SetLevel(entry.category, entry.level);
        }
    }

    // Fills in every category the moment the component is added, so the list is never a partial
    // one somebody has to finish by hand.
    private void Reset()
    {
        System.Array categories = System.Enum.GetValues(typeof(LogCategory));
        levels = new CategoryLevel[categories.Length];

        for (int i = 0; i < categories.Length; i++)
        {
            levels[i] = new CategoryLevel
            {
                category = (LogCategory)categories.GetValue(i),
                level = LogLevel.Info
            };
        }
    }
}
