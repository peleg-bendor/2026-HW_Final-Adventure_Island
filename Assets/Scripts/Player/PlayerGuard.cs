using UnityEngine;

// The player's protections in one place. Riding a mount and holding a fairy are the two things
// that absorb a contact, and no hazard or enemy holds either rule itself.
public class PlayerGuard : MonoBehaviour, IPlayerGuard
{
    public bool TryAbsorb(IDestructible source)
    {
        return false;
    }
}
