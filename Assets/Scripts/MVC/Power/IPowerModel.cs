// Owns the power count and the rules for changing it. No Unity types at all - the controller does
// everything the scene needs.
public interface IPowerModel
{
    int Current { get; }
    int Capacity { get; }

    // Both answer with how much actually moved, so fruit taken at full power and a hit taken at
    // empty are distinguishable from real changes without re-reading the count.
    int Add(int amount);
    int Remove(int amount);

    void SetTo(int amount);
}
