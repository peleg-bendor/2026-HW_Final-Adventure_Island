using Zenject;

// The composition root: the one place that decides which implementation satisfies which interface.
// Deliberately empty of bindings - Stage 2 only needs to prove the container builds and runs, and
// the first real binding arrives with the state model in stage 8.
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        GameLog.Info(LogCategory.Game, "Zenject container built - no bindings yet");
    }
}
