# 2026-HW_Final-Adventure_Island

The final project for the "Methods in Game Development" Unity course — a 2D platformer inspired by
Adventure Island 2 and 3, with two levels, a draining power meter, rideable animals, six enemy types
and no scene reloads anywhere.

**In progress.** This README describes what the project is and how it is put together; what is built
so far and what is next is in [`HW_Final_PLAN.md`](HW_Final_PLAN.md).

## Starting point

A fresh Unity 6 project, not a copy of an earlier exercise. The three Mario exercises before it are
finished and graded, and this is a different game, so nothing about their scenes, prefabs, hierarchy
or gameplay code carries over.

Eleven files did travel from `2026-HW_3-Mario`, all of them infrastructure this project would
otherwise rebuild from scratch:

- The editor tooling — `LevelWindow`, `TilePlacerWindow`, `TilePrefabMap`, `TiledMap` and
  `SceneObjectMemory`, plus `LogsWindow`.
- The logging system — `GameLog` and its four supporting files.

None of them needed a line changed for a different game: `LevelWindow` holds no tile ids of its own
and the logging is generic, so only `LogCategory`'s values were rewritten to name this game's
systems.

## What it has to do

[`Exercise Adventure Island.md`](Exercise%20Adventure%20Island.md), at the project root. That
document is the requirements in one decided piece, built by reading the exercise text and the lesson
12 transcript together — the instructor corrected and extended the written text on camera in about a
dozen places without the document ever being updated, so where the two disagree the transcript wins.
Requirements are numbered `section.item` and every stage in the plan is written against those
numbers.

The seven techniques the exercise names, with SOLID throughout: **DI, Pooling, Builder, Factory,
MVC, Async & Tasks, Template**. Where each of them lives, and why there rather than somewhere else,
is argued in the plan's Stage 1.

## Dependency injection

Zenject — Extenject 9.2.0, the same build lesson 12 used, under `Assets/Plugins/Zenject/` with its
sample and test folders removed. `Assets/Scripts/Installers/GameInstaller.cs` is the composition
root: the one place that decides which implementation satisfies which interface. Logic lives in
plain C# classes with their dependencies injected, and MonoBehaviours are thin adapters that forward
Unity's events into them.

Two `CS0618` deprecation warnings come out of Zenject's own source on Unity 6, from
`Object.FindObjectsOfType<T>()`. The API is deprecated rather than removed, and the copy is left
unmodified on purpose.

## Logging

No requirement asks for it. Game code writes through `GameLog` rather than calling `Debug.Log`
directly: one static class, one category per line, and the informational levels compiled out of a
release build entirely. `Tools > Logs` sets a level per category while the game runs, and every Play
session is written to `GameLog.txt` beside the project.

## The levels

`Assets/Levels/Level01.txt` and `Level02.txt` hold each level as a grid of tile ids, and
`TilePrefabMap.asset` says which prefab each id means. `Tools > Level` builds a file into the scene
and writes the scene back out; `Tools > Tile Placer` stamps and erases single tiles. The data file
is the source and the scene is the output, so building deletes and recreates every child of the
level root it was pointed at.

Tiled authors the first draft of a level, because that is easier there. Every edit after that
happens through the tools in Unity, because re-exporting from Tiled is not.

Both levels live in the one scene, one active at a time. Nothing in this project calls
`SceneManager.LoadScene` — not on death, not on game over, not on the level transition. Starting a
level resets that level's own variables instead.

## Running it

Open the project in Unity, then open `Assets/Scenes/Scene_Game.unity`.

## Conventions

The comment, logging, naming, hierarchy and code-quality rules the code follows are in
[`CONVENTIONS.md`](CONVENTIONS.md). They came across from the Mario projects; the naming and
hierarchy section was rewritten for this game.

## Submission

Two recordings rather than one: the game being played, and a walk through the code. The instructor
watches the gameplay first, writes down what he sees, then watches the code, and builds the oral
defense questions out of both. Anything not shown counts as not done.
