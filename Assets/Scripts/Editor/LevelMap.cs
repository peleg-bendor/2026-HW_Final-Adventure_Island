using System;

// The fields a level file holds, shaped for JsonUtility. Deliberately still Tiled's map export
// minus every key this project doesn't read - levels are authored in the editor tools rather than
// in Tiled, and keeping its shape is what leaves that reversible.
[Serializable]
public class LevelMap
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
