using System;
using Zenject;

// The one mount the player is riding. Not an IResettable: a new game and a level change are the same
// reset scope, and a mount is lost on one and kept across the other.
public class MountSlot : IMountSlot, IInitializable
{
    private readonly IGameFlow flow;

    private MountDefinition current;

    public MountSlot(IGameFlow flow)
    {
        this.flow = flow;
    }

    public MountDefinition Current { get { return current; } }

    public event Action Changed;

    // Run from the scene kernel's Start, at execution order -9997, so both events are subscribed
    // before anything at the default order can start a game.
    public void Initialize()
    {
        flow.GameStarted += Clear;
        flow.StrikeLost += Clear;
    }

    public void Take(MountDefinition mount)
    {
        if (mount == null)
            return;

        current = mount;
        GameLog.Info(LogCategory.Mount, "Mounted: " + mount.name);
        Changed?.Invoke();
    }

    public void Clear()
    {
        if (current == null)
            return;

        current = null;
        GameLog.Info(LogCategory.Mount, "Mount lost");
        Changed?.Invoke();
    }
}
