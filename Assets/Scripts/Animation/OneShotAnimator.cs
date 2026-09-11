using System.Collections;
using UnityEngine;

// Shows a SpriteRenderer each of a list of frames once, then destroys the object it is on. The
// looping animator runs off a shared clock, which is wrong for something that starts when it is made.
public class OneShotAnimator : MonoBehaviour
{
    // The frames in order, each shown once.
    [SerializeField] private Sprite[] frames;

    // How long each frame is held, in seconds.
    [SerializeField, Min(0.01f)] private float secondsPerFrame = 0.08f;

    // A coroutine and not a Task: it runs on this object for the whole of its short life, cancels
    // nothing and returns nothing, and stopping under a popup is what it should do.
    private IEnumerator Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || frames == null || frames.Length == 0)
        {
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer or no frames on " + name + ", it shows nothing");
            Destroy(gameObject);
            yield break;
        }

        foreach (Sprite frame in frames)
        {
            spriteRenderer.sprite = frame;
            yield return new WaitForSeconds(secondsPerFrame);
        }

        Destroy(gameObject);
    }
}
