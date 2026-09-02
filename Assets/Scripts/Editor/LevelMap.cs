using System;

// The fields a level file holds, shaped for JsonUtility. Tiled's map export minus every key this
// project doesn't read, kept to that shape so going back to Tiled stays possible.
[Serializable]
public class LevelMap
{
    public int width;
    public int height;
    public Layer[] layers;

    [Serializable]
    public class Layer
    {
        public string name;
        public int[] data;
    }
}
