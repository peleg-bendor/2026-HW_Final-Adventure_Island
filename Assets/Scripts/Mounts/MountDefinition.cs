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

    // Shown in order once over the length of an attack.
    [SerializeField] private Sprite[] attacking;

    // How wide the player's capsule is while riding this one, in units. His height never changes.
    [SerializeField, Min(0.1f)] private float bodyWidth = 1.5f;

    // How long one attack lasts, in seconds. It is its own cooldown as well.
    [SerializeField, Min(0.05f)] private float attackSeconds = 0.25f;

    // What the hit looks like. Empty for a mount whose own frames already show it.
    [SerializeField] private Sprite strikeSprite;

    // Where its attack happens, from his transform, which is half a unit above his feet: the middle
    // of the hit, or the mouth the fire leaves from. X mirrors with his facing.
    [SerializeField] private Vector2 strikeOffset;

    // How big the hit is, in units. Zero for a mount whose attack is not a hit at all.
    [SerializeField] private Vector2 strikeSize;

    // Whether its attack leaves its mouth. Only the red one does; the other two reach as far as
    // their hit and no further.
    [SerializeField] private bool spitsFire;

    public Sprite Idle { get { return idle; } }

    public Sprite[] Walking { get { return walking; } }

    public Sprite Jumping { get { return jumping; } }

    public Sprite[] Attacking { get { return attacking; } }

    public float BodyWidth { get { return bodyWidth; } }

    public float AttackSeconds { get { return attackSeconds; } }

    public Sprite StrikeSprite { get { return strikeSprite; } }

    public Vector2 StrikeOffset { get { return strikeOffset; } }

    public Vector2 StrikeSize { get { return strikeSize; } }

    public bool SpitsFire { get { return spitsFire; } }
}
