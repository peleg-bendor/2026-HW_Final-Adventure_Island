using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// A level root in the open scene, one object per cell: finding, placing, clearing and gathering them,
// each change recorded for undo. Both level tools go through here, so they agree on what a cell is.
public static class LevelScene
{
    // Rounded rather than truncated, so a tile nudged off its cell by hand still counts as in it.
    public static Vector2Int CellOf(Transform child)
    {
        Vector3 position = child.localPosition;
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public static Transform FindInCell(Transform parent, Vector2Int cell)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (CellOf(child) == cell)
                return child;
        }

        return null;
    }

    // Every child keyed by its cell. Two objects in one cell can't both be written out, so the later
    // ones go into duplicates.
    public static Dictionary<Vector2Int, GameObject> IndexByCell(Transform parent, List<GameObject> duplicates)
    {
        Dictionary<Vector2Int, GameObject> byCell = new Dictionary<Vector2Int, GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject child = parent.GetChild(i).gameObject;
            Vector2Int cell = CellOf(child.transform);

            if (byCell.ContainsKey(cell))
                duplicates.Add(child);
            else
                byCell.Add(cell, child);
        }

        return byCell;
    }

    // PrefabUtility rather than Instantiate, so the tile keeps the prefab link that gives it its id.
    public static GameObject Place(Transform parent, GameObject prefab, Vector2Int cell, string undoName)
    {
        GameObject tile = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        if (tile == null)
        {
            Debug.LogWarning("Could not place " + prefab.name);
            return null;
        }

        tile.transform.localPosition = new Vector3(cell.x, cell.y, 0f);

        // Varied here rather than at Play, so what the Scene view shows is what runs.
        PickVariant(tile);

        Undo.RegisterCreatedObjectUndo(tile, undoName);
        return tile;
    }

    // A cell holds one tile, all the file can store, so clearing it removes everything there.
    public static int ClearCell(Transform parent, Vector2Int cell)
    {
        int removed = 0;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (CellOf(child) == cell)
            {
                Undo.DestroyObjectImmediate(child.gameObject);
                removed++;
            }
        }

        return removed;
    }

    // Answers with whether the tile had variants at all, so a stroke over plain ground reports nothing.
    public static bool PickVariant(GameObject tile)
    {
        SpriteVariant variant = tile.GetComponent<SpriteVariant>();

        if (variant == null)
            return false;

        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

        if (renderer != null)
            Undo.RecordObject(renderer, "Vary Tile");

        variant.PickOne();
        return true;
    }

    // Makes the children match the wanted cells. A cell already holding the right prefab keeps what was
    // set on it by hand, a wrong one is replaced, and anything not wanted is removed.
    public static void Reconcile(Transform parent, Dictionary<Vector2Int, GameObject> wanted,
        out int kept, out int added, out int removed)
    {
        List<GameObject> doomed = new List<GameObject>();
        Dictionary<Vector2Int, GameObject> existing = IndexByCell(parent, doomed);

        kept = 0;
        added = 0;

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

            if (Place(parent, cell.Value, cell.Key, "Build Level") != null)
                added++;
        }

        // Destroyed after placing, so a replaced cell's old tile can't be taken for the new one.
        foreach (GameObject leftover in existing.Values)
            doomed.Add(leftover);

        foreach (GameObject gone in doomed)
            Undo.DestroyObjectImmediate(gone);

        removed = doomed.Count;
    }

    // Everything under the root that the tile map can name, as the file would store it.
    public static List<PlacedTile> Collect(Transform parent, TilePrefabMap tilePrefabMap,
        List<string> unmapped, List<string> snapped, List<string> negative)
    {
        List<PlacedTile> tiles = new List<PlacedTile>();

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
            Vector2Int cell = CellOf(child.transform);

            if (!Mathf.Approximately(position.x, cell.x) || !Mathf.Approximately(position.y, cell.y))
                snapped.Add(child.name + " " + position);

            if (cell.x < 0 || cell.y < 0)
            {
                negative.Add(child.name + " " + position);
                continue;
            }

            tiles.Add(new PlacedTile { x = cell.x, y = cell.y, tileId = tileId, name = child.name });
        }

        return tiles;
    }
}
