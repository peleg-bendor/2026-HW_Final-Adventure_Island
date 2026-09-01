using UnityEngine;

// The one way game code writes to the log. Static rather than an extension method or a per-class
// logger field: there is nothing per-object about logging, and the alternative would mean a new
// field in every file. Editor tooling deliberately doesn't use this and keeps plain Debug.
public static class GameLog
{
    // Info by default rather than silent, because LogSettings pushes its own values in Awake and
    // Unity's Awake order across objects is arbitrary. An unconfigured category says everything
    // rather than swallowing whatever happened to run first.
    private static readonly LogLevel[] levels = CreateDefaultLevels();

    public static void SetLevel(LogCategory category, LogLevel level)
    {
        levels[(int)category] = level;
    }

    public static LogLevel GetLevel(LogCategory category)
    {
        return levels[(int)category];
    }

    // For lines that fire on a timer or on contact and stop being informative once they have been
    // seen working. Off at the default Info level, which is the point of the tier.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Verbose(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Verbose)
            Debug.Log(Format(category, message));
    }

    // Conditional strips the call and its arguments from a release build, so the concatenation at
    // the call site costs nothing there either. Nothing may rely on a side effect in an argument.
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Info(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Info)
            Debug.Log(Format(category, message));
    }

    // Kept in a release build, unlike Info and Verbose: a shipped game should still say when
    // something is wrong with it.
    public static void Warning(LogCategory category, string message)
    {
        if (levels[(int)category] >= LogLevel.Warning)
            Debug.LogWarning(Format(category, message));
    }

    // No level check at all. Error is the floor every category is measured down to, so there is
    // no setting anywhere that can hide one.
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
