using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// The attack key. What it throws is whatever the slot is holding, so this class never learns which
// weapons exist.
public class PlayerAttack : MonoBehaviour
{
    // How far in front of his middle a throw leaves, in units. X mirrors with his facing.
    [SerializeField] private Vector2 throwOffset = new Vector2(0.6f, 0f);

    private ProjectileDirector director;
    private IWeaponSlot slot;
    private Player player;
    private PlayerMountAttack mount;

    [Inject]
    public void Construct(ProjectileDirector director, IWeaponSlot slot)
    {
        this.director = director;
        this.slot = slot;
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        mount = GetComponent<PlayerMountAttack>();

        if (player == null)
            GameLog.Warning(LogCategory.Player, "No Player found, throws will leave from his feet");

        if (mount == null)
            GameLog.Warning(LogCategory.Player, "No PlayerMountAttack found, the key will throw a weapon even while riding");
    }

    private void Update()
    {
        if (Keyboard.current == null || Keyboard.current.zKey.wasPressedThisFrame == false)
            return;

        // The mount answers first, so a weapon is kept while riding and usable again the moment the
        // mount is gone.
        if (mount != null && mount.TryAttack())
            return;

        if (director == null || slot == null)
        {
            GameLog.Warning(LogCategory.Player, "No ProjectileDirector or IWeaponSlot injected, nothing is thrown");
            return;
        }

        if (slot.Held == null)
        {
            GameLog.Info(LogCategory.Player, "Attack ignored - no weapon held");
            return;
        }

        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 middle = player != null ? player.Middle : (Vector2)transform.position;
        Vector2 origin = middle + new Vector2(throwOffset.x * direction, throwOffset.y);
        director.Throw(slot.Held, origin, direction);
    }
}
