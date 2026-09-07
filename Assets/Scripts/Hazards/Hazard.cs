using UnityEngine;
using Zenject;

// Something dangerous to touch that can also be destroyed: detect the player, let his guard absorb
// the contact, then apply the effect. Spikes are deliberately not one of these.
public abstract class Hazard : MonoBehaviour, IDestructible, IResettable
{
    private IPlayerGuard guard;
    private IResetRegistry registry;
    private int lastTouchFrame = -1;

    [Inject]
    private void Construct(IPlayerGuard guard, IResetRegistry registry)
    {
        this.guard = guard;
        this.registry = registry;
    }

    // What is allowed to destroy this hazard. One line per subclass, and it is the whole rule.
    protected abstract Destroyer DestroyedBy { get; }

    // What touching this costs the player, once his guard has declined to absorb it.
    protected abstract void Hurt();

    // Registered in Awake and released on destroy, like a collectible: a destroyed hazard switches
    // itself off, so unregistering on disable would drop what the reset has to bring back.
    private void Awake()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Hazard, "No IResetRegistry injected, " + name + " will not come back after a strike");
            return;
        }

        registry.Register(this);
    }

    private void OnDestroy()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Touch(other);
    }

    // Standing inside a hazard keeps hurting, so a player pinned against a wall by a shove is not
    // safe there. Whether the repeat costs anything is each subclass's own rule.
    private void OnTriggerStay2D(Collider2D other)
    {
        Touch(other);
    }

    // Applied once a frame at most, since 2D sends Enter and Stay together on the step a contact
    // begins and a fire would otherwise cost two strikes for one touch.
    private void Touch(Collider2D other)
    {
        if (lastTouchFrame == Time.frameCount)
            return;

        if (other.GetComponent<Player>() == null)
            return;

        if (guard != null && guard.TryAbsorb(this))
            return;

        lastTouchFrame = Time.frameCount;
        Hurt();
    }

    // Nothing throws anything yet, so this has no caller until the boomerang exists.
    public bool TryDestroy(Destroyer by)
    {
        if ((DestroyedBy & by) == 0)
            return false;

        gameObject.SetActive(false);
        GameLog.Info(LogCategory.Hazard, name + " destroyed - " + by);
        return true;
    }

    // Everything except enemies comes back, on a strike as well as on a level start.
    public void ResetTo(ResetScope scope)
    {
        gameObject.SetActive(true);
    }
}
