using System.Collections.Generic;

// The list a reset walks, and the walk itself. Its own class rather than part of GameFlow, which
// was answering a question nobody asks the flow.
public class ResetRegistry : IResetRegistry, IResetRunner
{
    private readonly List<IResettable> resettables = new List<IResettable>();

    public void Register(IResettable resettable)
    {
        if (resettable != null && resettables.Contains(resettable) == false)
            resettables.Add(resettable);
    }

    public void Unregister(IResettable resettable)
    {
        resettables.Remove(resettable);
    }

    // Copied before walking, so a resettable that registers or unregisters while being reset cannot
    // change the list underneath the loop.
    public void ResetAll(ResetScope scope)
    {
        IResettable[] snapshot = resettables.ToArray();

        foreach (IResettable resettable in snapshot)
            resettable.ResetTo(scope);
    }
}
