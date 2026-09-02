using UnityEngine;
using UnityEngine.InputSystem;

// The attack key. Holds no weapon and throws nothing - it exists so the input is in place
// before there is anything to throw.
public class PlayerAttack : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.zKey.wasPressedThisFrame)
            GameLog.Info(LogCategory.Player, "Attack ignored - no weapon held");
    }
}
