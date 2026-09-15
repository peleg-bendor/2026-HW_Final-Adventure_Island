using System;
using UnityEngine;

// The projectile prefabs, in one object so they can be injected by type. Four bare GameObjects
// bound side by side would be ambiguous to the container.
[Serializable]
public class ProjectilePrefabs
{
    public GameObject axe;
    public GameObject boomerang;

    // Named for the snake because the red mount spits fire of its own, with different numbers.
    public GameObject snakeFireball;

    public GameObject mountFire;
}
