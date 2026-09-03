// How a resettable joins the list a reset walks. Split from IGameFlow so an enemy or a collectible
// depends on registering itself and not on the operations that can end a game.
public interface IResetRegistry
{
    void Register(IResettable resettable);
    void Unregister(IResettable resettable);
}
