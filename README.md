# 2026-HW_Final-Adventure_Island

The final project for the "Methods in Game Development" Unity course: a 2D platformer inspired by
Adventure Island 2 and 3. Two levels, three strikes, a power bar that drains as you play, fruit that
refills it, two throwable weapons, three rideable animals, six enemy types, eggs that hatch into
what they were authored to hold, and a fairy that destroys whatever the player touches.

## Running it

Open the project in Unity 6, then open `Assets/Scenes/Scene_Game.unity` and press Play.

Arrows move, Space jumps and holding it jumps higher, Z throws the weapon being carried or attacks
with the animal being ridden.

## How it is put together

Zenject (Extenject 9.2.0) is under `Assets/Plugins/Zenject/`, with its sample and test folders
removed. `Assets/Scripts/Installers/GameInstaller.cs` is the composition root: the one place that
decides which implementation satisfies which interface, and where the game's rule numbers enter.
The rules live in plain C# classes with their dependencies injected, and the MonoBehaviours are thin
adapters that forward Unity's messages into them.

Scripts are grouped under `Assets/Scripts/` by domain and never by pattern, so a pattern's
implementation sits with the part of the game it serves:

- **Dependency injection** — `Installers/GameInstaller.cs`, with 31 bindings.
- **Builder and Object Pooling** — `Projectiles/`: the director holds a recipe per projectile kind,
  the builder turns one into a configured instance, and the pool builds every kind at startup and
  hands out an inactive copy.
- **Factory** — `Collectibles/DropFactory.cs`: an enemy or an egg asks for a drop type and gets back
  a finished pickup.
- **Template Method** — four bases own a fixed sequence each and leave the varying steps to their
  subclasses: `Enemies/Enemy.cs`, `Hazards/Hazard.cs`, `Collectibles/Collectible.cs` and
  `Projectiles/BaseProjectile.cs`.
- **MVC** — `MVC/`, one folder per triad: the power bar, the strikes and the fruit count.
- **Async and Tasks** — `Enemies/RespawnCountdown.cs` for an enemy's countdown, and `State/GameFlow.cs`
  with `UI/Popup.cs` for the wait on a popup's button. The fairy, the egg's crack and the death puff
  are coroutines instead, since those wait on objects that stay alive and should freeze under a popup.
- **SOLID** throughout: nothing in the game code switches over a type or checks one, and what varies
  by kind is a subclass, an interface, or data authored on the object.

## Logging

Game code writes through `GameLog` rather than calling `Debug.Log`, one
category per line, with the informational levels compiled out of a release build entirely.
`Tools > Logs` sets a level per category while the game runs, `LogSettings` in the scene holds the
levels a session starts with, and every Play session is written to `GameLog.txt` beside the project.

## The levels

`Assets/Levels/Level01.txt` and `Level02.txt` hold each level as a grid of tile ids, and
`TilePrefabMap.asset` says which prefab each id means. `Tools > Level` builds a file into the scene
and writes the scene back out to the file, and `Tools > Tile Placer` paints and erases single tiles.

Building is a diff rather than a teardown: a cell already holding the right prefab is left alone, a
cell holding the wrong thing is replaced, and anything the file doesn't name is removed. That is
what lets configuration set by hand survive a rebuild, such as an egg's contents, a bird's dip or an
enemy's drop, since the file records what is where and the scene instance records which one it is.

The file format is Tiled's map JSON, kept to that shape so going back to Tiled stays possible,
although both levels here were painted with the Tile Placer. Level 1 runs 200 cells by 18 and level 2
is 30 by 57. Both live in the one scene with one active at a time.

Nothing in this project calls `SceneManager.LoadScene`: not on death, not on game over, not on the
level transition. Losing a strike resets the current level's own objects through `IResettable` and the
reset registry, and moving to level 2 switches which level root is active.

## The recordings

- **Game Playthrough** — one run of both levels through to the congratulation popup.

  <https://youtu.be/UegTktSMyG4>
- **Game Features** — the features one at a time, each on a level arranged to show it.

  <https://youtu.be/l6sZotT-q1g>
- **Code, part 1** — <https://youtu.be/FDfgf3rh2Ko>
- **Code, part 2** — <https://youtu.be/PnLBzyyOXrc>