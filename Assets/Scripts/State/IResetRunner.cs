// Runs a reset over everything registered. Split from IResetRegistry so a collectible can register
// itself without also being able to reset the level.
public interface IResetRunner
{
    void ResetAll(ResetScope scope);
}
