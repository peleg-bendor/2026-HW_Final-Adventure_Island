// The mount's attack as the rest of the player sees it: starting one, and how far through it is. The
// attack key and the mount's animator hold this rather than the component itself.
public interface IMountAttack
{
    bool TryAttack();

    bool IsAttacking { get; }

    float AttackProgress { get; }
}
