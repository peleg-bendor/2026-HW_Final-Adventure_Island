// What a display is allowed to know about the session. Separate from SessionState so a counter can
// read the numbers without being able to spend a strike or restart the game.
public interface ISessionState
{
    int StrikesRemaining { get; }
    int FruitCount { get; }
}
