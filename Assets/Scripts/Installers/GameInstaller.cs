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

    // How long a destroyed enemy stays gone, the same for every enemy there is.
    [SerializeField] private RespawnDelay respawnDelay;

    // The prefabs the projectile builder makes copies of.
    [SerializeField] private ProjectilePrefabs projectilePrefabs;

    // Where the pooled projectiles are parked in the Hierarchy, so they do not litter the root.
    [SerializeField] private Transform projectileParent;

    // Every pickup a drop can turn out to be. Each prefab says which drop type it is, so a new one
    // is another entry here and no change to the factory.
    [SerializeField] private GameObject[] dropPrefabs;

    // What is left on screen when something is destroyed. Typed by the component each one carries,
    // so a prefab that could not play is refused here rather than failing at the first death.
    [SerializeField] private OneShotAnimator puffPrefab;
    [SerializeField] private FallingBody fallPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<SessionState>().AsSingle().WithArguments(startingStrikes);
        Container.Bind<ILevels>().To<Levels>().AsSingle();
        Container.Bind(typeof(IResetRegistry), typeof(IResetRunner)).To<ResetRegistry>().AsSingle();
        Container.Bind<IPopups>().To<Popups>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IPlayerGuard>().To<PlayerGuard>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IPlayerShove>().To<PlayerMovement>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IPlayerFairy>().To<PlayerFairy>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IGameFlow>().To<GameFlow>().AsSingle().WithArguments(fruitPerStrike);

        Container.Bind<IPowerModel>().To<PowerModel>().AsSingle().WithArguments(powerCapacity);
        Container.Bind<IPowerView>().To<PowerView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<PowerController>().AsSingle().WithArguments(drainSeconds);

        Container.Bind<IStrikesView>().To<StrikesView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesTo<StrikesController>().AsSingle();

        Container.Bind<IFruitView>().To<FruitView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesTo<FruitController>().AsSingle().WithArguments(fruitPerStrike);

        Container.Bind<RespawnDelay>().FromInstance(respawnDelay).AsSingle();

        Container.Bind<ProjectilePrefabs>().FromInstance(projectilePrefabs).AsSingle();
        Container.Bind<IProjectileBuilder>().To<ProjectileBuilder>().AsSingle().WithArguments(projectileParent);
        Container.BindInterfacesAndSelfTo<ProjectilePool>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProjectileDirector>().AsSingle();
        Container.BindInterfacesAndSelfTo<WeaponSlot>().AsSingle();
        Container.BindInterfacesTo<MountSlot>().AsSingle();
        Container.BindInterfacesTo<DropFactory>().AsSingle().WithArguments(dropPrefabs);
        Container.BindInterfacesTo<DeathEffects>().AsSingle().WithArguments(puffPrefab, fallPrefab);

        GameLog.Info(LogCategory.Game, "Zenject container built");
    }
}
