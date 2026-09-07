using UnityEngine;

// The steps that turn a prefab into a configured projectile, and the step that builds it. A
// director drives them in order; a caller only ever needs what Build hands back.
public interface IProjectileBuilder
{
    void SetSpeed(float speed);
    void SetLift(float lift);
    void SetGravity(float gravityScale);
    void SetMaxSeconds(float maxSeconds);

    BaseProjectile Build(GameObject prefab);
}
