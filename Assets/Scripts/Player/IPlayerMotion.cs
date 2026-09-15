// Whether the player is walking, for whatever draws him. The animators depend on this rather than on
// PlayerMovement, which would let them shove him.
public interface IPlayerMotion
{
    bool IsWalking { get; }
}
