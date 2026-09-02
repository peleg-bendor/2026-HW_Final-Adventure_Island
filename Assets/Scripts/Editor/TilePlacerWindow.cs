using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor window for editing the level one cell at a time by clicking, alongside the level window's
// all-at-once build: stamping a tile into a cell, or clearing whatever a cell holds. It places only
// what the tile prefab map already knows about, so everything it creates can be written back out as
// level data.
public class TilePlacerWindow : EditorWindow
{
    private enum Mode { Off, Place, Erase }

    [SerializeField] private TilePrefabMap tilePrefabMap;
    [SerializeField] private GameObject levelParent;
    [SerializeField] private int selectedIndex;

    // Defaulted rather than left empty, so a window opened for the first time already points at
    // the level this project builds first.
    [SerializeField] private string levelParentPath = "Level_1";

    // Off after every restart, and deliberately not serialized: while a mode is active the Scene
    // view stops selecting on click, and reopening Unity one click away from deleting a tile is
    // worse than one click away from placing one.
    private Mode mode;

    // One press-drag-release, collapsed into a single undo entry and reported as one line. None of
    // it is serialized either, since a stroke cannot outlive the drag that opened it.
    private bool painting;
    private Vector3 paintedCell;
    private int strokeUndoGroup;
    private int strokePlaced;
    private int strokeErased;
    private string strokePrefabName;

    [MenuItem("Tools/Tile Placer")]
    public static void ShowWindow()
    {
        GetWindow<TilePlacerWindow>("Tile Placer");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        tilePrefabMap = EditorGUILayout.ObjectField("Tile Prefabs", tilePrefabMap, typeof(TilePrefabMap), false) as TilePrefabMap;
        levelParent = EditorGUILayout.ObjectField("Parent", levelParent, typeof(GameObject), true) as GameObject;
        levelParent = SceneObjectMemory.Resolve(levelParent, ref levelParentPath);

        EditorGUILayout.Space();

        List<TilePrefabMap.Entry> entries = UsableEntries();
        if (entries.Count == 0 || levelParent == null)
        {
            mode = Mode.Off;
            EditorGUILayout.HelpBox("Assign a parent and a tile prefab map with at least one prefab in it.", MessageType.Info);
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, entries.Count - 1);

        string[] names = new string[entries.Count];
        for (int i = 0; i < entries.Count; i++)
            names[i] = entries[i].tileId + " - " + entries[i].prefab.name;

        // Disabled rather than hidden while erasing, which doesn't read it. Leaving it live would
        // suggest the choice still changes something.
        using (new EditorGUI.DisabledScope(mode == Mode.Erase))
        {
            selectedIndex = EditorGUILayout.Popup("Tile", selectedIndex, names);
        }

        mode = (Mode)EditorGUILayout.EnumPopup("Mode", mode);

        EditorGUILayout.HelpBox(ModeHelp(), MessageType.None);
    }

