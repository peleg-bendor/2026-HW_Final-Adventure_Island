using UnityEditor;
using UnityEngine;

// Editor window for the per-category log levels, so filtering the Console is a couple of clicks
// rather than finding a GameObject first. A view onto the LogSettings component in the open scene
// rather than a second copy of the settings, so a build reads the same values this edits.
public class LogsWindow : EditorWindow
{
    [MenuItem("Tools/Logs")]
    public static void ShowWindow()
    {
        GetWindow<LogsWindow>("Logs");
    }

    // Repainted on a timer rather than only on interaction, so a level changed from the Inspector
    // while this window is open doesn't leave the two showing different answers.
    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnGUI()
    {
        LogSettings settings = FindAnyObjectByType<LogSettings>();

        if (settings == null)
        {
            EditorGUILayout.HelpBox("No LogSettings in the open scene. Every category logs at Info until one exists.", MessageType.Info);
            return;
        }

        LogSettings.CategoryLevel[] levels = settings.Levels;

        if (levels == null || levels.Length == 0)
        {
            EditorGUILayout.HelpBox("LogSettings holds no categories. Use Reset on the component to fill them in.", MessageType.Warning);
            return;
        }

        EditorGUILayout.LabelField("Errors are never hidden, whatever a category is set to.", EditorStyles.wordWrappedMiniLabel);
        EditorGUILayout.Space();

        foreach (LogSettings.CategoryLevel entry in levels)
        {
            if (entry == null)
                continue;

            EditorGUI.BeginChangeCheck();
            LogLevel chosen = (LogLevel)EditorGUILayout.EnumPopup(entry.category.ToString(), entry.level);

            if (EditorGUI.EndChangeCheck())
            {
                // Recorded and marked dirty by hand, because this edits a component from outside
                // the Inspector, which is what normally does both.
                Undo.RecordObject(settings, "Change log level");
                entry.level = chosen;
                EditorUtility.SetDirty(settings);
                settings.Apply();
            }
        }
    }
}
