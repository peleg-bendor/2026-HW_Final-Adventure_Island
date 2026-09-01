using System.IO;
using UnityEngine;

// Writes a Play session's log to a file beside the project, so reading a run stops meaning copying
// the Console out by hand. Subscribes in OnEnable and unsubscribes in OnDisable, so a scene reload
// can't leave a second copy of this listening to the same messages.
[DefaultExecutionOrder(-100)]
public class LogFileWriter : MonoBehaviour
{
    [SerializeField] private string fileName = "GameLog.txt";

    // Off by default: a stack trace under every ordinary line makes this file harder to read than
    // the Console it saves a copy of. Switched on when a run needs tracing back to call sites.
    [SerializeField] private bool traceEveryLine;

    // Truncated once per Play session and appended to afterwards. The static is reset explicitly
    // through RuntimeInitializeOnLoadMethod below rather than left to a scene reload to clear it,
    // which is what makes it survive this project having no scene reloads at all: nothing here
    // reloads on death or on game over, so a static that only a reload would reset would keep its
    // value for the whole session.
    private static bool fileStarted;

    private StreamWriter writer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void StartNewSession()
    {
        fileStarted = false;
    }

    private void OnEnable()
    {
        string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, fileName);

        try
        {
            writer = new StreamWriter(path, fileStarted);

            // Flushed per line, so stopping Play or crashing still leaves everything up to that
            // point in the file.
            writer.AutoFlush = true;
        }
        // Broad on purpose. A read-only folder throws UnauthorizedAccessException rather than an
        // IOException, and a log sink that can't open its file should go quiet, not take the game
        // down on the way past.
        catch (System.Exception ex)
        {
            writer = null;
            GameLog.Warning(LogCategory.Game, "Could not open " + fileName + ", this session will not be written to a file (" + ex.Message + ")");
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

        // Only the kinds worth tracing back to a line of code. A stack trace under every ordinary
        // line would make this file harder to read than the Console it saves a copy of.
        if (traceEveryLine || type != LogType.Log)
            writer.WriteLine(stackTrace.TrimEnd());
    }
}
