using UnityEngine;

// What one mount looks like and how wide it makes the player, as one asset each. A ScriptableObject
// rather than a builder, since composing a mount is data and stays editable without a recompile.
[CreateAssetMenu(fileName = "Mount", menuName = "Mount/Mount Definition")]
public class MountDefinition : ScriptableObject
{
    [SerializeField] private Sprite idle;

    // Shown in order while he walks, however many there are.
    [SerializeField] private Sprite[] walking;

    // One frame for the whole of a jump, since the mount art has no separate rise and fall.
    [SerializeField] private Sprite jumping;

    // How wide the player's capsule is while riding this one, in units. His height never changes.
    [SerializeField, Min(0.1f)] private float bodyWidth = 1.5f;

    public Sprite Idle { get { return idle; } }

    public Sprite[] Walking { get { return walking; } }

    public Sprite Jumping { get { return jumping; } }

    public float BodyWidth { get { return bodyWidth; } }
}
