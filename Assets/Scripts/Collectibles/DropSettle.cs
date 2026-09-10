using UnityEngine;

// Brings a dropped pickup to rest somewhere it can be reached: out of any ground it appeared inside,
// then down onto the first solid thing below. Only on the prefabs the factory spawns.
public class DropSettle : MonoBehaviour
{
    // Every sprite is anchored at the middle of its bottom cell, so resting on a surface means
    // sitting half a cell above it rather than exactly on it.
    private const float PivotAboveGround = 0.5f;

    // How many times it will climb out of a tile, which is how thick a wall it can be buried in.
    private const int MaxLifts = 4;

    // How fast it falls, in units a second.
    [SerializeField, Min(0f)] private float fallSpeed = 8f;

    // How far below it looks for ground. Past this it stays where it appeared, which is what a drop
    // over a pit does instead of falling out of the world.
    [SerializeField, Min(0f)] private float maxFall = 20f;

    private float restY;
    private bool falling;

    // Settled in Start rather than in OnEnable, since the factory positions it after the instantiate
    // that raised OnEnable.
    private void Start()
    {
        Vector2 here = transform.position;
        float top;

        // Climbed clear first, or the cast below starts inside a tile and finds it at no distance.
        for (int lift = 0; lift < MaxLifts && Ground.TopOfOverlap(here, out top); lift++)
            here.y = top + PivotAboveGround;

        MoveTo(here.y);

        float distance = Ground.DistanceTo(here, Vector2.down, maxFall);

        if (float.IsInfinity(distance))
        {
            GameLog.Verbose(LogCategory.Collectible, name + " found no ground below, it stays where it appeared");
            return;
        }

        restY = here.y - distance + PivotAboveGround;
        falling = restY < here.y;
    }

    private void Update()
    {
        if (falling == false)
            return;

        float y = Mathf.MoveTowards(transform.position.y, restY, fallSpeed * Time.deltaTime);
        MoveTo(y);

        if (Mathf.Approximately(y, restY))
            falling = false;
    }

    private void MoveTo(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
