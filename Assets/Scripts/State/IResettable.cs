// Implemented by anything a level reset restores - a collectible, an enemy, the player, the כוח
// model. The reset walks the registered list and never learns which kinds exist.
public interface IResettable
{
    void ResetTo(ResetScope scope);
}
