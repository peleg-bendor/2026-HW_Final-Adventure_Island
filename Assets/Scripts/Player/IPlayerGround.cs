// Whether the player is standing on something. The jump and the animators depend on this rather than
// on PlayerGround, which would hand them its transform and its enabled flag.
public interface IPlayerGround
{
    bool IsGrounded();
}
