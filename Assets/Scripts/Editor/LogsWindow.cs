using UnityEditor;
using UnityEngine;

// Editor window for the per-category log levels, so filtering the Console doesn't mean finding a
// GameObject first. A view onto the open scene's LogSettings rather than a second copy of it.
public class LogsWindow : EditorWindow
{
    [MenuItem("Tools/Logs")]
    public static void ShowWindow()
    {
        GetWindow<LogsWindow>("Logs");
    }

    // Repainted on a timer, so an Inspector edit doesn't leave the two views disagreeing.
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
                // Recorded and marked dirty by hand, edited from outside the Inspector.
                Undo.RecordObject(settings, "Change log level");
                entry.level = chosen;
                EditorUtility.SetDirty(settings);
                settings.Apply();
            }
        }
    }
}
