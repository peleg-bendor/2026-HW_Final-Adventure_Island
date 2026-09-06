using UnityEngine;
using Zenject;

// Restores the player whenever a reset runs: position, facing, velocity and buffered input. His own
// IResettable rather than something the flow does to him, which keeps a Transform out of the flow.
public class PlayerReset : MonoBehaviour, IResettable
{
    private IResetRegistry registry;
    private ILevels levels;
    private Rigidbody2D rigid;
    private PlayerMovement movement;
    private PlayerJump jump;

    [Inject]
    public void Construct(IResetRegistry registry, ILevels levels)
    {
        this.registry = registry;
        this.levels = levels;
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();

        if (rigid == null)
            GameLog.Warning(LogCategory.Player, "No Rigidbody2D found, the player will keep his velocity across a reset");

        if (movement == null)
            GameLog.Warning(LogCategory.Player, "No PlayerMovement found, the player will keep his facing across a reset");

        if (jump == null)
            GameLog.Warning(LogCategory.Player, "No PlayerJump found, a jump pressed under a popup will fire after the restart");
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
        // Before the start is looked up, since clearing his input is worth doing even when there
        // is nowhere to put him.
        if (jump != null)
            jump.ClearInput();

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

        if (movement != null)
            movement.Face(start.FacesRight);
    }
}
