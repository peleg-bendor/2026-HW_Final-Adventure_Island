using System.Collections;
using UnityEngine;
using Zenject;

// The fairy the player is holding, and what is left of its ten seconds. An IResettable, unlike the
// weapon and mount slots: this one is lost on a strike and on a level change alike.
public class PlayerFairy : MonoBehaviour, IPlayerFairy, IResettable
{
    // How long one fairy lasts, in seconds.
    [SerializeField, Min(0f)] private float seconds = 10f;

    // Shown for as long as it lasts, so the effect can be seen and not only inferred. A child of the
    // player, so it follows him without anything here moving it.
    [SerializeField] private GameObject marker;

    private IResetRegistry registry;
    private bool active;

    [Inject]
    public void Construct(IResetRegistry registry)
    {
        this.registry = registry;
    }

    public bool Active { get { return active; } }

    private void Awake()
    {
        if (marker == null)
            GameLog.Warning(LogCategory.Player, "No fairy marker assigned, nothing on screen will show that one is held");

        Show(false);
    }

    // Registered on enable like the player's other resettable, since he is never switched off.
    private void OnEnable()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Player, "No IResetRegistry injected, a fairy will outlive the strike that should have cost it");
            return;
        }

        registry.Register(this);
    }

    private void OnDisable()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    public void Take()
    {
        StopAllCoroutines();
        active = true;
        Show(true);
        GameLog.Info(LogCategory.Player, "Fairy taken - " + seconds + "s");
        StartCoroutine(Hold());
    }

    // A coroutine and not a Task: this runs on the player, who is never switched off, it cancels
    // nothing and returns nothing, and stopping under a popup is what it should do.
    private IEnumerator Hold()
    {
        yield return new WaitForSeconds(seconds);

        active = false;
        Show(false);
        GameLog.Info(LogCategory.Player, "Fairy gone");
    }

    // Lost on a strike and on a level change alike, which is what makes this a resettable where the
    // weapon and mount slots are not.
    public void ResetTo(ResetScope scope)
    {
        StopAllCoroutines();

        if (active == false)
            return;

        active = false;
        Show(false);
        GameLog.Info(LogCategory.Player, "Fairy lost");
    }

    private void Show(bool visible)
    {
        if (marker != null)
            marker.SetActive(visible);
    }
}
