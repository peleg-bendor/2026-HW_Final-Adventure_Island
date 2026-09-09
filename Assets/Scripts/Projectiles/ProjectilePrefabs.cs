using System;
using UnityEngine;

// The projectile prefabs, in one object so the installer passes a single argument. Zenject matches
// WithArguments by type, and bare GameObjects side by side would be ambiguous.
[Serializable]
public class ProjectilePrefabs
{
    public GameObject axe;
    public GameObject boomerang;

    // Named for the snake because the red mount spits fire of its own, with different numbers.
    public GameObject snakeFireball;

    public GameObject mountFire;
}
