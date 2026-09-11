using UnityEngine;

// Spawns the two things something can leave on screen when it is destroyed. Instantiated plainly
// rather than through the container, since neither needs anything injected.
public class DeathEffects : IDeathEffects
{
    private readonly OneShotAnimator puffPrefab;
    private readonly FallingBody fallPrefab;

    public DeathEffects(OneShotAnimator puffPrefab, FallingBody fallPrefab)
    {
        this.puffPrefab = puffPrefab;
        this.fallPrefab = fallPrefab;

        if (puffPrefab == null || fallPrefab == null)
            GameLog.Warning(LogCategory.Game, "No puff or fall prefab assigned on GameInstaller, some things will vanish without an effect");
    }

    // A sibling of what was destroyed rather than a child, which would be switched off with it. It
    // goes away with the level it happened in for the same reason.
    public void Puff(Transform at)
    {
        if (puffPrefab == null)
            return;

        Object.Instantiate(puffPrefab, at.position, Quaternion.identity, at.parent);
    }

    public void Fall(SpriteRenderer body)
    {
        if (fallPrefab == null)
            return;

        FallingBody fall = Object.Instantiate(fallPrefab, body.transform.position, Quaternion.identity, body.transform.parent);
        fall.Begin(body);
    }
}
