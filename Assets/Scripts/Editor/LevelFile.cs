using System.Collections.Generic;
using System.Text;
using UnityEngine;

// The level file's format: Tiled's map JSON, one tile id per cell, rows running down from the top.
// Knows nothing about the scene, and nothing about which prefab an id stands for.
public static class LevelFile
{
    // Every save writes a single layer under this name, as Tiled would.
    private const string LayerName = "Tile Layer 1";

    // Null when the text is not a level file at all.
    public static LevelMap Read(string json)
    {
        return JsonUtility.FromJson<LevelMap>(json);
    }

    // Every non-empty cell the file names, in the order the file names them.
    public static List<KeyValuePair<Vector2Int, int>> Cells(LevelMap map)
    {
        List<KeyValuePair<Vector2Int, int>> cells = new List<KeyValuePair<Vector2Int, int>>();
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

                // File rows run downwards, Unity's Y upwards, so row 0 sits at the highest Y.
                Vector2Int cell = new Vector2Int(i % map.width, (map.height - 1) - (i / map.width));
                cells.Add(new KeyValuePair<Vector2Int, int>(cell, tileId));
            }
        }

        return cells;
    }

    // Lays the tiles into one grid. Two tiles in one cell can't both be stored, so the last is kept and
    // the others are named in shared.
    public static LevelMap Pack(LevelMap existing, List<PlacedTile> tiles, List<string> shared)
    {
        // The file's size is the floor, so deleting at the edge can't shrink the level.
        int width = existing != null ? Mathf.Max(existing.width, 1) : 1;
        int height = existing != null ? Mathf.Max(existing.height, 1) : 1;

        foreach (PlacedTile tile in tiles)
        {
            width = Mathf.Max(width, tile.x + 1);
            height = Mathf.Max(height, tile.y + 1);
        }

        int[] data = new int[width * height];

        foreach (PlacedTile tile in tiles)
        {
            int index = (height - 1 - tile.y) * width + tile.x;

            if (data[index] != 0)
                shared.Add(tile.name + " (" + tile.x + ", " + tile.y + ")");

            data[index] = tile.tileId;
        }

        return new LevelMap
        {
            width = width,
            height = height,
            layers = new[] { new LevelMap.Layer { name = LayerName, data = data } }
        };
    }

    // Written by hand rather than with JsonUtility, which puts the whole grid on one line.
    public static string Write(LevelMap map)
    {
        int[] data = map.layers[0].data;
        StringBuilder json = new StringBuilder();
        json.Append("{\n");
        json.Append("  \"width\": ").Append(map.width).Append(",\n");
        json.Append("  \"height\": ").Append(map.height).Append(",\n");
        json.Append("  \"layers\": [\n    {\n      \"name\": \"").Append(LayerName).Append("\",\n      \"data\": [");

        for (int i = 0; i < data.Length; i++)
        {
            // A line break per grid row, so a changed cell shows up as a one-line diff.
            if (i % map.width == 0)
                json.Append("\n        ");

            json.Append(data[i]);
            if (i < data.Length - 1)
                json.Append(", ");
        }

        json.Append("\n      ]\n    }\n  ]\n}\n");
        return json.ToString();
    }
}
