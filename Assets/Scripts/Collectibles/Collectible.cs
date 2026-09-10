using UnityEngine;
using Zenject;

// Anything picked up by touching it: detect the player, disappear, then apply an effect. Subclasses
// write only the effect, which is the one step that differs between the eight kinds.
public abstract class Collectible : MonoBehaviour, IResettable
{
    // Which drop type this prefab is, which is what lets a factory find it. None for anything only
    // ever placed by hand, which is both fruit.
    [SerializeField] private DropType drop = DropType.None;

    private IResetRegistry registry;
    private bool dropped;

    [Inject]
    private void Construct(IResetRegistry registry)
    {
        this.registry = registry;
    }

    public DropType Drop { get { return drop; } }

    // Registered in Awake and released on destroy, unlike the project's other resettables: a
    // collectible switches itself off when taken, so unregistering on disable would drop the very
    // object a reset has to bring back.
    private void Awake()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No IResetRegistry injected, " + name + " will not come back after a strike");
            return;
        }

        registry.Register(this);
    }

    private void OnDestroy()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    // Called by the factory on anything it spawns, which is what separates a produced pickup from
    // one the level was authored with.
    public void MarkDropped()
    {
        dropped = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;

        // Consumed before the effect is applied, since an effect can cost a strike and the reset
        // that follows has to be able to bring this one back with the rest.
        gameObject.SetActive(false);
        PickUp();
    }

    protected abstract void PickUp();

    // Everything the level was authored with comes back, on a strike as well as on a level start.
    // A dropped one goes instead, because whatever produced it comes back and would hand out a second.
    public void ResetTo(ResetScope scope)
    {
        if (dropped == false)
        {
            gameObject.SetActive(true);
            return;
        }

        // Switched off as well as destroyed, since Destroy waits for the end of the frame and its
        // trigger is live until then.
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
