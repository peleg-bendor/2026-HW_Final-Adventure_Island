using UnityEngine;

// The one way game code writes to the log. Static rather than a per-class logger field, since
// there is nothing per-object about logging. Editor tooling keeps plain Debug instead.
public static class GameLog
{
    // Info by default rather than silent, since LogSettings may not have pushed its values yet.
    private static readonly LogLevel[] levels = CreateDefaultLevels();

    public static void SetLevel(LogCategory category, LogLevel level)
    {
        levels[(int)category] = level;
    }

    public static LogLevel GetLevel(LogCategory category)
    {
        return levels[(int)category];
    }

    // For lines that fire on a timer or on contact. Off at the default Info level.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Verbose(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Verbose)
            Debug.Log(Format(category, message));
    }

    // Conditional strips the call and its arguments from a release build, side effects included.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Info(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Info)
            Debug.Log(Format(category, message));
    }

    // Kept in a release build, unlike Info and Verbose: a shipped game should still report faults.
    public static void Warning(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Warning)
            Debug.LogWarning(Format(category, message));
    }

    // No level check: Error is the floor every category is measured down to, so nothing hides one.
    public static void Error(LogCategory category, string message)
    {
        Debug.LogError(Format(category, message));
    }

    private static string Format(LogCategory category, string message)
    {
        return "[" + category + "] " + message;
    }

    private static LogLevel[] CreateDefaultLevels()
    {
        LogLevel[] created = new LogLevel[System.Enum.GetValues(typeof(LogCategory)).Length];

        for (int i = 0; i < created.Length; i++)
            created[i] = LogLevel.Info;

        return created;
    }
}
