# Conventions

The rules this project follows, in one place so they don't have to be re-typed at the start of
every session. Written for whoever is working on the code, not for a grader: each rule states what
to do, and the reasoning behind it lives in the current exercise's plan file, under Notes /
Decisions Log. Most of it was first argued out in `2026-HW_2-Mario/HW_2_PLAN.md`.

The comment rules were agreed in Exercise 1 and revisited in Exercise 2 against how the code had
actually been written since. The logging rules were written down for the first time in Exercise 2,
from the habits 89 existing calls already followed. The naming, hierarchy and code-quality rules
had been carried by hand from session to session until they landed here.

Carried into the final project at its Stage 2. The logging and code-quality rules came across
unchanged. The comment rules were cut back during Stage 6: four lines became two, and a comment
now names the alternative it rejected instead of making the case for it, because the case is
already in the plan and reading it twice in every file was tiring. The naming and hierarchy section was rewritten, because this is a different game:
two levels living in one scene, 16px source art instead of Mario's, and a dependency-injection
container sitting in the hierarchy. Stage 4 revised that section again once the level pipeline was
actually built: the pivot, the sorting layers, the prefab rules, and what a rebuild does now.

This file does not travel with `Assets` - it sits at the project root, so copying `Assets` into the
next exercise leaves it, `README.md` and `.gitignore` behind. Copy all four by hand.

## Comments

1. **Two lines maximum**, on their own lines above what they describe. A file header may run to
   three. No trailing comments. A comment that needs more room is arguing something that belongs
   in the plan.
2. **Name the rejected alternative, don't argue it.** "A capsule rather than a box, so he doesn't
   catch on the seams" is the whole comment. The case for the decision lives in the plan's Decisions
   Log and is not repeated in every file the decision touches.
3. Comment why, not what. The exception is a serialized field, which may carry a one-line gloss of
   what it means and what units it is in, since the Inspector shows its name and nothing else.
4. Every file has a header comment: below the `using` block, directly above the type or its
   attributes, `//` and not `///`. What the class is for, and what it deliberately isn't.
5. A nested type gets its own header only when its existence is non-obvious.
6. Comment a field only when the value or its existence is non-obvious, and a branch only when a
   reader would plausibly delete it. Most lines get nothing.
7. A comment makes one point. Reaching for "also" means the second claim belongs at the line it
   explains, or nowhere.
8. State what is true now. No changelog voice, nothing about what the code used to do.
9. Never repeat a value the code or the scene already holds. Say what the number means instead:
   "the cutoff sits at 60 degrees off vertical" rather than the `0.5f` the code already carries.
10. No references to plan stages, lesson numbers, or the plan file.
11. A comment naming another class, event or method is a reference. Renaming that thing means
    updating the comment with it.
12. If in doubt, write it so a human reading the file cold understands it. That outranks the rules
    above where they conflict.

## Logging

1. Log at meaningful state transitions. Never per frame.
2. Name the subject first, then say what changed: `Health lost - 2 remaining`, not `Lost health`.
3. `" - "` introduces detail. `": "` introduces a name.
4. No terminal punctuation.
5. A rejected action reads `<action> ignored - <reason>`. Use "ignored" rather than "skipped".
6. Append `gameObject.name` only when several of that thing can exist at once. Mario is one object,
   so naming him repeats what the message already said.
7. Never put the logging class's own name in the message. The Console already shows the class and
   line. Naming a *different* type is fine and often necessary, as in `No HealthView assigned`.
8. One event, one line. The class that owns the decision logs it; the classes it passes through
   stay quiet.
9. When many instances make the same transition on the same frame, log it once for all of them, guarded on `Time.frameCount`. The level reset restoring every collectible at once is the case to watch for.
10. A missing Inspector reference is a warning, shaped `No X found, <what stops working>`.
11. A line that fires on a timer or on every contact goes to `GameLog.Verbose`, not `Info`. It
    stays available behind one dropdown without filling the Console.
12. Never put a side effect in a log argument. `Info` and `Verbose` are `[Conditional]`, so the
    call and everything inside it disappears from a release build.
13. `Assets/Scripts/Editor/` keeps plain `Debug` and reports results rather than events. Tool
    feedback is not game logging, and it never reaches a build.

## Naming and hierarchy

Sprites live at `Assets/Sprites/Sprite_X.png`. The source art is drawn on a 16px grid and sprites
span one cell or several: a tile and the smallest enemies are 16x16, most enemies are 16x16 to 16x32,
the player is 16x32, and an animal with its rider is about 32x32. All of it is upscaled 3x with
nearest-neighbour - any smoothing turns pixel art to mush - so one grid cell is stored as 48px.
Pixels Per Unit is 48 for every sprite, whatever its dimensions. A 16px source dimension is then exactly one
world unit: a tile is 1x1, an animal with its rider is 2x2, and a creature 16 wide by 24 tall ends
up 1 by 1.5. Setting PPU per sprite to normalise every sprite to one unit would break the tile grid,
which `TilePlacerWindow` draws as a 1-unit cell cursor and places on integer coordinates.

