using System.IO;
using UnityEngine;

// Writes a Play session's log to a file beside the project, so reading a run stops meaning
// copying the Console out by hand.
[DefaultExecutionOrder(-10000)]
public class LogFileWriter : MonoBehaviour
{
    [SerializeField] private string fileName = "GameLog.txt";

    // Off by default: a trace under every line makes this harder to read than the Console.
    [SerializeField] private bool traceEveryLine;

    // Truncated the first time a session opens the file and appended to after. Cleared at the start
    // of every Play, since a static can outlive one when domain reload is off.
    private static bool fileStarted;

    private StreamWriter writer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void StartNewSession()
    {
        fileStarted = false;
    }

    // Opened before SceneContext builds the container, by way of this class's own execution order, so
    // what is logged during the build reaches the file.
    private void OnEnable()
    {
        string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, fileName);

        try
        {
            writer = new StreamWriter(path, fileStarted);

            // Flushed per line, so a crash still leaves everything up to that point in the file.
            writer.AutoFlush = true;
        }
        // Broad on purpose: a read-only folder throws UnauthorizedAccessException, not IOException,
        // and a log sink that can't open its file should go quiet rather than stop the game.
        catch (System.Exception ex)
        {
            writer = null;
            GameLog.Warning(LogCategory.Game, "Could not open " + fileName + ", this session will not be written to a file - " + ex.Message);
            return;
        }

        fileStarted = true;
        Application.logMessageReceived += OnLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;

        if (writer != null)
        {
            writer.Dispose();
            writer = null;
        }
    }

    private void OnLogMessage(string message, string stackTrace, LogType type)
    {
        if (writer == null)
            return;

        writer.WriteLine(type + ": " + message);

        // Warnings and errors carry their trace whatever the setting says.
        if (traceEveryLine || type != LogType.Log)
            writer.WriteLine(stackTrace.TrimEnd());
    }
}
