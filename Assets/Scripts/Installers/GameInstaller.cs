using UnityEngine;
using Zenject;

// The composition root: the one place that decides which implementation satisfies which interface,
// and where the numbers describing the game's rules enter the object graph.
public class GameInstaller : MonoInstaller
{
    // How many strikes a game starts with.
    [SerializeField] private int startingStrikes = 3;

    // How many units the power bar holds. What a level opens with is authored on the level.
    [SerializeField] private int powerCapacity = 16;

    // How long one unit of power lasts, in seconds.
    [SerializeField] private float drainSeconds = 3f;

    public override void InstallBindings()
    {
        Container.Bind<SessionState>().AsSingle().WithArguments(startingStrikes);
        Container.Bind(typeof(IGameFlow), typeof(IResetRegistry)).To<GameFlow>().AsSingle();

        Container.Bind<IPowerModel>().To<PowerModel>().AsSingle().WithArguments(powerCapacity);
        Container.Bind<IPowerView>().To<PowerView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PowerController>().AsSingle().WithArguments(drainSeconds);

        GameLog.Info(LogCategory.Game, "Zenject container built");
    }
}
