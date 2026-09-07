// The player being pushed by something other than his own keys. A hazard depends on this rather
// than on PlayerMovement, which would hand it his facing, his transform and his enabled flag.
public interface IPlayerShove
{
    // True while a shove is still running. Only a rock asks, so a fire he is flung into still kills.
    bool IsShoving { get; }

    // Pushed the way he faces, with his own controls suspended for the duration.
    void Shove(float shoveSpeed, float seconds);
}
