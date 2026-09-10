using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Turns a configured drop type into a pickup standing in the level. Each prefab declares which type
// it is, so a new drop type is another prefab on the installer rather than an edit here.
public class DropFactory : IDropFactory, IInitializable
{
    private readonly DiContainer container;
    private readonly ILevels levels;
    private readonly GameObject[] prefabs;

    private readonly Dictionary<DropType, GameObject> byType = new Dictionary<DropType, GameObject>();

    public DropFactory(DiContainer container, ILevels levels, GameObject[] prefabs)
    {
        this.container = container;
        this.levels = levels;
        this.prefabs = prefabs;
    }

    // Read once when the container is built, so no drop ever inspects a prefab's components.
    public void Initialize()
    {
        if (prefabs == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No drop prefabs assigned on GameInstaller, nothing can drop");
            return;
        }

        foreach (GameObject prefab in prefabs)
            Register(prefab);

        GameLog.Info(LogCategory.Collectible, "Drops registered: " + byType.Count);
    }

    private void Register(GameObject prefab)
    {
        if (prefab == null)
            return;

        Collectible collectible = prefab.GetComponent<Collectible>();

        if (collectible == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No Collectible found on " + prefab.name + ", it can never be dropped");
            return;
        }

        if (collectible.Drop == DropType.None)
        {
            GameLog.Warning(LogCategory.Collectible, "No drop type set on " + prefab.name + ", nothing can ask for it");
            return;
        }

        if (byType.ContainsKey(collectible.Drop))
        {
            GameLog.Warning(LogCategory.Collectible, prefab.name + " also claims " + collectible.Drop + ", the earlier prefab in the list keeps it");
            return;
        }

        byType[collectible.Drop] = prefab;
    }

    public Collectible Create(DropType type, Vector2 at)
    {
        if (type == DropType.None)
            return null;

        GameObject prefab;

        if (byType.TryGetValue(type, out prefab) == false)
        {
            GameLog.Warning(LogCategory.Collectible, "No prefab registered for " + type + ", nothing was dropped");
            return null;
        }

        // Instantiated through the container rather than with Object.Instantiate, so the pickup is
        // injected like anything else - a token needs the mount slot and a weapon needs the weapon slot.
        GameObject instance = container.InstantiatePrefab(prefab);
        Collectible dropped = instance.GetComponent<Collectible>();

        if (dropped == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No Collectible found on " + prefab.name + ", nothing was dropped");
            Object.Destroy(instance);
            return null;
        }

        Place(instance, at);
        dropped.MarkDropped();

        GameLog.Info(LogCategory.Collectible, "Dropped: " + type);
        return dropped;
    }

    // A child of the level and never of whatever produced it, which would switch the drop off on its
    // way out. It also means a drop goes away with the level it was made in.
    private void Place(GameObject instance, Vector2 at)
    {
        instance.transform.position = new Vector3(at.x, at.y, instance.transform.position.z);

        if (levels.Current == null)
        {
            GameLog.Warning(LogCategory.Collectible, "No level is active, the drop was left at the scene root");
            return;
        }

        instance.transform.SetParent(levels.Current.transform, true);
    }
}
