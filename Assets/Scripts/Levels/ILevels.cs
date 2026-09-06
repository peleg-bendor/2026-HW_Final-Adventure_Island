// Which level is on, and how to move to the next. Split from IGameFlow so the camera, the player
// and the power controller can ask where they are without being able to end the game.
public interface ILevels
{
    LevelDefinition Current { get; }

    // Both answer with whether a level was actually entered, so a caller needs no count of its own.
    bool EnterFirst();
    bool EnterNext();
}
