using UnityEngine;

// Plays the puff something leaves where it stood when it is destroyed. Split from the fall, so a
// hazard, which only ever puffs, depends on nothing it does not use.
public interface IPuff
{
    void Puff(Transform at);
}