    private string ModeHelp()
    {
        if (mode == Mode.Place)
            return "Click or drag in the Scene view to place. Clicking won't select anything while this is on.";

        if (mode == Mode.Erase)
            return "Click or drag in the Scene view to delete everything in those cells.";

        return "Off, so the Scene view behaves normally.";
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (mode == Mode.Off || levelParent == null)
            return;

        List<TilePrefabMap.Entry> entries = UsableEntries();
        if (entries.Count == 0)
            return;

        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        // Claims the Scene view's default click handling, so placing a tile doesn't also select
        // whatever happened to be under the cursor.
        HandleUtility.AddDefaultControl(controlId);

        GameObject prefab = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)].prefab;
        Event current = Event.current;

        // Closed before the cell is worked out, so a button released over a view that can't
        // answer still ends the stroke rather than leaving it open.
        if (painting && current.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
            EndStroke();
            current.Use();
            return;
        }

        if (!TryGetCell(current.mousePosition, out Vector3 cell))
            return;

        // The only place the active mode shows up while looking at the scene rather than the
        // window, which is where the cursor already is when a click is about to happen.
        Handles.color = mode == Mode.Erase ? Color.red : Color.yellow;
        Handles.DrawWireCube(levelParent.transform.TransformPoint(cell), Vector3.one);

        if (current.type == EventType.MouseMove)
            sceneView.Repaint();

        // Alt is Unity's own camera modifier, so a click holding it is someone orbiting rather
        // than someone placing a tile.
        if (current.type == EventType.MouseDown && current.button == 0 && !current.alt)
        {
            GUIUtility.hotControl = controlId;
            BeginStroke();
            Paint(prefab, cell);
            current.Use();
        }
        else if (current.type == EventType.MouseDrag && painting && cell != paintedCell)
        {
            Paint(prefab, cell);
            current.Use();
        }
    }

    private void BeginStroke()
    {
        // A stroke left open, by a button released outside the Scene view, is closed here rather
        // than tracked, so the next click starts from a clean undo group.
        if (painting)
            EndStroke();

        painting = true;
        strokeUndoGroup = Undo.GetCurrentGroup();
        strokePlaced = 0;
        strokeErased = 0;
    }

    private void EndStroke()
    {
        painting = false;

        // Collapsed per stroke, so one Ctrl+Z takes back a whole dragged run of ground rather
        // than one press per cell of it.
        Undo.SetCurrentGroupName(mode == Mode.Erase ? "Erase Tiles" : "Place Tiles");
        Undo.CollapseUndoOperations(strokeUndoGroup);

        // Reported per stroke for the same reason, and silent when a drag changed nothing.
        if (strokePlaced > 0)
            Debug.Log("Placed " + strokePlaced + " x " + strokePrefabName);

        if (strokeErased > 0)
            Debug.Log("Erased " + strokeErased + " object(s)");
    }

    private void Paint(GameObject prefab, Vector3 cell)
    {
        paintedCell = cell;

        if (mode == Mode.Erase)
        {
            strokeErased += ClearCell(cell);
            return;
        }

        if (Place(prefab, cell))
        {
            strokePrefabName = prefab.name;
            strokePlaced++;
        }
    }

    private List<TilePrefabMap.Entry> UsableEntries()
    {
        List<TilePrefabMap.Entry> usable = new List<TilePrefabMap.Entry>();
        if (tilePrefabMap == null || tilePrefabMap.Entries == null)
            return usable;

        foreach (TilePrefabMap.Entry entry in tilePrefabMap.Entries)
        {
            if (entry != null && entry.prefab != null)
                usable.Add(entry);
        }

        return usable;
    }

    private bool TryGetCell(Vector2 mousePosition, out Vector3 cell)
    {
        cell = Vector3.zero;

        // The level is flat on z = 0, so a cell is wherever the cursor's ray crosses that plane.
        // A Scene view rotated to look along it has no answer, hence the failure case.
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane levelPlane = new Plane(Vector3.forward, Vector3.zero);
        if (!levelPlane.Raycast(ray, out float distance))
            return false;

        // Rounded in the parent's own space, since that's the space tiles are placed and stored
        // in. Rounding in world space instead would offset every placement by however far the
        // parent sits from the origin.
        Vector3 local = levelParent.transform.InverseTransformPoint(ray.GetPoint(distance));
        cell = new Vector3(Mathf.Round(local.x), Mathf.Round(local.y), 0f);
        return true;
    }

    private bool Place(GameObject prefab, Vector3 cell)
    {
        // Left alone when the cell already holds this prefab, so dragging back across a painted
        // run doesn't destroy and rebuild what is already right.
        Transform occupant = FindInCell(cell);
        if (occupant != null && PrefabUtility.GetCorrespondingObjectFromSource(occupant.gameObject) == prefab)
            return false;

        ClearCell(cell);

        GameObject tile = PrefabUtility.InstantiatePrefab(prefab, levelParent.transform) as GameObject;
        if (tile == null)
        {
            Debug.LogWarning("Could not place " + prefab.name);
            return false;
        }

        tile.transform.localPosition = cell;

        // Instantiated through PrefabUtility rather than plain Instantiate, so the tile keeps a
        // link to its prefab - which is the only way saving can work out what tile id it is.
        Undo.RegisterCreatedObjectUndo(tile, "Place Tile");
        return true;
    }

    private Transform FindInCell(Vector3 cell)
    {
        Transform parent = levelParent.transform;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (IsInCell(child, cell))
                return child;
        }

        return null;
    }

    private int ClearCell(Vector3 cell)
    {
        Transform parent = levelParent.transform;

        // Counted so a stroke can stay quiet about cells that held nothing.
        int removed = 0;

        // A cell holds one tile, since that's all the level file can store. Painting over
        // something replaces it rather than leaving two objects stacked in the same place.
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (IsInCell(child, cell))
            {
                Undo.DestroyObjectImmediate(child.gameObject);
                removed++;
            }
        }

        return removed;
    }

    private static bool IsInCell(Transform child, Vector3 cell)
    {
        return Mathf.RoundToInt(child.localPosition.x) == Mathf.RoundToInt(cell.x)
            && Mathf.RoundToInt(child.localPosition.y) == Mathf.RoundToInt(cell.y);
    }
}
