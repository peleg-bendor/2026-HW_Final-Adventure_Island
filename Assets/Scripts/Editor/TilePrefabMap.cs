using System;
using System.Collections.Generic;
using UnityEngine;

// Maps the tile ids in a level file to the prefabs they stand for. An asset rather than fields on
// a tool window, so the mapping is version-controlled and survives a restart - and so a new tile
// type is a row added here instead of an edit to any tool that reads it.
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

    // For tools that offer a choice of tile rather than looking one up. Read-only, so the asset
    // stays the only place deciding what the mapping holds.
    public IReadOnlyList<Entry> Entries { get { return entries; } }

    // An unmapped id returns null rather than raising anything: a level file may legitimately
    // hold tiles this project doesn't place, and the caller is the one that reports them.
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

    // The reverse of GetPrefab, for writing a scene back out as level data. Answers 0 - the id
    // meaning an empty cell - for anything unmapped, so the caller can report it rather than
    // write a tile that would come back as something else.
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
