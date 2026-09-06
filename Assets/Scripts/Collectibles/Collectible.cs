using UnityEngine;
using Zenject;

// Anything picked up by touching it: detect the player, disappear, then apply an effect. Subclasses
// write only the effect, which is the one step that differs between the eight kinds.
public abstract class Collectible : MonoBehaviour, IResettable
{
    private IResetRegistry registry;

    [Inject]
    private void Construct(IResetRegistry registry)
    {
        this.registry = registry;
    }

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

    // Everything collectible comes back, on a strike as well as on a level start. Only enemies
    // stay dead.
    public void ResetTo(ResetScope scope)
    {
        gameObject.SetActive(true);
    }
}
