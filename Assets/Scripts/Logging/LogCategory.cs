// The groupings log lines are filtered by, one level above the class the Console already shows.
// Taken from the systems the plan's stages actually build, so every call site has an obvious home
// and no category ends up holding a single line. Revisited at Stage 20 against where the calls
// really landed, since this time the categories were chosen before the code that fills them.
public enum LogCategory
{
    Player,
    Enemy,
    Animal,
    Weapon,
    Projectile,
    Collectible,
    Hazard,
    Game
}
