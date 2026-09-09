using UnityEngine;
using Zenject;

// The mount's attack: how long it runs, and putting the hit into the world for that long. Which key
// starts it is PlayerAttack's, which asks this before it throws anything.
public class PlayerMountAttack : MonoBehaviour
{
    private IMountSlot slot;
    private ProjectileDirector director;
    private ProjectilePrefabs prefabs;
    private MountStrike strike;

    private float attackUntil = float.NegativeInfinity;
    private float attackSeconds;

    [Inject]
    public void Construct(IMountSlot slot, ProjectileDirector director, ProjectilePrefabs prefabs)
    {
        this.slot = slot;
        this.director = director;
        this.prefabs = prefabs;
    }

    // True while an attack is running, which both picks the attack frames and refuses a second press.
    public bool IsAttacking { get { return Time.time < attackUntil; } }

    // How far through it, from 0 to 1, so the frames run once over its own length rather than off
    // the shared clock.
    public float AttackProgress
    {
        get { return IsAttacking ? 1f - (attackUntil - Time.time) / attackSeconds : 1f; }
    }

    private void Awake()
    {
        strike = GetComponentInChildren<MountStrike>(true);

        if (strike == null)
            GameLog.Warning(LogCategory.Mount, "No MountStrike found under the player, a mount attack will hit nothing");
    }

    // Answers whether it took the key, so a held weapon is thrown only when he is on foot.
    public bool TryAttack()
    {
        MountDefinition mount = slot != null ? slot.Current : null;

        if (mount == null)
            return false;

        if (IsAttacking)
        {
            GameLog.Verbose(LogCategory.Mount, "Attack ignored - the last one is still running");
            return true;
        }

        attackSeconds = mount.AttackSeconds;
        attackUntil = Time.time + attackSeconds;

        if (strike != null)
            strike.Begin(mount.StrikeSprite, mount.StrikeOffset, mount.StrikeSize);

        if (mount.SpitsFire)
            SpitFire(mount);

        GameLog.Verbose(LogCategory.Mount, "Mount attacked: " + mount.name);
        return true;
    }

    // The prefab comes from the installer rather than the definition asset, since the pool is keyed
    // on the reference and a second copy of it could point somewhere else.
    private void SpitFire(MountDefinition mount)
    {
        if (director == null || prefabs == null || prefabs.mountFire == null)
        {
            GameLog.Warning(LogCategory.Mount, "No ProjectileDirector injected or no mount fire prefab assigned, the red mount spits nothing");
            return;
        }

        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)transform.position +
                         new Vector2(mount.StrikeOffset.x * direction, mount.StrikeOffset.y);

        director.Throw(prefabs.mountFire, origin, direction);
    }

    // The hit goes when its time is up, and the moment he is off the mount - losing it to a contact
    // happens inside a physics callback, mid-swing.
    private void Update()
    {
        if (attackUntil == float.NegativeInfinity)
            return;

        if (IsAttacking && slot != null && slot.Current != null)
            return;

        attackUntil = float.NegativeInfinity;

        if (strike != null)
            strike.End();
    }
}
