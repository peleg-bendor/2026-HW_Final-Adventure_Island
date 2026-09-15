using UnityEngine;
using Zenject;

// Puts the player back at the current level's start whenever a reset runs, and stops him there. His
// own IResettable rather than something the flow does to him, which keeps a Transform out of the flow.
// His facing, a shove and a buffered jump are reset by the components that hold them.
public class PlayerReset : MonoBehaviour, IResettable
{
    private IResetRegistry registry;
    private ILevels levels;
    private Rigidbody2D rigid;

    [Inject]
    public void Construct(IResetRegistry registry, ILevels levels)
    {
        this.registry = registry;
        this.levels = levels;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will keep his velocity across a reset");
    }

    private void OnEnable()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Player, "No IResetRegistry injected, the player will not return to the start");
            return;
        }

        registry.Register(this);
    }

    private void OnDisable()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    // The same for both scopes: losing a strike returns him to the current level's start, and
    // starting a level puts him there too.
    public void ResetTo(ResetScope scope)
    {
        PlayerStart start = levels != null && levels.Current != null ? levels.Current.PlayerStart : null;

        if (start == null)
        {
            GameLog.Warning(LogCategory.Player, "No PlayerStart to return to, the player stays where he is");
            return;
        }

        transform.position = start.transform.position;

        // Cleared, or he arrives at the start still carrying the fall that killed him.
        if (rigid != null)
            rigid.linearVelocity = Vector2.zero;
    }
}
