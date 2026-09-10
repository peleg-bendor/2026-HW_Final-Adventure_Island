using UnityEngine;

// What counts as solid ground in this game, and the three questions anything walking or falling
// asks of it. A static class rather than helpers on Enemy, since a falling drop is not an enemy.
public static class Ground
{
    // How far to the nearest ground along a ray, or infinity when there is none.
    public static float DistanceTo(Vector2 from, Vector2 direction, float maxDistance)
    {
        float nearest = Mathf.Infinity;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(from, direction, maxDistance))
        {
            if (Is(hit.collider))
                nearest = Mathf.Min(nearest, hit.distance);
        }

        return nearest;
    }

    // The same question asked with a body rather than a line, for anything that has to move its
    // whole shape through the level and be stopped by what is in the way.
    public static bool SweepTo(Vector2 from, Vector2 size, Vector2 direction, float distance,
        out RaycastHit2D nearest)
    {
        nearest = new RaycastHit2D();
        bool found = false;

        foreach (RaycastHit2D hit in Physics2D.BoxCastAll(from, size, 0f, direction, distance))
        {
            if (Is(hit.collider) == false)
                continue;

            if (found == false || hit.distance < nearest.distance)
            {
                nearest = hit;
                found = true;
            }
        }

        return found;
    }

    // The top of the highest ground a point sits inside, for anything that has to climb out of the
    // level rather than be stopped by it.
    public static bool TopOfOverlap(Vector2 point, out float top)
    {
        top = 0f;
        bool found = false;

        foreach (Collider2D collider in Physics2D.OverlapPointAll(point))
        {
            if (Is(collider) == false)
                continue;

            if (found == false || collider.bounds.max.y > top)
            {
                top = collider.bounds.max.y;
                found = true;
            }
        }

        return found;
    }

    // Ground is the only solid collider in this game: every hazard, pickup, door, projectile and
    // enemy is a trigger, and the player is the one solid thing that is not ground.
    private static bool Is(Collider2D collider)
    {
        return collider != null && collider.isTrigger == false && collider.GetComponent<Player>() == null;
    }
}
