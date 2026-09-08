using UnityEngine;

// A bird that waits on its perch until the player is near and to its left, swoops leftward through
// one dip, then flies back to the perch and waits again. It never leaves the level.
public class Bird : Enemy
{
    // How fast it flies, in units per second, out and back.
    [SerializeField, Min(0f)] private float speed = 4f;

    // How far below the perch the bottom of the swoop passes, in units.
    [SerializeField, Min(0f)] private float dip = 3f;

    // How far left the swoop travels before it turns for home, in units.
    [SerializeField, Min(0f)] private float swoopDistance = 8f;

    private bool swooping;
    private bool returning;
    private float swoopStartedAt;

    // An axe, a boomerang, an animal or a fairy, which is every enemy except the ghost.
    protected override Destroyer DestroyedBy
    {
        get
        {
            return Destroyer.Axe | Destroyer.Boomerang | Destroyer.AnimalAttack | Destroyer.Riding |
                   Destroyer.Fairy;
        }
    }

    // A swoop and the flight home both finish even if he has run out of range, or the bird would
    // stop in mid-air with nothing to bring it back.
    protected override bool IsMidAction
    {
        get { return swooping || returning; }
    }

    // Back on the perch and waiting, whether the level restarted or it was just killed.
    protected override void OnSpawned()
    {
        swooping = false;
        returning = false;
        Face(false);
    }

    protected override void Behave()
    {
        if (returning)
        {
            FlyHome();
            return;
        }

        if (swooping == false)
        {
            // Flying left at someone already to the right would only carry it away from him.
            if (PlayerPosition.x > transform.position.x)
                return;

            swooping = true;
            swoopStartedAt = Time.time;
            Face(false);
            GameLog.Verbose(LogCategory.Enemy, name + " swooped - he was " +
                Vector2.Distance(transform.position, PlayerPosition).ToString("0.0") + " away");
        }

        Swoop();
    }

    private void Swoop()
    {
        float travelled = (Time.time - swoopStartedAt) * speed;

        if (swoopDistance <= 0f || travelled >= swoopDistance)
        {
            swooping = false;
            returning = true;
            Face(true);
            return;
        }

        // One full cosine across the swoop: level at the perch, lowest halfway, level again at the
        // far end. How V-shaped it reads is the dip against the distance, not a shape of its own.
        float drop = dip * (1f - Mathf.Cos(travelled / swoopDistance * 2f * Mathf.PI)) * 0.5f;
        transform.position = new Vector3(Home.x - travelled, Home.y - drop, transform.position.z);
    }

    private void FlyHome()
    {
        Vector3 perch = new Vector3(Home.x, Home.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, perch, speed * Time.deltaTime);

        if (transform.position == perch)
        {
            returning = false;
            Face(false);
        }
    }
}
