using System;

// How many of each projectile the pool builds when the game starts. For the player's kinds that is
// also the most that can be in the air at once; the snake's is only where its pool begins.
[Serializable]
public class ProjectileCounts
{
    // The instructor's own number, from watching the original: that many go out, then you wait.
    public int axe = 3;

    // It comes back to him, so a second in the air would mean nothing.
    public int boomerang = 1;

    // A starting size rather than a cap. How many a level needs depends on how many shooters it holds.
    public int snakeFireball = 4;

    // Capped the way the axe is, since how many the player may have in the air is a rule of this game.
    public int mountFire = 3;
}
