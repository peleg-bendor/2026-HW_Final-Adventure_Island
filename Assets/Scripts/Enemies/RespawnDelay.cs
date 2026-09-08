using System;

// How long a destroyed enemy stays gone. A rule of the game rather than a property of any one enemy,
// so it enters the object graph at the installer beside the strike count and the drain rate.
[Serializable]
public class RespawnDelay
{
    public float minSeconds = 10f;
    public float maxSeconds = 20f;
}
