using UnityEngine;
using Zenject;

// The composition root: the one place that decides which implementation satisfies which interface,
// and where the game's tunable numbers enter the object graph.
public class GameInstaller : MonoInstaller
{
    // How many פסילות a game starts with.
    [SerializeField] private int startingStrikes = 3;

    public override void InstallBindings()
    {
        Container.Bind<SessionState>().AsSingle().WithArguments(startingStrikes);
        Container.Bind(typeof(IGameFlow), typeof(IResetRegistry)).To<GameFlow>().AsSingle();

        GameLog.Info(LogCategory.Game, "Zenject container built");
    }
}
