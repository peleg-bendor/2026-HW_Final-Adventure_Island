using UnityEngine;
using Zenject;

// Keeps the camera on the player without letting the view leave the level. Framing the whole level
// is a testing aid rather than anything the game uses.
public class LevelCamera : MonoBehaviour, IResettable
{
    [SerializeField] private Transform target;

    // Half the view's height in world units. Owned here rather than left on the Camera, because
    // framing the whole level overwrites it and has to give it back.
    [SerializeField] private float followSize = 5f;

    // How far up the view the player sits, from the bottom. Below a half he gets headroom, which is
    // where a jump needs the room; horizontally he stays centred.
    [SerializeField, Range(0f, 1f)] private float targetViewportY = 0.25f;

    [SerializeField] private float smoothTime = 0.15f;

    [SerializeField] private bool frameWholeLevel;

    private IGameFlow flow;
    private IResetRegistry registry;

    private Camera view;
    private Vector3 followVelocity;

    // True to begin with, so the first frame opens on the player rather than sliding in from
    // wherever the camera was saved.
    private bool snapNextFrame = true;

    // Read once rather than hardcoded, so moving the camera in the Editor leaves no stale depth.
    private float cameraZ;

    [Inject]
    public void Construct(IGameFlow flow, IResetRegistry registry)
    {
        this.flow = flow;
        this.registry = registry;
    }

    private void Awake()
    {
        view = GetComponent<Camera>();
        cameraZ = transform.position.z;

        if (view == null)
            GameLog.Warning(LogCategory.Game, "No Camera found, the view will not move");

        if (target == null)
            GameLog.Warning(LogCategory.Game, "No target assigned, the camera will not follow");
    }

    private void OnEnable()
    {
        if (registry == null)
        {
            GameLog.Warning(LogCategory.Game, "No IResetRegistry injected, the camera will glide instead of cutting");
            return;
        }

        registry.Register(this);
    }

    private void OnDisable()
    {
        if (registry != null)
            registry.Unregister(this);
    }

    // Flagged rather than snapped here, because the reset walks its list in registration order and
    // the player may not have moved yet. LateUpdate always runs after he has.
    public void ResetTo(ResetScope scope)
    {
        snapNextFrame = true;
    }

    private void LateUpdate()
    {
        Apply(snapNextFrame ? 0f : smoothTime);
        snapNextFrame = false;
    }

    private void Apply(float smoothing)
    {
        LevelDefinition level = flow != null ? flow.CurrentLevel : null;

        if (view == null || level == null)
            return;

        if (frameWholeLevel)
        {
            FrameLevel(level);
            return;
        }

        view.orthographicSize = followSize;

        if (target == null)
            return;

        Vector3 wanted = FollowPosition(level);
        transform.position = smoothing > 0f
            ? Vector3.SmoothDamp(transform.position, wanted, ref followVelocity, smoothing)
            : wanted;
    }

    private Vector3 FollowPosition(LevelDefinition level)
    {
        Rect area = level.PlayableArea;
        float halfHeight = view.orthographicSize;

        // Biased before clamping, so pushing the view up can still never take it past the level.
        float wantedY = target.position.y + halfHeight * (1f - 2f * targetViewportY);

        return new Vector3(
            ClampAxis(target.position.x, area.xMin, area.xMax, halfHeight * view.aspect),
            ClampAxis(wantedY, area.yMin, area.yMax, halfHeight),
            cameraZ);
    }

    // A level smaller than the view on an axis cannot contain the camera at all, so it is centred.
    // Mathf.Clamp would quietly answer with one of the edges instead.
    private static float ClampAxis(float value, float min, float max, float half)
    {
        if (max - min <= half * 2f)
            return (min + max) * 0.5f;

        return Mathf.Clamp(value, min + half, max - half);
    }

    // Sized to whichever axis needs more room, so the whole rect fits at any aspect.
    private void FrameLevel(LevelDefinition level)
    {
        Rect area = level.PlayableArea;

        view.orthographicSize = Mathf.Max(area.height * 0.5f, area.width * 0.5f / view.aspect);
        transform.position = new Vector3(area.center.x, area.center.y, cameraZ);
    }
}
