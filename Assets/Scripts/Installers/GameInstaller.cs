using Zenject;

// The composition root: the one place that decides which implementation satisfies which interface.
// Empty of bindings so far, because nothing in the game needs one yet.
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        GameLog.Info(LogCategory.Game, "Zenject container built - no bindings yet");
    }
}
