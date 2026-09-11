using UnityEngine;

// Both ways something can leave the screen: the puff, and the upside-down fall of an enemy the player
// killed. Only an enemy can go either way, so only an enemy asks for this rather than for IPuff.
public interface IDeathEffects : IPuff
{
    void Fall(SpriteRenderer body);
}
