using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// The attack key. It throws an axe unconditionally - what the player is actually carrying is the
// weapon slot's business, and there is no slot yet.
public class PlayerAttack : MonoBehaviour
{
    // Where a throw leaves him, in units from his feet. X mirrors with his facing.
    [SerializeField] private Vector2 throwOffset = new Vector2(0.6f, 1.2f);

    private ProjectileDirector director;

    [Inject]
    public void Construct(ProjectileDirector director)
    {
        this.director = director;
    }

    private void Update()
    {
        if (Keyboard.current == null || Keyboard.current.zKey.wasPressedThisFrame == false)
            return;

        if (director == null)
        {
            GameLog.Warning(LogCategory.Player, "No ProjectileDirector injected, nothing is thrown");
            return;
        }

        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 origin = (Vector2)transform.position + new Vector2(throwOffset.x * direction, throwOffset.y);
        director.ThrowAxe(origin, direction);
    }
}
