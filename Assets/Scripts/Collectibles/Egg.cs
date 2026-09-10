using System.Collections;
using UnityEngine;
using Zenject;

// An egg standing in the level: touched, it cracks for a moment and is then replaced by whatever it
// was authored to hold. Not a Collectible, since that base disappears before it applies its effect.
public class Egg : MonoBehaviour, IResettable
{
    // What this egg holds. Set per instance when the level is authored, and never rolled.
    [SerializeField] private DropType drop = DropType.Heart;

    // How long the cracked frame shows before what it held appears, in seconds.
    [SerializeField, Min(0f)] private float crackSeconds = 0.5f;

    // The whole egg and the broken one. There is no third frame, so it goes rather than staying in
    // the level as an opened shell.
    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite cracked;

    private IDropFactory drops;
    private IResetRegistry registry;
    private SpriteRenderer body;
    private bool opened;

    [Inject]
    public void Construct(IDropFactory drops, IResetRegistry registry)
    {
        this.drops = drops;
        this.registry = registry;
    }

    // Registered in Awake and released on destroy, like a collectible: an opened egg switches itself
    // off, so unregistering on disable would drop the very object a reset has to bring back.
    private void Awake()
    {
        body = GetComponent<SpriteRenderer>();

        if (body == null)
            GameLog.Warning(LogCategory.Collectible, "No SpriteRenderer found on " + name + ", it cannot show that it opened");

        if (idle == null || cracked == null)
            GameLog.Warning(LogCategory.Collectible, "No idle or cracked sprite set on " + name + ", it opens without looking any different");

        if (drop == DropType.None)
            GameLog.Warning(LogCategory.Collectible, "No drop set on " + name + ", opening it gives nothing");

        if (registry != null)
            registry.Register(this);
        else
            GameLog.Warning(LogCategory.Collectible, "No IResetRegistry injected on " + name + ", it will not come back after a strike");
    }

    private void OnDestroy()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened || other.GetComponent<Player>() == null)
            return;

        opened = true;
        Show(cracked);
        GameLog.Info(LogCategory.Collectible, name + " opened - " + drop);
        StartCoroutine(Hatch());
    }

    // A coroutine and not a Task: this object stays alive for the whole wait, the wait cancels
    // nothing and returns nothing, and freezing with Time.timeScale is what a popup should do to it.
    private IEnumerator Hatch()
    {
        yield return new WaitForSeconds(crackSeconds);

        // Made before the egg switches itself off, since deactivating stops this coroutine.
        if (drops != null)
            drops.Create(drop, transform.position);
        else
            GameLog.Warning(LogCategory.Collectible, "No IDropFactory injected on " + name + ", it gives nothing");

        gameObject.SetActive(false);
    }

    private void Show(Sprite sprite)
    {
        if (body != null && sprite != null)
            body.sprite = sprite;
    }

    // Whole again on a strike as well as on a level start, since everything collectible comes back.
    // What it gave out is destroyed by the same reset, so nothing is handed out twice.
    public void ResetTo(ResetScope scope)
    {
        StopAllCoroutines();
        opened = false;
        Show(idle);
        gameObject.SetActive(true);
    }
}
