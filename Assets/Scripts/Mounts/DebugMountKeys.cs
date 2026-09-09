using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

// Q, W and E put the player on a mount, standing in for the eggs that will drop the tokens.
// Development speed only, and the eggs are what replace it.
public class DebugMountKeys : MonoBehaviour
{
    [SerializeField] private MountDefinition blue;
    [SerializeField] private MountDefinition red;
    [SerializeField] private MountDefinition green;

    private IMountSlot slot;

    [Inject]
    public void Construct(IMountSlot slot)
    {
        this.slot = slot;
    }

    private void Update()
    {
        if (slot == null || Keyboard.current == null)
            return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            slot.Take(blue);

        if (Keyboard.current.wKey.wasPressedThisFrame)
            slot.Take(red);

        if (Keyboard.current.eKey.wasPressedThisFrame)
            slot.Take(green);
    }
}
