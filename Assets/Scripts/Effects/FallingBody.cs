using UnityEngine;

// What an enemy killed by the player leaves on screen: its last picture, upside down, hopping once and
// dropping through everything. It has no collider, so nothing in the game can touch it.
public class FallingBody : MonoBehaviour
{
    // How fast it leaves upwards, in units a second. How hard it falls back is the Rigidbody2D's.
    [SerializeField, Min(0f)] private float hopSpeed = 6f;

    // How long it lasts before it is removed, in seconds.
    [SerializeField, Min(0f)] private float lifetime = 1.2f;

    // Called once, straight after it is made, with the renderer of whatever was just destroyed.
    public void Begin(SpriteRenderer from)
    {
        SpriteRenderer picture = GetComponent<SpriteRenderer>();
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();

        if (picture == null || rigid == null)
        {
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer or Rigidbody2D on " + name + ", the enemy vanishes without falling");
            Destroy(gameObject);
            return;
        }

        // Enemies turn by their scale rather than by flipping the renderer, so the facing is read off
        // the scale and carried over here as a flip.
        picture.sprite = from.sprite;
        picture.flipX = from.transform.lossyScale.x < 0f;
        picture.flipY = true;

        rigid.linearVelocity = Vector2.up * hopSpeed;
        Destroy(gameObject, lifetime);
    }
}
