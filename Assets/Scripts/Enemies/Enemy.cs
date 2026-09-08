using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

// Anything in the level that hunts the player: it behaves while he is near, costs a strike on
// contact, answers what may destroy it, and brings itself back after a countdown. A subclass writes
// only what it does while alive - it declares no Awake and no Update, which the base owns.
public abstract class Enemy : MonoBehaviour, IDestructible, IResettable
{
    // How close the player has to be, in units, before this does anything. Zero is always active.
    [SerializeField, Min(0f)] private float activationRange = 12f;

    private IGameFlow flow;
    private IPlayerGuard guard;
    private IResetRegistry registry;
    private RespawnDelay delay;
    private Player player;

    private Vector2 home;
    private float feet;
    private bool destroyed;
    private int lastTouchFrame = -1;
    private CancellationTokenSource respawn;

    [Inject]
    private void Construct(IGameFlow flow, IPlayerGuard guard, IResetRegistry registry,
        RespawnDelay delay, Player player)
    {
        this.flow = flow;
        this.guard = guard;
        this.registry = registry;
        this.delay = delay;
        this.player = player;
    }

    // Where this was placed when the level was authored. Everything that puts it back puts it here.
    protected Vector2 Home { get { return home; } }

    // How far the pivot sits above the bottom of the collider, so anything that lands on a floor
    // lands on its feet. A constant of the prefab, measured once.
    protected float Feet { get { return feet; } }

    // The middle of his body rather than his transform, which sits at his feet.
    protected Vector2 PlayerPosition
    {
        get { return player != null ? player.Middle : (Vector2)transform.position; }
    }

    // Every sprite in this project is drawn facing right, so a positive scale is a rightward one.
    protected bool FacesRight { get { return transform.localScale.x > 0f; } }

    protected void Face(bool right)
    {
        transform.localScale = new Vector3(right ? 1f : -1f, 1f, 1f);
    }

    // Terrain is the only solid collider in this game: every hazard, pickup, door, projectile and
    // enemy is a trigger, and the player is the one solid thing that is not ground.
    protected static bool IsTerrain(Collider2D collider)
    {
        return collider != null && collider.isTrigger == false && collider.GetComponent<Player>() == null;
    }

    // What is allowed to destroy this enemy. One line per subclass, and it is the whole rule.
    protected abstract Destroyer DestroyedBy { get; }

    // What this one does while the player is near. The only step that makes a spider a spider.
    protected abstract void Behave();

    // True while part-way through something that should finish even if the player has moved out of
    // range - a swoop, a hop, a jump - so it is not left frozen halfway.
    protected virtual bool IsMidAction { get { return false; } }

    // Registered in Awake and released on destroy, like a collectible: a killed enemy switches
    // itself off, so unregistering on disable would drop what a level start has to bring back.
    private void Awake()
    {
        home = transform.position;

        Collider2D body = GetComponent<Collider2D>();

        if (body != null)
            feet = transform.position.y - body.bounds.min.y;
        else
            GameLog.Warning(LogCategory.Enemy, "No Collider2D found on " + name + ", it cannot be touched or destroyed");

        if (registry != null)
            registry.Register(this);
        else
            GameLog.Warning(LogCategory.Enemy, "No IResetRegistry injected on " + name + ", it will not come back when a level starts");

        if (player == null)
            GameLog.Warning(LogCategory.Enemy, "No Player injected on " + name + ", it will never activate");

        OnAwake();
    }

    // Where a subclass caches components or reads its own configuration once. The base owns Awake,
    // so a subclass cannot skip the registration by declaring one of its own.
    protected virtual void OnAwake()
    {
    }

    private void OnDestroy()
    {
        if (registry != null)
            registry.Unregister(this);

        CancelRespawn();
    }

    // The base owns Update so the activation range cannot be forgotten. Behaviour goes in Behave,
    // because a subclass declaring its own Update would hide this one and never be gated.
    private void Update()
    {
        if (IsPlayerNear() || IsMidAction)
            Behave();
    }

