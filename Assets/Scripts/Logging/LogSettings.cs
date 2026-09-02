using UnityEngine;

// Holds one log level per category and pushes them into GameLog. A MonoBehaviour rather than a
// ScriptableObject, since GameLog is static and this project has no Resources folder to reach.
[DefaultExecutionOrder(-100)]
public class LogSettings : MonoBehaviour
{
    // Named rows rather than a bare LogLevel array, so the Inspector says which category is which.
    [System.Serializable]
    public class CategoryLevel
    {
        public LogCategory category;
        public LogLevel level;
    }

    [SerializeField] private CategoryLevel[] levels;

    public CategoryLevel[] Levels { get { return levels; } }

    // Beats the Awake calls that log, by way of this class's own execution order.
    private void Awake()
    {
        Apply();
    }

    // An Inspector edit during Play has to reach GameLog's own copy, or the dropdown does nothing.
    private void OnValidate()
    {
        Apply();
    }

    // Public so the Tools > Logs window can push a change while the game is running.
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

    // Fills in every category the moment the component is added, so the list is never half-written.
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
