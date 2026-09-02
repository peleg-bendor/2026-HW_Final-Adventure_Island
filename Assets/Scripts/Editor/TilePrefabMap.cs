using System;
using System.Collections.Generic;
using UnityEngine;

// Maps the tile ids in a level file to the prefabs they stand for. An asset rather than fields on
// a tool window, so a new tile type is a row added here instead of an edit to any tool.
[CreateAssetMenu(fileName = "TilePrefabMap", menuName = "Level/Tile Prefab Map")]
public class TilePrefabMap : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public int tileId;
        public GameObject prefab;
    }

    [SerializeField] private Entry[] entries;

    // Read-only, so the asset stays the only place deciding what the mapping holds.
    public IReadOnlyList<Entry> Entries { get { return entries; } }

    // An unmapped id returns null rather than raising: the caller is the one that reports it.
    public GameObject GetPrefab(int tileId)
    {
        if (entries == null)
            return null;

        foreach (Entry entry in entries)
        {
            if (entry != null && entry.tileId == tileId)
                return entry.prefab;
        }

        return null;
    }

    // The reverse of GetPrefab. Answers 0 for anything unmapped, so the caller reports it.
    public int GetTileId(GameObject prefab)
    {
        if (entries == null || prefab == null)
            return 0;

        foreach (Entry entry in entries)
        {
            if (entry != null && entry.prefab == prefab)
                return entry.tileId;
        }

        return 0;
    }
}
