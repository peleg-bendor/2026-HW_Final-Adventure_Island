using System;

// The handful of fields this project reads out of a Tiled JSON export, shaped for JsonUtility.
// Deliberately not the whole format - every key left out here is ignored on parse, so the tool
// stays indifferent to Tiled's version and to map settings that don't affect placement.
[Serializable]
public class TiledMap
{
    public int width;
    public int height;
    public Layer[] layers;

    // Nested rather than its own file: a layer means nothing outside the map holding it, and
    // the schema is easier to check against the JSON when it reads in one piece.
    [Serializable]
    public class Layer
    {
        public string name;
        public int[] data;
    }
}