    // Nothing starts moving, shooting or chasing until he is close enough, which is what keeps a
    // bird from swooping at nobody.
    private bool IsPlayerNear()
    {
        if (activationRange <= 0f)
            return true;

        if (player == null)
            return false;

        return Vector2.Distance(transform.position, PlayerPosition) <= activationRange;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Touch(other);
    }

    // Standing inside an enemy keeps hurting, so a player pinned against a wall is not safe there.
    private void OnTriggerStay2D(Collider2D other)
    {
        Touch(other);
    }

    // Applied once a frame at most, since 2D sends Enter and Stay together on the step a contact
    // begins and one touch would otherwise cost two strikes.
    private void Touch(Collider2D other)
    {
        if (lastTouchFrame == Time.frameCount)
            return;

        if (other.GetComponent<Player>() == null)
            return;

        if (guard != null && guard.TryAbsorb(this))
            return;

        lastTouchFrame = Time.frameCount;

        if (flow == null)
        {
            GameLog.Warning(LogCategory.Enemy, "No IGameFlow injected on " + name + ", touching it costs nothing");
            return;
        }

        GameLog.Info(LogCategory.Enemy, name + " touched - a strike is owed");
        flow.LoseStrike();
    }

    // Refuses a second hit in the same frame, or two axes would start two countdowns on one enemy.
    public bool TryDestroy(Destroyer by)
    {
        if (destroyed || (DestroyedBy & by) == 0)
            return false;

        Die(by);
        return true;
    }

    // The fixed order, and a subclass has no say in it: gone, then the countdown that brings it back.
    private void Die(Destroyer by)
    {
        destroyed = true;
        gameObject.SetActive(false);
        GameLog.Info(LogCategory.Enemy, name + " destroyed - " + by);
        WaitAndReturn();
    }

    // A Task and not a coroutine: this object is switched off before the wait begins, and Unity
    // stops coroutines on a disabled GameObject.
    private async void WaitAndReturn()
    {
        if (delay == null)
        {
            GameLog.Warning(LogCategory.Enemy, "No RespawnDelay injected on " + name + ", it stays destroyed");
            return;
        }

        CancelRespawn();
        respawn = new CancellationTokenSource();

        float seconds = UnityEngine.Random.Range(delay.minSeconds, delay.maxSeconds);

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds), respawn.Token);
        }
        catch (OperationCanceledException)
        {
            // A level start brings every enemy back itself, so a pending countdown is abandoned.
            return;
        }
        catch (Exception error)
        {
            // async void rather than an unobserved Task, so a fault surfaces here instead of vanishing.
            GameLog.Error(LogCategory.Enemy, name + " respawn failed - " + error.Message);
            return;
        }

        destroyed = false;
        Spawn();
        GameLog.Info(LogCategory.Enemy, name + " back after " + seconds.ToString("0.0") + "s");
    }

    private void CancelRespawn()
    {
        if (respawn == null)
            return;

        respawn.Cancel();
        respawn.Dispose();
        respawn = null;
    }

    // Coming back is one thing that happens two ways, rather than two that have to agree: the
    // countdown and a level start both land here.
    private void Spawn()
    {
        transform.position = new Vector3(home.x, home.y, transform.position.z);
        gameObject.SetActive(true);
        OnSpawned();
    }

    // Where a subclass puts itself back into its starting state, for the same reason the base puts
    // it back at its position.
    protected virtual void OnSpawned()
    {
    }

    // A destroyed enemy stays destroyed through a strike and its own countdown is what brings it
    // back. A living one goes home, so a replayed stretch is not empty of everything that moved.
    public void ResetTo(ResetScope scope)
    {
        if (scope == ResetScope.Full)
        {
            CancelRespawn();
            destroyed = false;
        }
        else if (destroyed)
        {
            return;
        }

        Spawn();
    }
}
