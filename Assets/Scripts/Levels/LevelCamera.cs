using UnityEngine;

// Keeps the camera on the player without letting the view leave the level. Framing the whole level
// is a testing aid rather than anything the game uses.
public class LevelCamera : MonoBehaviour
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

    private Camera view;
    private LevelDefinition level;
    private Vector3 followVelocity;

    // Read once rather than hardcoded, so moving the camera in the Editor leaves no stale depth.
    private float cameraZ;

    private void Awake()
    {
        view = GetComponent<Camera>();
        cameraZ = transform.position.z;

        if (view == null)
            GameLog.Warning(LogCategory.Game, "No Camera found, the view will not move");

        if (target == null)
            GameLog.Warning(LogCategory.Game, "No target assigned, the camera will not follow");
    }

    private void Start()
    {
        level = FindActiveLevel();

        if (level == null)
        {
            GameLog.Warning(LogCategory.Game, "No active LevelDefinition found, the camera will not be clamped");
            return;
        }

        // Snapped on the first frame, so Play opens on the player instead of sliding in from
        // wherever the camera was last saved.
        Apply(0f);
    }

    private void LateUpdate()
    {
        Apply(smoothTime);
    }

    private void Apply(float smoothing)
    {
        if (view == null || level == null)
            return;

        if (frameWholeLevel)
        {
            FrameLevel();
            return;
        }

        view.orthographicSize = followSize;

        if (target == null)
            return;

        Vector3 wanted = FollowPosition();
        transform.position = smoothing > 0f
            ? Vector3.SmoothDamp(transform.position, wanted, ref followVelocity, smoothing)
            : wanted;
    }

    // Only active objects answer, which is how this picks a level with the other root switched off.
    private static LevelDefinition FindActiveLevel()
    {
        LevelDefinition[] found = FindObjectsByType<LevelDefinition>(FindObjectsSortMode.None);

        if (found.Length > 1)
            GameLog.Warning(LogCategory.Game, "More than one level is active, the camera clamped to " + found[0].name);

        return found.Length > 0 ? found[0] : null;
    }

    private Vector3 FollowPosition()
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
    private void FrameLevel()
    {
        Rect area = level.PlayableArea;

        view.orthographicSize = Mathf.Max(area.height * 0.5f, area.width * 0.5f / view.aspect);
        transform.position = new Vector3(area.center.x, area.center.y, cameraZ);
    }
}
