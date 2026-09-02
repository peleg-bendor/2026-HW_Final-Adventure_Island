using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Editor window that moves a level between its data file and the open scene, in both directions.
// It knows no tile ids of its own, so a new tile type never means changing this file.
public class LevelWindow : EditorWindow
{
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private TilePrefabMap tilePrefabMap;
    [SerializeField] private GameObject levelParent;

    [SerializeField] private string levelParentPath = "Level_1";

    // Gathered before anything is written, so the grid fits what was found, not the file's claim.
    private struct PlacedTile
    {
        public int x;
        public int y;
        public int tileId;
        public string name;
    }

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
        LevelMap map = JsonUtility.FromJson<LevelMap>(levelFile.text);
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
        Dictionary<Vector2Int, GameObject> wanted = ReadCells(map, unmapped);

        List<GameObject> doomed = new List<GameObject>();
        Dictionary<Vector2Int, GameObject> existing = IndexChildren(doomed);

        int kept = 0;
        int added = 0;

        foreach (KeyValuePair<Vector2Int, GameObject> cell in wanted)
        {
            if (existing.TryGetValue(cell.Key, out GameObject current))
            {
                existing.Remove(cell.Key);

                if (PrefabUtility.GetCorrespondingObjectFromSource(current) == cell.Value)
                {
                    kept++;
                    continue;
                }

                doomed.Add(current);
            }

            if (Place(cell.Value, cell.Key))
                added++;
        }

        // Destroyed after placing, so a replaced cell's old tile can't be taken for the new one.
        foreach (GameObject leftover in existing.Values)
            doomed.Add(leftover);

        foreach (GameObject gone in doomed)
            Undo.DestroyObjectImmediate(gone);

        Undo.SetCurrentGroupName("Build Level");
        Undo.CollapseUndoOperations(undoGroup);

        if (unmapped.Count > 0)
            Debug.LogWarning("Tile ids skipped, nothing mapped to them: " + string.Join(", ", unmapped));

        Debug.Log("Level built from " + levelFile.name + " under " + levelParent.name
            + " - " + kept + " kept, " + added + " added, " + doomed.Count + " removed");
    }

    private Dictionary<Vector2Int, GameObject> ReadCells(LevelMap map, HashSet<int> unmapped)
    {
        Dictionary<Vector2Int, GameObject> cells = new Dictionary<Vector2Int, GameObject>();
        int cellCount = map.width * map.height;

        foreach (LevelMap.Layer layer in map.layers)
        {
            if (layer == null || layer.data == null)
                continue;

            // Stopped at the grid's size, so a trailing entry in a hand-edited file places nothing.
            int last = Mathf.Min(layer.data.Length, cellCount);

            for (int i = 0; i < last; i++)
            {
                int tileId = layer.data[i];

                // Tiled writes 0 for an empty cell, the one id that means "nothing here".
                if (tileId == 0)
                    continue;

                GameObject prefab = tilePrefabMap.GetPrefab(tileId);
                if (prefab == null)
                {
                    unmapped.Add(tileId);
                    continue;
                }

                // File rows run downwards, Unity's Y upwards, so row 0 sits at the highest Y.
                cells[new Vector2Int(i % map.width, (map.height - 1) - (i / map.width))] = prefab;
            }
        }

        return cells;
    }

    // Two objects in one cell can't both be written out, so later ones go with the removals.
    private Dictionary<Vector2Int, GameObject> IndexChildren(List<GameObject> doomed)
    {
        Dictionary<Vector2Int, GameObject> byCell = new Dictionary<Vector2Int, GameObject>();
        Transform parent = levelParent.transform;

        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            Vector3 position = child.transform.localPosition;
            Vector2Int cell = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

            if (byCell.ContainsKey(cell))
                doomed.Add(child);
            else
                byCell.Add(cell, child);
        }

        return byCell;
    }

    private bool Place(GameObject prefab, Vector2Int cell)
    {
        GameObject tile = PrefabUtility.InstantiatePrefab(prefab, levelParent.transform) as GameObject;
        if (tile == null)
        {
            Debug.LogWarning("Could not instantiate " + prefab.name);
            return false;
        }

        tile.transform.localPosition = new Vector3(cell.x, cell.y, 0f);

        Undo.RegisterCreatedObjectUndo(tile, "Build Level");
        return true;
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
        List<PlacedTile> tiles = CollectTiles(unmapped, snapped, negative);

        // The file's size is the floor, so deleting at the edge can't shrink the level.
        LevelMap existing = JsonUtility.FromJson<LevelMap>(levelFile.text);
        int width = existing != null ? Mathf.Max(existing.width, 1) : 1;
        int height = existing != null ? Mathf.Max(existing.height, 1) : 1;
        foreach (PlacedTile tile in tiles)
        {
            width = Mathf.Max(width, tile.x + 1);
            height = Mathf.Max(height, tile.y + 1);
        }

        List<string> shared = new List<string>();
        int[] data = new int[width * height];
        foreach (PlacedTile tile in tiles)
        {
            int index = (height - 1 - tile.y) * width + tile.x;
            if (data[index] != 0)
                shared.Add(tile.name + " (" + tile.x + ", " + tile.y + ")");

            data[index] = tile.tileId;
        }

        File.WriteAllText(path, ToJson(width, height, data));

        // Without this Unity keeps serving the copy it imported earlier.
        AssetDatabase.ImportAsset(path);

        ReportSaveWarnings(unmapped, snapped, negative, shared);
        Debug.Log("Level saved to " + path + " - " + tiles.Count + " objects, grid " + width + "x" + height);
    }

    private List<PlacedTile> CollectTiles(List<string> unmapped, List<string> snapped, List<string> negative)
    {
        List<PlacedTile> tiles = new List<PlacedTile>();
        Transform parent = levelParent.transform;

        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;

            // Only a prefab link makes a tile id recoverable, so anything else is skipped.
            GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(child);
            int tileId = tilePrefabMap.GetTileId(source);
            if (tileId == 0)
            {
                unmapped.Add(child.name);
                continue;
            }

            Vector3 position = child.transform.localPosition;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            if (!Mathf.Approximately(position.x, x) || !Mathf.Approximately(position.y, y))
                snapped.Add(child.name + " " + position);

            if (x < 0 || y < 0)
            {
                negative.Add(child.name + " " + position);
                continue;
            }

            tiles.Add(new PlacedTile { x = x, y = y, tileId = tileId, name = child.name });
        }

        return tiles;
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

    private static string ToJson(int width, int height, int[] data)
    {
        StringBuilder json = new StringBuilder();
        json.Append("{\n");
        json.Append("  \"width\": ").Append(width).Append(",\n");
        json.Append("  \"height\": ").Append(height).Append(",\n");
        json.Append("  \"layers\": [\n    {\n      \"name\": \"Tile Layer 1\",\n      \"data\": [");

        for (int i = 0; i < data.Length; i++)
        {
            // A line break per grid row, so a changed cell shows up as a one-line diff.
            if (i % width == 0)
                json.Append("\n        ");

            json.Append(data[i]);
            if (i < data.Length - 1)
                json.Append(", ");
        }

        json.Append("\n      ]\n    }\n  ]\n}\n");
        return json.ToString();
    }
}
