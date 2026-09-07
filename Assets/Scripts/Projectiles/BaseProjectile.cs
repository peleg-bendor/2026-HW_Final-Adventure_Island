using UnityEngine;

// Anything in flight: launched with a velocity, travelling until it hits something or runs out of
// time, then switched off for the pool to hand out again. Where it came from lives outside it.
public abstract class BaseProjectile : MonoBehaviour
{
    private float speed;
    private float lift;
    private float maxSeconds;
    private float launchedAt;

    protected Rigidbody2D Body { get; private set; }

    protected virtual void Awake()
    {
        Body = GetComponent<Rigidbody2D>();

        if (Body == null)
            GameLog.Warning(LogCategory.Projectile, "No Rigidbody2D found on " + name + ", it will not fly");
    }

    // Called once by the builder, before this is ever handed out. The numbers come from a recipe
    // rather than from the prefab, since there is only ever one kind of each projectile.
    public void Configure(float speed, float lift, float gravityScale, float maxSeconds)
    {
        this.speed = speed;
        this.lift = lift;
        this.maxSeconds = maxSeconds;

        if (Body != null)
            Body.gravityScale = gravityScale;
    }

    // The fixed sequence, and the reason a pooled projectile is safe to reuse: whatever the last
    // flight left behind is cleared before this one starts.
    public void Launch(Vector2 origin, float direction)
    {
        if (Body == null)
            return;

        gameObject.SetActive(true);
        transform.position = origin;
        transform.localScale = new Vector3(direction, 1f, 1f);

        Body.angularVelocity = 0f;
        Body.linearVelocity = new Vector2(direction * speed, lift);
        launchedAt = Time.time;

        GameLog.Verbose(LogCategory.Projectile, name + " launched");
    }

    // The base owns Update so the timeout cannot be forgotten. Steering goes in Fly, because a
    // subclass declaring its own Update would hide this one and Unity would never call it.
    private void Update()
    {
        Fly();

        if (maxSeconds <= 0f || Time.time - launchedAt < maxSeconds)
            return;

        GameLog.Verbose(LogCategory.Projectile, name + " timed out");
        Despawn();
    }

    // Per-frame steering, for a projectile that does not simply follow its launch velocity.
    protected virtual void Fly()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnHit(other);
    }

    // What touching something means. No shared default: an axe, a boomerang and an enemy's fireball
    // want opposite things from the same contact.
    protected abstract void OnHit(Collider2D other);

    // Switched off rather than destroyed, which is the whole point of the pool. The pool finds it
    // again by looking for an inactive one.
    protected void Despawn()
    {
        gameObject.SetActive(false);
    }
}
