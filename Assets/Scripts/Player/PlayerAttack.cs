using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// The attack key. What it throws is whatever the slot is holding, so this class never learns which
// weapons exist.
public class PlayerAttack : MonoBehaviour
{
    // How far in front of his middle a throw leaves, in units. X mirrors with his facing.
    [SerializeField] private Vector2 throwOffset = new Vector2(0.6f, 0f);

    private IProjectilePool pool;
    private IWeaponSlot slot;
    private IMountAttack mount;
    private Player player;

    [Inject]
    public void Construct(IProjectilePool pool, IWeaponSlot slot, IMountAttack mount, Player player)
    {
        this.pool = pool;
        this.slot = slot;
        this.mount = mount;
        this.player = player;
    }

    private void Awake()
    {
        if (player == null)
            GameLog.Warning(LogCategory.Player, "No Player injected, throws will leave from his feet");

        if (mount == null)
            GameLog.Warning(LogCategory.Player, "No IMountAttack injected, the key will throw a weapon even while riding");
    }

    private void Update()
    {
        if (Keyboard.current == null || Keyboard.current.zKey.wasPressedThisFrame == false)
            return;

        // The mount answers first, so a weapon is kept while riding and usable again the moment the
        // mount is gone.
        if (mount != null && mount.TryAttack())
            return;

        if (pool == null || slot == null)
        {
            GameLog.Warning(LogCategory.Player, "No IProjectilePool or IWeaponSlot injected, nothing is thrown");
            return;
        }

        if (slot.Held == null)
        {
            GameLog.Info(LogCategory.Player, "Attack ignored - no weapon held");
            return;
        }

        // Null when every copy is already in the air, which the pool has already said.
        BaseProjectile projectile = pool.Get(slot.Held);

        if (projectile == null)
            return;

        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 middle = player != null ? player.Middle : (Vector2)transform.position;
        Vector2 origin = middle + new Vector2(throwOffset.x * direction, throwOffset.y);
        projectile.Launch(origin, direction);
    }
}
