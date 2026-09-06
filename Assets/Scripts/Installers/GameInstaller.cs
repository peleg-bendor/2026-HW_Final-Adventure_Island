using UnityEngine;
using Zenject;

// The composition root: the one place that decides which implementation satisfies which interface,
// and where the numbers describing the game's rules enter the object graph.
public class GameInstaller : MonoInstaller
{
    // How many strikes a game starts with.
    [SerializeField] private int startingStrikes = 3;

    // How much fruit costs a strike. Never zero, or every pickup divides by it.
    [SerializeField, Min(1)] private int fruitPerStrike = 20;

    // How many units the power bar holds. What a level opens with is authored on the level.
    [SerializeField] private int powerCapacity = 16;

    // How long one unit of power lasts, in seconds.
    [SerializeField] private float drainSeconds = 3f;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SessionState>().AsSingle().WithArguments(startingStrikes);
        Container.Bind<ILevels>().To<Levels>().AsSingle();
        Container.Bind(typeof(IResetRegistry), typeof(IResetRunner)).To<ResetRegistry>().AsSingle();
        Container.Bind<IPopups>().To<Popups>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IGameFlow>().To<GameFlow>().AsSingle().WithArguments(fruitPerStrike);

        Container.Bind<IPowerModel>().To<PowerModel>().AsSingle().WithArguments(powerCapacity);
        Container.Bind<IPowerView>().To<PowerView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PowerController>().AsSingle().WithArguments(drainSeconds);

        Container.Bind<IStrikesView>().To<StrikesView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesTo<StrikesController>().AsSingle();

        Container.Bind<IFruitView>().To<FruitView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesTo<FruitController>().AsSingle().WithArguments(fruitPerStrike);

        GameLog.Info(LogCategory.Game, "Fruit reached the container"); // remove me
        GameLog.Info(LogCategory.Game, "Zenject container built");
    }
}
