using UnityEngine;

// A bat that hangs still while the player is looking at it and drifts at him the moment his back is
// turned. The one thing here that goes through walls, and the one only a fairy can destroy.
public class Ghost : Enemy
{
    // How fast it drifts, in units per second. Slower than he walks, so he can always leave.
    [SerializeField, Min(0f)] private float chaseSpeed = 2.5f;

    // Hanging still, and coming for him. Two states rather than two frames of one.
    [SerializeField] private Sprite frozenSprite;
    [SerializeField] private Sprite chasingSprite;

    private SpriteRenderer art;
    private bool chasing;

    // A fairy and nothing else. The one answer of the six that is not the long line, which is why
    // this property is abstract rather than inherited.
    protected override Destroyer DestroyedBy
    {
        get { return Destroyer.Fairy; }
    }

    protected override void OnAwake()
    {
        art = GetComponent<SpriteRenderer>();

        if (art == null)
            GameLog.Warning(LogCategory.Enemy, "No SpriteRenderer found on " + name + ", it will not change pose");
    }

    protected override void OnSpawned()
    {
        chasing = false;
        Show(frozenSprite);
    }

    protected override void Behave()
    {
        bool shouldChase = IsWatched() == false;

        if (shouldChase != chasing)
        {
            chasing = shouldChase;
            Show(chasing ? chasingSprite : frozenSprite);
            GameLog.Verbose(LogCategory.Enemy, name + (chasing ? " is coming" : " froze") + " - he is " +
                Vector2.Distance(transform.position, PlayerPosition).ToString("0.0") + " away");
        }

        if (chasing == false)
            return;

        // No sweep and no ground under it: terrain is the one thing that does not stop a ghost.
        Vector2 next = Vector2.MoveTowards(transform.position, PlayerPosition, chaseSpeed * Time.deltaTime);
        transform.position = new Vector3(next.x, next.y, transform.position.z);
    }

    // It is on the side he faces, so he is looking straight at it. Giving up past a distance is the
    // base's activation range and needs nothing here.
    private bool IsWatched()
    {
        return (transform.position.x > PlayerPosition.x) == PlayerFacesRight;
    }

    private void Show(Sprite sprite)
    {
        if (art != null && sprite != null)
            art.sprite = sprite;
    }
}
