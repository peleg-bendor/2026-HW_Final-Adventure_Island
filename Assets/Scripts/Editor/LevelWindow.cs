using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Editor window that moves a level between its data file and the open scene, in both directions. It
// knows no tile ids of its own, and leaves the file format to LevelFile and the scene to LevelScene.
public class LevelWindow : EditorWindow
{
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private TilePrefabMap tilePrefabMap;
    [SerializeField] private GameObject levelParent;

    [SerializeField] private string levelParentPath = "Level_1";

    [MenuItem("Tools/Level")]
    public static void ShowWindow()
    {
        GetWindow<LevelWindow>("Level");
    }

    private void OnGUI()
    {
        levelFile = EditorGUILayout.ObjectField("Level File", levelFile, typeof(TextAsset), false) as TextAsset;
        tilePrefabMap = EditorGUILayout.ObjectField("Tile Prefabs", tilePrefabMap, typeof(TilePrefabMap), false) as TilePrefabMap;
        levelParent = EditorGUILayout.ObjectField("Parent", levelParent, typeof(GameObject), true) as GameObject;
        levelParent = SceneObjectMemory.Resolve(levelParent, ref levelParentPath);

        EditorGUILayout.Space();

        // Disabled rather than hidden, so absent buttons don't read as a broken tool.
        bool ready = levelFile != null && tilePrefabMap != null && levelParent != null;
        using (new EditorGUI.DisabledScope(!ready))
        {
            EditorGUILayout.HelpBox("Build makes Parent match the level file. Save overwrites the level file.", MessageType.Warning);

            if (GUILayout.Button("Build Level"))
                Build();

            if (GUILayout.Button("Save Level"))
                Save();
        }
    }

    // Reconciled rather than emptied first, so a correct cell keeps what was set on it by hand.
    private void Build()
    {
        LevelMap map = LevelFile.Read(levelFile.text);
        if (map == null || map.layers == null)
        {
            Debug.LogError("Not a level file this tool can read: " + levelFile.name);
            return;
        }

        if (map.width <= 0 || map.height <= 0)
        {
            Debug.LogError("Level file reports no usable size (" + map.width + "x" + map.height + "): " + levelFile.name);
            return;
        }

        int undoGroup = Undo.GetCurrentGroup();

        HashSet<int> unmapped = new HashSet<int>();
        Dictionary<Vector2Int, GameObject> wanted = PrefabsByCell(map, unmapped);

        LevelScene.Reconcile(levelParent.transform, wanted, out int kept, out int added, out int removed);

        Undo.SetCurrentGroupName("Build Level");
        Undo.CollapseUndoOperations(undoGroup);

        if (unmapped.Count > 0)
            Debug.LogWarning("Tile ids skipped, nothing mapped to them: " + string.Join(", ", unmapped));

        Debug.Log("Level built from " + levelFile.name + " under " + levelParent.name
            + " - " + kept + " kept, " + added + " added, " + removed + " removed");
    }

    // The file's ids turned into prefabs. An id the map doesn't know is reported rather than placed.
    private Dictionary<Vector2Int, GameObject> PrefabsByCell(LevelMap map, HashSet<int> unmapped)
    {
        Dictionary<Vector2Int, GameObject> cells = new Dictionary<Vector2Int, GameObject>();

        foreach (KeyValuePair<Vector2Int, int> cell in LevelFile.Cells(map))
        {
            GameObject prefab = tilePrefabMap.GetPrefab(cell.Value);
            if (prefab == null)
            {
                unmapped.Add(cell.Value);
                continue;
            }

            cells[cell.Key] = prefab;
        }

        return cells;
    }

    private void Save()
    {
        string path = AssetDatabase.GetAssetPath(levelFile);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("The level file has no path on disk, so there is nothing to save to.");
            return;
        }

        List<string> unmapped = new List<string>();
        List<string> snapped = new List<string>();
        List<string> negative = new List<string>();
        List<PlacedTile> tiles = LevelScene.Collect(levelParent.transform, tilePrefabMap, unmapped, snapped, negative);

        List<string> shared = new List<string>();
        LevelMap packed = LevelFile.Pack(LevelFile.Read(levelFile.text), tiles, shared);

        File.WriteAllText(path, LevelFile.Write(packed));

        // Without this Unity keeps serving the copy it imported earlier.
        AssetDatabase.ImportAsset(path);

        ReportSaveWarnings(unmapped, snapped, negative, shared);
        Debug.Log("Level saved to " + path + " - " + tiles.Count + " objects, grid " + packed.width + "x" + packed.height);
    }

    private static void ReportSaveWarnings(List<string> unmapped, List<string> snapped, List<string> negative, List<string> shared)
    {
        // Its own warning: these look fine now, and the next build is what deletes them.
        if (unmapped.Count > 0)
            Debug.LogWarning("Left out of the level file, so a rebuild will delete them: " + string.Join(", ", unmapped));

        if (snapped.Count > 0)
            Debug.LogWarning("Moved to the nearest cell: " + string.Join(", ", snapped));

        if (negative.Count > 0)
            Debug.LogWarning("Skipped, a level file can't store negative coordinates: " + string.Join(", ", negative));

        if (shared.Count > 0)
            Debug.LogWarning("Sharing a cell, so only the last of each was kept: " + string.Join(", ", shared));
    }
}