Every sprite is anchored at the middle of its bottom cell: a custom pivot of `(0.5, 24 / texture
height)`, clamped to `0.5` for anything shorter than a cell. One integer coordinate then means the
same thing for a 1x1 tile as for a 3x4 animal, and a creature's feet land on a cell boundary whatever
its height. Unity's default Center fails on parity, because art is bottom-aligned inside a whole
number of cells: an odd-height sprite lands on a boundary and an even-height one lands half a cell
into the floor. `SpriteImportRules.cs` applies the pivot on every import along with PPU 48, Point
filtering, no compression and Full Rect, so a setting changed by hand in the Inspector goes back on
the next reimport. Don't set import settings by hand; change the rule.

Cut per sheet, not per sprite: key out the background at native resolution first, so the colour match
is exact, then upscale the whole sheet, then cut on a 48px grid. A sprite spans one cell, two or six
depending on what it is, so the grid is the guide rather than the cut size.

Prefabs live in `Assets/Prefabs/`, flat. Those for objects placed in the level take the `Sprite_`
prefix; those for things spawned at runtime don't.

A collider is sized to the art rather than to the sprite's box, since the box carries transparent
padding: the door measures 1.3 by 1.8 inside a 2x2 box. Ground tiles carry a plain `BoxCollider2D`
with no `CompositeCollider2D` above them, so anything that walks needs a rounded bottom or it catches
on the seams between tiles - a capsule, or a circle where the body is a single cell.

Sorting layers, back to front: `Background`, `Level`, `Pickups`, `Enemies`, `Player`, `Effects`.
Unity's `Default` is left first and unused, so a prefab whose layer was forgotten renders behind the
ground and shows itself.

Scripts are grouped under `Assets/Scripts/` by domain, with three folders named for a pattern
instead: `Builder/`, `Factory/` and `Pooling/`. Grouping by pattern is worse organisation - it
scatters the projectile system across four folders - and it is kept anyway, because the code video
is what gets graded and "show me the Factory" has to be answerable by opening one folder. The rest
are `Editor/`, `Logging/`, `Installers/`, `State/`, `Levels/`, `Player/` with `Projectiles/` and
`Weapons/` beneath it, `Enemies/`, `Animals/`, `Collectibles/`, `Hazards/`, `UI/` and `Extensions/`.

There is no `Interfaces/` folder. An interface lives beside the thing implementing it. Exercise 3
had few enough to collect in one place; this project has `IResettable`, `IDestructible`,
`IProjectilePool`, `IDropFactory` and three MVC pairs, and one folder would separate every one of
them from its implementation.

In the Hierarchy the scene root holds `Main Camera`, `SceneContext` with `GameInstaller` as its only
child, `Scripts` for logic-only manager objects, `Logging` for `LogSettings` and `LogFileWriter`,
`Level_1` and `Level_2`, and `Canvas` holding the `Txt_` GUI objects with the `EventSystem` uGUI adds
beside it. Both levels exist in the one scene with one active at a time, because nothing in this
project loads a scene.

The levels are data-driven. `Assets/Levels/Level01.txt` and `Level02.txt` hold one tile id per cell,
in Tiled's map format although nothing here is authored in Tiled: `Tools > Level` builds a file into
the scene and writes the scene back out, and `Tools > Tile Placer` paints and erases, by click or by
drag, one undo entry and one Console line per stroke. Both tools take the level root as a field, so
point them at `Level_1` or `Level_2` to match the file before pressing anything. Saving the level
file deliberately does not save the scene, so a tile can be tried without committing it.

Building is a diff rather than a teardown. A cell already holding the right prefab is left alone, a
cell holding the wrong thing is replaced, and anything the file doesn't name is removed. That is what
makes per-instance configuration safe: the file records what is where, and the scene instance records
which one it is - a spider's range, a bird's dip, an enemy's drop, a ביצה's contents. None of it
would survive a rebuild that emptied the parent first.

Tile ids are contiguous and grouped by kind in the order they were added, starting at 1. The mapping
from id to prefab lives only in `Assets/Levels/TilePrefabMap.asset`, so a new tile type is a row in
that asset rather than an edit to any tool. A new id appends rather than slotting into its group,
since renumbering would mean rewriting every level file by hand.

## Code quality

1. One class, one responsibility.
2. Use interfaces the way the course examples do, rather than growing a class an if/else branch at
   a time as new types arrive.
3. Null-check anything obtained from the Inspector, `GetComponent`, `Find`, or parsed from a file.
4. No magic numbers. A named `public` or `[SerializeField]` field when it is tunable, a `const`
   when it describes the code's own shape.
5. No abstraction before a second real use case. Two known future needs that would share no code
   are a naming convention, not a base class.
6. The prefab's value is what runs. A script's own default is not kept in sync with it, since
   retuning a number that has no effect is noise in a diff.
