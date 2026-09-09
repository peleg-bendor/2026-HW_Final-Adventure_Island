using System;

// What is allowed to destroy something. Flags rather than one value per destroyer, so a whole rule
// fits in one field: a rock answers boomerang, mount attack, riding and fairy together.
[Flags]
public enum Destroyer
{
    None = 0,
    Axe = 1,
    Boomerang = 2,
    MountAttack = 4,

    // Separate from MountAttack, since a mount's attack leaves a fire standing while riding into
    // one destroys it.
    Riding = 8,

    Fairy = 16
}
