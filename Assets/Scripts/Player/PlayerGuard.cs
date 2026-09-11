using UnityEngine;
using Zenject;

// The player's protections in one place. Riding a mount and holding a fairy are the two things
// that absorb a contact, and no hazard or enemy holds either rule itself.
public class PlayerGuard : MonoBehaviour, IPlayerGuard
{
// How long the thing that took the mount is ignored afterwards, in seconds. Only that one
// thing: anything else touched inside the same window costs what it always would.
    [SerializeField] private float absorbGraceSeconds = 0.5f;

    private IMountSlot mount;
    private IPlayerFairy fairy;

    private IDestructible lastAbsorbed;
    private float ignoreLastUntil;

    [Inject]
    public void Construct(IMountSlot mount, IPlayerFairy fairy)
    {
        this.mount = mount;
        this.fairy = fairy;
    }

    public bool TryAbsorb(IDestructible source)
    {
        // Asked before the mount, since a fairy costs nothing and a mount is spent on the hit. It
        // says nothing to the log: what it destroys announces itself.
        if (fairy != null && fairy.Active)
        {
            if (source != null)
                source.TryDestroy(Destroyer.Fairy);

            return true;
        }

        // He is left standing inside anything that refused to be destroyed, and the ghost is the
        // one thing that does. Without this it takes the mount and then a strike a frame later.
        if (source != null && source == lastAbsorbed && Time.time < ignoreLastUntil)
            return true;

        if (mount == null || mount.Current == null)
            return false;

        // Destroyed before the mount goes: clearing it resizes his capsule inside this very trigger
        // callback, and Unity re-issues enter events for whatever is still overlapping.
        bool destroyed = source != null && source.TryDestroy(Destroyer.Riding);

        lastAbsorbed = source;
        ignoreLastUntil = Time.time + absorbGraceSeconds;

        // Said before the mount is cleared, so the log reads in the order it happened rather than
        // announcing the loss and then explaining it.
        GameLog.Info(LogCategory.Mount, destroyed
            ? "Hit absorbed - the mount and what hit it are both gone"
            : "Hit absorbed - the mount is gone and what hit it is still standing");

        mount.Clear();
        return true;
    }
}
