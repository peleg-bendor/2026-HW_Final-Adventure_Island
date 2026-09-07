using UnityEngine;

// Cycles a SpriteRenderer through a list of frames forever. An Animator would need a controller and
// a clip asset per object to say the same thing about two sprites and one interval.
public class SpriteCycleAnimator : MonoBehaviour
{
    // The frames in order, shown on a loop. Fewer than two switches this off.
    [SerializeField] private Sprite[] frames;

    // How long each frame is held, in seconds.
    [SerializeField] private float secondsPerFrame = 0.2f;

    private SpriteRenderer spriteRenderer;
    private int shownIndex = -1;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            GameLog.Warning(LogCategory.Game, "No SpriteRenderer found on " + name + ", it will not animate");
            enabled = false;
            return;
        }

        if (frames == null || frames.Length < 2 || secondsPerFrame <= 0f)
        {
            GameLog.Warning(LogCategory.Game, "No frames or no interval set on " + name + ", it will not animate");
            enabled = false;
        }
    }

    // Read off Time.time rather than counted per instance, so every copy of a thing is in step by
    // construction instead of by having been switched on at the same moment.
    private void Update()
    {
        int index = (int)(Time.time / secondsPerFrame) % frames.Length;

        if (index == shownIndex)
            return;

        shownIndex = index;
        spriteRenderer.sprite = frames[index];
    }
}
