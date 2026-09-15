using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor window for painting the level one cell at a time, alongside the level window's all-at-once
// build. It places only what the tile prefab map knows about, so everything it creates can be written
// back out as level data.
public class TilePlacerWindow : EditorWindow
{
    private enum Mode { Off, Place, Erase }

    [SerializeField] private TilePrefabMap tilePrefabMap;
    [SerializeField] private GameObject levelParent;
    [SerializeField] private int selectedIndex;

    [SerializeField] private string levelParentPath = "Level_1";

    // Not serialized, so a mode is off after a restart; reopening one click from erasing is bad.
    private Mode mode;

    // One press-drag-release as one undo entry, unserialized since it can't outlive its drag.
    private bool painting;
    private Vector2Int paintedCell;
    private int strokeUndoGroup;
    private int strokePlaced;
    private int strokeVaried;
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

        // Disabled rather than hidden while erasing, which doesn't read it.
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

        // Claims the Scene view's click handling, so placing a tile doesn't also select.
        HandleUtility.AddDefaultControl(controlId);

        GameObject prefab = entries[Mathf.Clamp(selectedIndex, 0, entries.Count - 1)].prefab;
        Event current = Event.current;

        // Closed before the cell is worked out, so a release over a dead view still ends it.
        if (painting && current.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
            EndStroke();
            current.Use();
            return;
        }

        if (!TryGetCell(current.mousePosition, out Vector2Int cell))
            return;

        Handles.color = mode == Mode.Erase ? Color.red : Color.yellow;
        Handles.DrawWireCube(levelParent.transform.TransformPoint(new Vector3(cell.x, cell.y, 0f)), Vector3.one);

        if (current.type == EventType.MouseMove)
            sceneView.Repaint();

        // Alt is Unity's camera modifier, so a click holding it is orbiting rather than placing.
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
        // A stroke left open outside the Scene view is closed here, for a clean next undo group.
        if (painting)
            EndStroke();

        painting = true;
        strokeUndoGroup = Undo.GetCurrentGroup();
        strokePlaced = 0;
        strokeVaried = 0;
        strokeErased = 0;
    }

    private void EndStroke()
    {
        painting = false;

        // Collapsed per stroke, so one Ctrl+Z takes back a whole dragged run of ground.
        Undo.SetCurrentGroupName(mode == Mode.Erase ? "Erase Tiles" : "Place Tiles");
        Undo.CollapseUndoOperations(strokeUndoGroup);

        if (strokePlaced > 0)
            Debug.Log("Placed " + strokePlaced + " x " + strokePrefabName);

        if (strokeVaried > 0)
            Debug.Log("Varied " + strokeVaried + " x " + strokePrefabName);

        if (strokeErased > 0)
            Debug.Log("Erased " + strokeErased + " object(s)");
    }

    private void Paint(GameObject prefab, Vector2Int cell)
    {
        paintedCell = cell;

        if (mode == Mode.Erase)
        {
            strokeErased += LevelScene.ClearCell(levelParent.transform, cell);
            return;
        }

        strokePrefabName = prefab.name;

        if (Place(prefab, cell))
            strokePlaced++;
    }

    // A cell already holding this prefab is not rebuilt, so dragging back over it costs nothing. If the
    // prefab draws one of several sprites, painting it again rolls a new one.
    private bool Place(GameObject prefab, Vector2Int cell)
    {
        Transform occupant = LevelScene.FindInCell(levelParent.transform, cell);
        if (occupant != null && PrefabUtility.GetCorrespondingObjectFromSource(occupant.gameObject) == prefab)
        {
            if (LevelScene.PickVariant(occupant.gameObject))
                strokeVaried++;

            return false;
        }

        LevelScene.ClearCell(levelParent.transform, cell);
        return LevelScene.Place(levelParent.transform, prefab, cell, "Place Tile") != null;
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

    private bool TryGetCell(Vector2 mousePosition, out Vector2Int cell)
    {
        cell = Vector2Int.zero;

        // The level is flat on z = 0, so a cell is where the cursor's ray crosses that plane. A
        // view rotated to look along it has no answer, hence the failure case.
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        Plane levelPlane = new Plane(Vector3.forward, Vector3.zero);
        if (!levelPlane.Raycast(ray, out float distance))
            return false;

        // Rounded in the parent's space, not world, or placements shift by the parent's offset.
        Vector3 local = levelParent.transform.InverseTransformPoint(ray.GetPoint(distance));
        cell = new Vector2Int(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
        return true;
    }
}
