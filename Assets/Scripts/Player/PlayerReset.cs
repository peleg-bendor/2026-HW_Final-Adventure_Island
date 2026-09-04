using UnityEngine;
using Zenject;

// Puts the player back at the current level's start whenever a reset runs. His own IResettable
// rather than something the flow does to him, which is what keeps a Transform out of the flow.
public class PlayerReset : MonoBehaviour, IResettable
{
    private IResetRegistry registry;
    private IGameFlow flow;
    private Rigidbody2D rigid;
    private PlayerMovement movement;

    [Inject]
    public void Construct(IResetRegistry registry, IGameFlow flow)
    {
        this.registry = registry;
        this.flow = flow;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will keep his velocity across a reset");

        if (movement == null)
            GameLog.Warning(LogCategory.Player, "No PlayerMovement found, the player will keep his facing across a reset");
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

    // The same for both scopes: losing a פסילה returns him to the current level's start, and
    // starting a level puts him there too.
    public void ResetTo(ResetScope scope)
    {
        PlayerStart start = flow != null && flow.CurrentLevel != null ? flow.CurrentLevel.PlayerStart : null;

        if (start == null)
        {
            GameLog.Warning(LogCategory.Player, "No PlayerStart to return to, the player stays where he is");
            return;
        }

        transform.position = start.transform.position;

        // Cleared, or he arrives at the start still carrying the fall that killed him.
        if (rigid != null)
            rigid.linearVelocity = Vector2.zero;

        if (movement != null)
            movement.Face(start.FacesRight);
    }
}
