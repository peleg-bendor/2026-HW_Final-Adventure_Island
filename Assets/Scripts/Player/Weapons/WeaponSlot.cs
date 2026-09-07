using UnityEngine;
using Zenject;

// The one weapon the player carries. Not an IResettable: a new game and a level change are the same
// reset scope, and a weapon is lost on one and kept across the other.
public class WeaponSlot : IWeaponSlot, IInitializable
{
    private readonly IGameFlow flow;

    private GameObject held;

    public WeaponSlot(IGameFlow flow)
    {
        this.flow = flow;
    }

    public GameObject Held { get { return held; } }

    // Run from the scene kernel's Start, at execution order -9997, so both events are subscribed
    // before anything at the default order can start a game.
    public void Initialize()
    {
        flow.GameStarted += Clear;
        flow.StrikeLost += Clear;
    }

    public void Take(GameObject projectilePrefab)
    {
        if (projectilePrefab == null)
            return;

        held = projectilePrefab;
        GameLog.Info(LogCategory.Weapon, "Weapon taken: " + projectilePrefab.name);
    }

    private void Clear()
    {
        if (held == null)
            return;

        held = null;
        GameLog.Info(LogCategory.Weapon, "Weapon lost");
    }
}
