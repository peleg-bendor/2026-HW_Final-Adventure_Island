using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Editor window that moves a level between its data file and the open scene, in both directions.
// It knows no tile ids of its own - the tile prefab map holds those - so supporting a new tile
// type never means changing this file.
public class LevelWindow : EditorWindow
{
    [SerializeField] private TextAsset levelFile;
    [SerializeField] private TilePrefabMap tilePrefabMap;
    [SerializeField] private GameObject levelParent;

    // Defaulted rather than left empty, so a window opened for the first time already points at
    // the level this project builds first.
    [SerializeField] private string levelParentPath = "Level_1";

    // Gathered from the scene before anything is written, so the grid can be sized to fit what
    // was actually found rather than assumed from the file it came from.
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

        // Disabled rather than hidden while something is missing, so the buttons' absence is
        // never mistaken for the tool being broken.
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

    // Reconciles the scene against the file rather than emptying it first. A cell already holding
    // the right prefab is left alone, so everything set on that instance by hand survives a
    // rebuild - which is the only place a bird's dip or a ביצה's contents is stored.
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

        // One undo group around the whole rebuild, so undoing a mistaken build is a single
        // Ctrl+Z instead of one per object destroyed and created.
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

        // Whatever the file never named. Destroyed after the placing rather than before it, so a
        // replaced cell's old occupant can't be confused for the new one while both are alive.
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

            // Stopped at the grid's own size rather than the array's, so a hand-edited file with
            // a trailing entry places nothing at a negative row instead.
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

                // Rows are stored downwards from the top of the file while Unity's Y axis points
                // upwards, so the file's first row belongs at the highest Y in the scene.
                cells[new Vector2Int(i % map.width, (map.height - 1) - (i / map.width))] = prefab;
            }
        }

        return cells;
    }

    // Two objects in one cell can't both be written to the file, so the second and later ones go
    // straight in with what the rebuild is about to remove.
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

        // The file's own size is the floor, so deleting something at the edge can't quietly
        // shrink the level, while anything placed past it grows the grid to fit.
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

        // Without this Unity keeps serving the copy it imported earlier, and the next build
        // would read the level as it was before this save.
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

            // Everything the builder places keeps a link back to the prefab it came from, which
            // is what makes a tile id recoverable at all. Anything else can't be written out.
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
        // Worth a warning of its own rather than a line in a summary: these objects look fine in
        // the scene right now, and the next build is what deletes them.
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
            // A line break per grid row, so a changed cell shows up as a one-line diff and the
            // file stays readable enough to edit by hand.
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
