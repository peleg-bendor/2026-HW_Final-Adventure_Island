# HW-Final Plan — Adventure Island

Shared working notes for the final project, modeled on `2026-HW_3-Mario/HW_3_PLAN.md`. The assistant
may edit this file directly as we go (it's not game code, just shared notes).

This started as a skeleton with everything between Stage 5 and the last two left blank. Stage 0 and
Stage 1 filled it in, on 1.9.2026: Stage 0 produced `Exercise Adventure Island.md` beside this file,
which is the requirements in one decided piece and what every stage here is written against, and
Stage 1 turned the gap into stages 6 to 19, each with its sub-steps and a note on what it actually
needs rather than which pattern it was assigned. Stage 1's five steps are the reasoning behind all
of it and are worth reading before arguing with any stage.

**Submission 26.9.2026.** Oral defense with the instructor on 28, 29 or 30 September, 18:00-21:00,
booked by email. That is about four weeks from the start of this plan, and the defense means the code
has to be explainable out loud, not only working.

## Status Legend

- `[ ]` not started
- `[~]` in progress
- `[x]` done AND confirmed working in-editor

## Git Workflow Reminder

- After any step that leaves the project in a working state, consider committing (small, working
  commits > one giant commit).
- After a whole stage is finished and confirmed working, push.

## What the final project asks for

Not restated here on purpose. `Final Project.md` is the exercise text and it is less clear than
Exercises 1 to 3 were, and the lesson 12 transcript both corrects it and adds to it in a dozen
places. Stage 0 produces `Exercise Adventure Island.md`, a single clear reading of both, and that
file is what every later stage is written against.

The one-line version: a two-level platformer inspired by Adventure Island 2/3, with strikes, a
draining power meter, fruit, rideable animals, two throwable weapons, six enemy types, eggs, a
fairy, and no scene reloads anywhere. Required techniques, named by the instructor: **DI, Pooling,
Builder, Factory, MVC, Async & Tasks, Template**, with SOLID throughout.

## Stage Order

| Stage      | What it covers                                             |
| ---------- | ---------------------------------------------------------- |
| 0          | `Exercise Adventure Island.md`, and the project name        |
| 1          | Turn this skeleton into a real plan                         |
| 2          | Project and repo setup                                      |
| 3          | Sprites                                                     |
| 4          | The level pipeline, and a scratch level to test it          |
| 5          | Re-read the plan before any game code                       |
| 6 to 19    | Game development. Defined in Stage 1, Step 3                 |
| 20         | Final testing, comments and log check                        |
| 21         | Two video scripts                                            |

### Stage 0 — The exercise, restated `[x]`

`Final Project.md` is harder to work from than the previous three exercise texts. It contradicts
itself in at least one place, leaves several rules ambiguous, and the instructor changed or clarified
a good deal of it on camera in lesson 12 without the document being updated.

#### Step 1 — Write `Exercise Adventure Island.md` `[x]`

`Exercise Adventure Island.md`: 15 sections, two-level numbering (`5.3`, said in the
videos as דרישה 5.3), English body with כוח, פסילה and the object names kept in Hebrew for the video.
Everything in it is required unless tagged `[good]`, `[nice]` or `[out]`, and section 15 collects the
out-of-scope items in one place.

A section 16 held four questions for the instructor while the step ran. All four were settled before
the step closed, so it was removed rather than left as a stale to-do.

How the five items the skeleton flagged went:

- **20 fruit.** Settled as a penalty: every 20 costs a פסילה, the count never resetting, so it fires
  again at 40 and at 60. Peleg's call, and it is the literal reading of both sources. The discussion
  argued the other way — at 00:09:57 he defines פסילות as how many goes you have left, and the 100 he
  mentions there is the original game's own counter read off its HUD rather than a second rule, which
  is why 30 and then 20 replace it.
- **The rock's cost.** Settled at 3, in a serialized field. "כתוב פה טעות שלוש קווים" (00:18:30)
  reads equally as calling the written 3 wrong or as correcting himself back to it, so it does not
  override the written text on its own.
- **Strike count and what resets.** Settled. A פסילה costs the weapon and the animal and resets כוח
  to the level's starting amount; the fruit count survives. Every collectible in the level comes back
  — fruit, eggs, placed items — and only dead enemies persist, matching the original and keeping a
  level from getting harder each time you die in it.
- **Whether the ghost, the frog and the second snake are all required.** Settled: all six enemies. He
  walks through each one with its own behaviour spec, and the only "one is enough" (00:43:21) is about
  how many ghosts, not which types.
- **Which three animals.** Settled: blue, red, green, with לב, עלה and כוכב as their tokens.

Five things surfaced that neither the plan nor the first read had: the ridden animal carries into
level 2, a weapon is kept while riding and returns when the animal dies, the power bar has a ceiling,
two popups are required and neither may reload a scene, and the thrown axes must be pooled — which
is the instructor placing one of the seven techniques himself. Crouching went from `[nice]` to
`[out]`, and weapons now carry from level 1 into level 2 alongside the animal and the fruit count,
which reverses what the emulator showed and is a deliberate choice. The instructor's request that one
level give the axe and the other the boomerang (11.6) still works, since level 2's boomerang replaces
whatever arrived with the player.

**The 20-fruit penalty constrains level length, and item 14 has to satisfy it.** כוח starts at 11 and
drains 1 per 2 seconds, so a bare bar is 22 seconds and a full one 32. A two-minute level needs
roughly 60 units, meaning 25 to 49 fruit depending on the mix, which costs one to two פסילות per
level and two to four across the game against the three you start with. Under those numbers a
normally-paced run dies before the exit. The levers are the drain rate, level length, and how much
fruit is placed, and the last one fights the instructor's request that every item be reachable in the
opening half-minute. The question is closed; this is recorded because the constraint is real
whichever way the wording was meant.

#### Step 2 — Project name `[x]`

`2026-HW_Final-Adventure_Island`. Fits the existing `2026-HW_<n>-<Game>` shape, with `Final` where a
number went and the first two-word game name the pattern has met. The underscore stays: the hyphen is
already separating year from exercise from game, so a hyphen inside the game name would read
ambiguously, and the underscore is already the within-field joiner in `HW_Final` and `HW_3`.
`AdventureIsland` was the alternative and it would have invented a second convention to avoid one
character. The folder is 15 characters longer than `2026-HW_3-Mario`, which is worth remembering if
Unity's `Library\PackageCache` ever runs into Windows' 260-character path limit.

### Stage 1 — Turn this skeleton into a real plan `[x]`

The stage this document existed to reach, and the reasoning behind every stage after it.

Five steps, in dependency order. Step 2 comes before Step 3 because Async & Tasks and Builder both
want to live in the level pipeline, so what the pipeline is has to be settled before the techniques
are placed.

- **Step 1** — the system inventory and the state model. `[x]`
- **Step 2** — the level pipeline, what travels from `2026-HW_3-Mario`, and level 2's shape. `[x]`
- **Step 3** — the stage list, and what each stage actually needs. `[x]`
- **Step 4** — animation scope, which decides how many sprites Stage 3 has to cut. `[x]`
- **Step 5** — coverage check across the seven techniques, with the defense answer for each. `[x]`

#### Step 1 — The system inventory and the state model `[x]`

**Three scopes of state.** With no scene reload anywhere, something has to own the word "reset", and
`Exercise Adventure Island.md` turns out to need three different ones:

- **Session state** — פסילות and the fruit count. Survives deaths, survives the level change, cleared
  only by the Game Over popup's restart button.
- **Carried state** — the weapon and the ridden animal. Survives the level change, lost on a פסילה.
- **Level state** — כוח, which enemies are alive, their respawn timers, which eggs are open, which
  fruit and items are collected, and where the player is. Fully reset when a level starts. Partially
  reset on a פסילה: player back to the start, כוח back to 11, every collectible restored, and dead
  enemies left dead.

That partial reset is the unusual one and it is exactly what the instructor asked for at 00:11:29. It
is also what a scene reload makes impossible to get right, which is presumably why he raises the two
together.

**The systems, in dependency order.** Peleg's ordering. Not yet the stage list — Step 3 can still
reshape it and Step 5 turns it into stages with deliverables and commit points. Levels 1 and 2 are
both scratch testing grounds until item 14; nothing before that is the level anyone will play.

1. Level lifecycle and level data; player side movement; player jump; player attack as a log stub.
   One item, because a level with nobody walking on it shows nothing. The attack stub goes in early
   so the input plumbing exists before there is a weapon to argue about.
1. Camera, with an inspector toggle between following the player and framing the whole level. The
   second mode is for testing and the first is the shipped behaviour. Bounds clamping belongs here
   rather than with level 2, since level 1 has edges too and 11.4 says the view never leaves the
   playable area.
1. State model and game flow — the three scopes above, plus start-level, lose-פסילה, next-level and
   game-over as real operations. Driven by a debug key at this point, since nothing can kill you yet,
   which is the right pressure: losing a פסילה has to be an event anything can raise, not something a
   hazard's collision code performs by hand.
1. כוח and its bar. The first real cause of a פסילה, so it validates item 3 immediately.
1. Popups and restart. Placed here, not at the end, because item 4 is the first point where the whole
   loop can be run end to end: die, lose a פסילה, level resets, die twice more, game over, popup,
   restart, session state clears. That proves the no-scene-reload architecture before any content is
   built on top of it, and it is the riskiest requirement in the project. The level-2 completion popup
   cannot exist yet and is added at item 13 as a second listener on machinery that already works.
1. Fruit and the counter. Closes the כוח economy, including the every-20 rule.
1. Hazards — the pit, then fire, then the rock with its knockback.
1. Weapons and projectiles, with the pool. Tested against rocks, which is a real test: the boomerang
   destroys them and the axe does not. Pickups hand-placed, since eggs do not exist yet.
1. Enemies — the shared lifecycle first, then the six behaviours.
1. Animals, with tokens hand-placed for the same reason.
1. Eggs, drops and collectibles.
1. פייה. Destroys everything except the pit, so it cannot be finished until everything else exists,
   which makes it the integration test for the whole game.
1. Level 2's **mechanics**: vertical camera framing, the level-to-level transition, and the completion
   popup. Built on a scratch level 2. The transition has to be generic — no `if (level == 1)` in it.
   The argument for that at the defense is open/closed, not extensibility, since a third level is
   `[out]`.
1. Author both levels to spec, against finished mechanics.

#### Step 2 — The level pipeline, what travels, and level 2's shape `[x]`

**The pipeline stays exactly as Mario's.** Tiled builds the first draft of a map because that is
easier there, it exports to a Tiled JSON file, and every edit after that happens through the in-editor
tools because re-exporting from Tiled is tiring. Tiled is therefore `[good]` and stays in the picture,
as a tool that is pleasant to use rather than anything the exercise asks for.

**Level 2 needs no format change.** `TiledMap` is width, height and a flat row-major `data` array, and
`LevelWindow.PlaceLayer` turns an index into a cell with `column = i % width` and
`row = (height - 1) - (i / width)`. Nothing in it assumes a wide level. Level 1 is 30x20 and level 2
is something like 20x60, and that is the whole of it.

**One layer, as Mario had.** `Build` reads every layer in a file but `Save` writes them back as a
single `"Tile Layer 1"`, so a multi-layer Tiled import survives exactly until the first edit in the
Tile Placer, then collapses and drops whatever shared a cell. Given the workflow above, that collapse
would certainly happen. It costs nothing here because nothing in this game needs two things in one
square: ground, platforms, fruit, eggs, enemies and hazards each want their own cell anyway, and the
background is a flat colour or a parallax image rather than tiles.

**The level is built at edit time, not at Play.** Peleg's call, against the alternative argued below.
The tool writes the level into the scene, the scene is saved with it, and nothing reads the level file
while the game runs — the same arrangement all three Mario exercises used.

The alternative was building the level at Play from the same file, which would have given Builder a
real job and Async an honest one: `File.ReadAllTextAsync` does real I/O off the main thread and returns
a value, neither of which a coroutine can do. It was turned down as the less steady of the two, and
the first argument for it was overstated in the discussion and withdrawn: building from the file covers
level start, but **not** death, because 3.4 says dead enemies stay dead and a rebuild would resurrect
them. The partial reset on death is hand-written under either choice. Building at edit time costs only
the full-reset routine, and it costs Async its home in the pipeline.

Two consequences that follow from it:

- **Both levels live in the one scene**, since there is no `LoadScene` to bring the second one in.
  Two root objects, `Level_1` and `Level_2`, each built by the tool from its own file, one active at a
  time. The tools take the parent as a field per build, so this needs no change to them.
- **Async & Tasks has to live somewhere else**, and Step 3 decides where. The candidate to beat is the
  popup and transition flow from item 5: `await popup.ShowGameOverAsync()` returns a value, which a
  coroutine cannot do without a callback or a shared field, and a Task survives the transition
  disabling the objects a coroutine would have been running on. The timers — the פייה's ten seconds,
  respawn countdowns, throw cooldowns — are all bad candidates, because a coroutine is the right tool
  for those and the instructor would be right to say so.

**What travels, unchanged.** None of the eleven files mentions Mario, coins, or anything from that
game. `LevelWindow` says it "knows no tile ids of its own" and that holds: the ids live in a
ScriptableObject, so a new tile type is a row in an asset rather than an edit to any tool.

- Tooling: `LevelWindow.cs`, `TilePlacerWindow.cs`, `TilePrefabMap.cs`, `TiledMap.cs`,
  `SceneObjectMemory.cs`, `LogsWindow.cs`.
- Logging: `GameLog.cs`, `LogCategory.cs`, `LogLevel.cs`, `LogSettings.cs`, `LogFileWriter.cs`.

`LogCategory`'s seven values — Player, Enemy, Weapon, Projectile, Pickup, Tile, Game — all apply to
this game unchanged, which is the only place in the eleven where game flavour could have crept in.

**What Stage 2 has to make by hand rather than copy:**

- `Level_1` and `Level_2` root GameObjects. Both tools default their parent name to `World`, so either
  that default changes or the field gets pointed at the right root per build.
- A **new, empty** `TilePrefabMap.asset`. Mario's is a table of GUID references to Mario's prefabs and
  every one of them would dangle. Copying the folder would bring a silently broken asset across.
- One world unit must equal one tile, because `TilePlacerWindow` draws its cell cursor as a 1-unit
  cube and places on integer coordinates. Step 4 settles that as 48px sprites at PPU 48.

#### Step 3 — The stage list, and what each stage needs `[x]`

Peleg's method, and the right one: write the stages first, then walk each one asking what would
genuinely help it, rather than starting from the list of seven and finding somewhere to put each. A
pattern placed because the exercise named it is the answer that loses marks at an oral defense.

Three things go in as givens rather than being rediscovered:

- **DI is not a placement.** It is how every class receives what it needs, so it is settled before the
  first stage rather than found in one of them. Logic lives in plain C# classes with
  constructor-injected interfaces; MonoBehaviours are thin adapters that forward Unity's events into
  them; one `GameInstaller` wires the lot. Decided late, it would mean rewriting every stage before it.
- **Pooling goes on the thrown projectiles**, because the instructor asked for it there by name
  (00:35:02).
- **Async goes on the enemy respawn timer**, because a killed enemy is deactivated and Unity stops
  coroutines on a disabled GameObject — an enemy cannot run its own respawn countdown as a coroutine
  at all. Lesson 5's own `EnemySpawner.cs` is `await Task.Delay(interval, cancellationToken)` in a loop,
  sitting deliberately beside `PlayerInvincible.cs`, which uses a coroutine for a gameplay timer.

The fourteen items from Step 1 become stages 6 to 19, which makes N = 21.

##### Stage 6 — The level, and the player moving in it

1. Scene skeleton: `Level_1` and `Level_2` roots, `SceneContext` with `GameInstaller`, `LogSettings`.
1. The eleven files from Step 2 brought across, plus a `TilePrefabMap` asset holding the first few
   tiles.
1. A scratch level 1 — enough ground to walk on, built through the normal Tiled-then-tools path so the
   pipeline is proven early.
1. Player movement on the arrow keys.
1. Player jump on `Space`, height varying with how long it is held.
1. Attack on `Z`, writing a log line and nothing else.

**What it needs: nothing from the seven, deliberately.** There is no logic here worth extracting from a
MonoBehaviour — reading input and pushing a `Rigidbody2D` is exactly what a MonoBehaviour is for, and an
`IInputService` injected into the player would be textbook DI with nothing behind it, since this game
never swaps input implementations and there are no tests to fake one for. The installer exists from this
stage and starts nearly empty. Saying that out loud at the defense is stronger than pretending
otherwise.

##### Stage 7 — Camera

1. Follow the player, clamped so the view never leaves the level (11.4).
1. An inspector toggle between following and framing the whole level, the second for testing.

Exercise 3 has a `CameraFollow.cs`, but it is gameplay code rather than tooling and it has neither the
clamp nor the toggle, so this is written fresh.

##### Stage 8 — State model and game flow

1. `SessionState` (פסילות, fruit count), `CarriedState` (weapon, animal), `LevelState` (כוח, which
   enemies are dead, which collectibles are taken, player position). Plain C# classes behind interfaces.
1. `GameFlow` with the real operations: `StartGame`, `StartLevel`, `LoseStrike`, `CompleteLevel`,
   `GameOver`. Driven by a debug key for now.
1. `IResettable`, and a reset that walks everything registered.
1. Events for `StrikeLost`, `GameOver` and `LevelComplete`, raised here and listened to later.

**What it needs: DI, and one interface that carries the whole stage.** `IResettable` is the design: a
collectible, an enemy, the player and the כוח model each register, and the reset walks the list. A full
reset restores everything; a partial reset restores everything except enemies, per 3.4 and 3.5. Adding a
new resettable kind later touches no reset code, which is the open/closed answer.

**A pattern deliberately rejected here.** Full and partial reset look like a Template Method with two
subclasses, and it would be one boolean of real difference. A flag on one method is simpler and honest;
a base class and two subclasses for one branch is pattern-for-its-own-sake and exactly what the SOLID
check would find. Worth saying at the defense that it was considered and dropped.

##### Stage 9 — כוח and its bar

1. `PowerModel` — current, maximum 16, level start 11, drain 1 per 2 seconds.
1. `PowerController` — ticks the drain, applies fruit, caps at maximum, raises "empty".
1. `PowerView` — the bar.
1. Empty raises `LoseStrike` through the flow's event rather than by calling it directly.

**What it needs: MVC**, in the same shape as Exercise 2's coin counter with its `ICoinsModel` and
`ICoinsView`, which he has already seen and accepted. The controller is a plain C# class ticked through
Zenject's `ITickable`, so no MonoBehaviour is involved at all — which is the Clean Architecture line the
course keeps making, demonstrated rather than asserted.

##### Stage 10 — Popups and restart

1. Game Over popup with a restart button.
1. Restart: session cleared, carried cleared, back to level 1, no scene load.
1. The פסילות display.

**What it needs: Async, second home.** `await popup.ShowAsync(...)` returns which button was pressed. A
coroutine cannot return a value — it needs a callback or a shared field — and a `TaskCompletionSource`
bridging a button click to an `await` is short enough to read on screen in the code video. Different
justification from the respawn timer, so the two uses give two answers rather than one repeated twice.

This stage is placed early on purpose: it is the first point the whole loop can run end to end — die,
lose a פסילה, level resets, die twice more, game over, popup, restart, session clears — which proves the
no-scene-reload architecture before any content is built on it.

##### Stage 11 — Fruit and the counter

1. `Fruit_1` and `Fruit_2`, adding 1 and 2 כוח.
1. The fruit counter, and a פסילה at every multiple of 20 (4.7).
1. The counter display.

**What it needs: the collectible base, and MVC again.** Eight things in this game are picked up by
touching them — two fruit, three animal tokens, two weapons, the פייה — and all eight share detect, apply,
consume, notify, with only the apply step differing. That is Template Method by its nature rather than by
decision. The counter is the second MVC triad.

##### Stage 12 — Hazards

1. תהום: falling below the level costs a פסילה.
1. מדורה: touch costs a פסילה; only the פייה destroys it.
1. אבן: touch costs 3 כוח, knocks the player forward, brief immunity for the length of the shove.

**What it needs: `IDestructible`, and it is the best idea in the plan.** Every object in this game answers
a different version of one question: what is allowed to destroy me? The אבן says boomerang, animal or פייה
but not axe (5.3). The מדורה says פייה only. Five of the six enemies say axe, boomerang, animal or פייה;
the רוח רפאים says פייה only. One interface answered per type turns "I don't want to see the axe destroy
the rock" into data instead of conditionals scattered across every projectile, and it is the same
interface for hazards and enemies.

The riding rule lives in one place too: the player's damage entry point checks whether an animal is being
ridden, and if so dismounts and destroys the hazard instead of applying it (5.5, 5.8, 7.10). One rule, one
site, rather than repeated in every hazard.

##### Stage 13 — Weapons and projectiles

1. `BaseProjectile` and its fixed lifecycle.
1. `ProjectileAxe`, thrown in an arc; `ProjectileBoomerang`, returning to the player's current position.
1. `ProjectileBuilder` and `ProjectileDirector`.
1. A projectile pool taking a prefab rather than one class per projectile type.
1. The weapon slot, the throw cooldown, the cap on how many are in flight (6.7).
1. All of it wired through the installer.

**What it needs: Builder, Pooling, Template and DI at once**, which is the Lesson 9, 10 and 12 chain
applied to this game's projectiles. Five recipes really differ — axe, boomerang, snake fireball, red
animal fire, and the arc and return behaviours between them — which is more variety than Exercise 3's
single laser had. Exercise 3's `LaserPoolManager` handled one type; four copies of that class is exactly
what the SOLID check finds, so the pool takes a prefab instead.

##### Stage 14 — Enemies

1. The `Enemy` base: spawn, behave, take damage, die, drop, wait, return.
1. עכביש, ציפור, jumping נחש, shooting נחש, צפרדע, רוח רפאים.
1. The respawn timer.
1. The shooting נחש's projectile, reusing stage 13's pool and builder.
1. Each enemy's `IDestructible` answer.

**What it needs: Template, Async and Pooling.** Six subclasses sharing one fixed lifecycle where only
`Behave` and the destructible answer vary is the strongest Template in the project. The respawn timer is
the Async home, for the reason above.

##### Stage 15 — Animals

1. Mount by collecting לב, עלה or כוכב; swap when a second token is taken while riding.
1. Blue hits low with its tail, red spits fire higher, green spins in place.
1. Absorb one hit, then both the animal and what hit it disappear.
1. Carry into level 2.

**A pattern deliberately rejected here.** Composing the mounted state — sprite, attack behaviour, attack
height, the absorb rule — looks like a second Builder. It is four fields, and a ScriptableObject per
animal says the same thing with less machinery and stays editable without recompiling. Builder keeps one
strong home in stage 13 rather than a weak second one here, which is the same call Exercise 3 made.

##### Stage 16 — ביצים, drops and collectibles

1. The drop factory, making a collectible from a configured type.
1. Per-enemy drop configuration, including dropping nothing (8.3).
1. Eggs, opened by stepping on them, contents configured per egg (10.2, 10.3).
1. **Reflection**: a `[Drop(DropType.Heart)]` attribute on each collectible class, scanned once when
   the container is built and cached in a `Dictionary<DropType, Type>`.

**What it needs: Factory, with two callers.** An enemy or an egg says *what*; the factory knows *how* to
build a לב, a boomerang or a פייה, and neither caller ever learns. Both requirements say the choice is
authored data rather than a roll, which is precisely a factory taking a configured type.

**And Reflection, which is `[good]` rather than required.** Without it this factory holds the one
switch statement the whole design admits, and every new drop type edits it. With the attribute and one
cached scan at startup, a new drop type is a new class and the factory is never opened again —
open/closed demonstrated instead of asserted, for about thirty lines and a single scan. The instructor
said he meant to put Reflection on the list and would be glad to see it (00:54:18); Exercise 2 parked
it and Exercise 3 dropped it, so this is the third time of asking.

**The escape hatch is deliberate.** If this stage arrives and the schedule is tight, write the switch
and move on. Nothing else in the project depends on which way this goes, which is exactly why the
reflection lives here rather than somewhere structural.

##### Stage 17 — פייה

1. Ten seconds of invincibility.
1. Destroys everything it touches, מדורה and רוח רפאים included.
1. תהום still kills (9.3), and a level needs a place to demonstrate exactly that (9.4).

**Deliberately a coroutine, not a Task.** The פייה's ten seconds run on a MonoBehaviour that stays alive
for the whole duration, needs no cancellation token and returns nothing, so a coroutine is the right tool
and a Task would be worse. Lesson 5 makes exactly this contrast in one project: `EnemySpawner.cs` on
Tasks beside `PlayerInvincible.cs` on a coroutine. Having both, and being able to say why each is where
it is, answers the question he said outright he will ask — in both directions.

This stage also touches every other system, so it doubles as the integration test.

##### Stage 18 — Level 2's mechanics

1. Vertical camera framing.
1. The transition, with no level-number branch anywhere in it.
1. The completion popup, a second listener on stage 10's machinery.
1. Carried state crossing: weapon, animal and fruit count survive; כוח resets.

##### Stage 19 — Author both levels to spec

1. Level 1 in Tiled, exported, then edited through the tools.
1. Level 2 the same, climbing bottom to top.
1. Satisfy the כוח arithmetic: the fruit needed to cross a level against a פסילה every 20.
1. Place drops and eggs so every type is reachable in the opening half-minute (10.4).
1. The פייה-then-תהום spot he said he will look for.

#### Step 4 — Animation scope `[x]`

**Two frames for anything that moves, a real cycle for the player, everything else static.** The
instructor made animation optional and blessed the two-frame flip by name, with one condition —
"אבל שזה יהיה מובן" (00:24:01). The enemy sheet already has those two frames side by side, so the
second one is nearly free to cut. The player is the exception because the camera is on him for the
whole gameplay recording, and he is the one place a frozen sprite is conspicuous. A rock, a fruit, a
tile and a HUD icon all read as finished without moving.

What the sheets actually hold, checked rather than assumed:

- `123.png` is the player: walk, jump, throw and death frames, and the animals **with the player
  already drawn on them**. Riding is a single combined sprite, so there is no layering, no per-animal
  offset and no sorting order to get wrong.
- `208311.png` is the enemies, labelled by name, two or three frames each. Spider, Snake and Frog are
  all there. No ghost, confirming 8.24. Solid green background, so this is the sheet the keying
  method has to handle.
- `29903.png` is the Adventure Island **2** animals, four of them at five frames each, each beside
  its pickup token. Different game's art from `123.png` and the rider is not drawn in, so mixing the
  two would look wrong. Use `123.png`.

Two notes for Stage 3 that came out of looking:

- The original's tokens are card suits, which is why the transcript says "אם אני לוקח את הקלף יהלום
  או כוכב" (00:37:05). Requirement 7.2 says לב, עלה and כוכב. The heart exists on `29903.png`; the
  other two need sourcing from elsewhere.
- **Sprites are stored at 48px per grid cell and Pixels Per Unit is 48 for every one of them.** The
  source art is drawn on a 16px grid, upscaled 3x with nearest-neighbour, so one cell is stored as
  48x48 and is one world unit. A creature 16x24 stores as 48x72 and occupies 1 by 1.5 units; an
  animal with its rider at 32x32 stores as 96x96 and occupies 2 by 2. `TilePlacerWindow` draws a
  1-unit cell cursor and places on integer coordinates, so one cell has to be one world unit, and
  normalising every sprite to one unit with a per-sprite PPU would break that grid.

  **The 16 was measured, not assumed.** It was first written down as "the source art is 16x16", from
  a single transcript line (00:28:40) and NES convention, and repeated three times before anyone
  checked. Flood-filling the non-background regions of three sheets gives: `208311.png` enemies at a
  median 16x21, most commonly 16x16 then 16x24 and 16x20; `123.png` player and animals at a median
  28x32, most commonly 16x32 and 32x32; `29903.png` at a median 29x27. Widths cluster on 16 and 32
  with nothing between them. So 16 is the grid unit and sprites are multiples of it - a tile is one
  cell, the player is 1x2, an animal with its rider is 2x2. Saying "the art is 16x16" would have had
  Stage 3 cutting the player in half.

  Peleg's call, against a first draft that kept the art at 16px and set PPU 16. The two render
  identically; 48 wins on workflow, because cutting from a 3x-upscaled sheet is far more forgiving
  than cutting 16px cells, and it keeps the number this project family has used since Exercise 1.
  The 16 now appears nowhere as a setting - it only describes the source material.

Roughly 59 sprites: 7 player on foot, 9 riding, 12 enemies, 6 projectiles, 8 collectibles, 2 egg, 3
hazards, 8 tiles, 4 UI. All-static would be about 40, so the animation scope costs 19 extra cuts —
an hour or two with a repeatable keying method, not a weekend.

**Animator on the player only; a shared sprite-swap component everywhere else.** The player has real
states driven by what he is doing. Everything else is an array of sprites and an interval, about
twenty lines, covering all six enemies, the מדורה, the animals and the boomerang from one script.
An Animator controller per prefab would mean states and transitions for twenty-odd objects, and the
lesson's own flower does it the cheap way.

#### Step 5 — Coverage check `[x]`

All seven accounted for, with the sentence to say when he asks why it is there. Two are flagged as
placed rather than earned, on purpose — being able to name them is better than being caught by them.

**DI — the whole project.** `GameInstaller : MonoInstaller` inside a `SceneContext`, binding the
state model, the game flow, the power controller, the projectile builder and director, the pool and
the drop factory. *"Logic is in plain C# classes and MonoBehaviours are adapters. The installer is
the only place that knows which implementation goes where."* The likely probe is whether this is real
DI or inspector wiring relabelled, and the proof is the same one lesson 12's own write-up makes: the
injected fields are private, carry no `[SerializeField]`, and never appear in the scene YAML at all.

**Pooling — thrown projectiles.** *"Axes are thrown in bursts with a cap on how many are live, so
they are created and destroyed constantly. Exercise 3's pool handled one projectile class; four
copies of that class would be the worst spot in this codebase, so this one takes a prefab."*

**Builder — projectile construction, with a Director holding the recipes.** *"A projectile has six
settings and there are five recipes. The Director is where 'an axe is this, a boomerang is that'
lives, so a new projectile is a new recipe rather than another six-argument constructor call."* The
probe to expect is "isn't this the laser again" — it is the same home, and the answer is that the
laser had one recipe where this has five, and the pool is no longer one class per projectile type.

**Factory — drops and egg contents, two callers.** *"8.3 and 10.3 both say what drops is configured
and never rolled, so an enemy holds a drop type and an egg holds a drop type, and neither knows how
to build a פייה."* The probe to expect is "why not just a prefab reference on the enemy", and the
honest answer is that several drop types need setup past instantiation — a token has to know which
animal it mounts, a weapon has to register in the slot, the פייה carries its duration — and the
caller should not learn which ones do.

**Template — three hierarchies: `BaseProjectile`, `Enemy`, `Collectible`.** *"Enemy is the clearest.
Spawn, behave, take damage, die, drop, wait, return — six subclasses, and only Behave and the
destructible answer vary. A subclass cannot reorder those steps, which is the point: a new enemy type
cannot forget to respawn."* The strongest of the seven and the one to lead with.

**MVC — three HUD triads: כוח, fruit count, פסילות.** *"Same shape as the coin counter in Exercise 2.
The model holds the number and its rules, cap at 16 and never below zero. The view only draws. The
controller is ticked by Zenject and never touches a MonoBehaviour."* **Flagged: only כוח earns it.**
The fruit count and the פסילות count are a number and a label, and the full triad on them is
consistency rather than necessity. That consistency is still worth defending, because the SOLID check
grades the worst spot and two different HUD idioms in one project is exactly the kind of
inconsistency it finds — but say it that way rather than claiming all three were equally needed.

**Async & Tasks — enemy respawn, and the popup flow.** *"A killed enemy is deactivated, and Unity
stops coroutines on a disabled GameObject, so an enemy cannot run its own countdown as a coroutine at
all — it would have needed a separate manager holding timers on its behalf. A Task is not tied to the
object's lifetime, and one CancellationToken cancels every pending respawn when the level resets."*
And for the popup: *"await returns which button was pressed; a coroutine returns nothing."* The
answer in the other direction, which is the one he said he asks: *"the פייה's ten seconds is a
coroutine, because it runs on an object that stays alive throughout, needs no cancellation and
returns nothing. A Task there would be worse."*

**Two patterns deliberately rejected**, recorded because a rejection with a reason defends better
than an application without one: Template for the full-versus-partial level reset, which is one
boolean of real difference; and Builder for the mounted-animal state, which is four fields and is
better as a ScriptableObject per animal.

**Reflection is placed at stage 16, and stays `[good]`.** The drop factory resolves a configured drop
type to its class by attribute rather than by a switch, removing the one switch statement the design
would otherwise need. It was first written down here as "reconsider at Stage 20 if there is room",
which was the wrong place: Stage 20 is after everything, so adding it there means opening finished
code. At stage 16 it is written with the factory or not at all, and dropping it costs nothing because
nothing else depends on it.

##### The SOLID risk register

He runs an automated check and grades the worst thing it finds, not the average (00:52:01), so the
worst spot is worth predicting rather than discovering.

- **The player is the god-object risk.** Movement, jump, attack, damage, immunity, mounting, weapon
  slot and כוח all want to live on him. Split from the first stage the way Exercise 3 already did —
  `PlayerMovement`, `PlayerJump`, `PlayerDeath`, `PlayerInvincible` were separate components there —
  rather than splitting a 400-line class later.
- **A switch over enemy types** would undo the Template. Behaviour belongs in the subclass, and what
  can kill a thing belongs in its `IDestructible` answer, so neither needs a type check.
- **A switch over drop types** in the factory is the one switch the design admits, and stage 16's
  Reflection removes it. If that gets dropped for time, this becomes the switch to expect a question
  about.
- **The three MVC triads have to be the same shape.** Two idioms for one HUD is a visible
  inconsistency in a place he will definitely look.

### Stage 2 — Project and repo setup `[x]`

- Create `2026-HW_Final-Adventure_Island` as a fresh Unity project. Not a copy of Exercise 3.
- Check the scene and the Build Settings.
- Bring across the eleven files named in Stage 1 Step 2, and assign what `Tools > Level`,
  `Tools > Tile Placer` and `Tools > Logs` need.
- Create by hand rather than copy: `Level_1` and `Level_2` root objects, and a **new, empty**
  `TilePrefabMap.asset`. Mario's asset is a table of GUIDs pointing at Mario's prefabs and every one
  would dangle.
- **The Zenject compile test**, before any project code depends on it. Copy `Assets/Plugins/Zenject/`
  out of `Lesson 12.zip`, delete `OptionalExtras` (321 of its 577 `.cs` files, all samples and test
  suites), and confirm Unity 6 compiles the remaining 256. If it does not come up clean, switch to
  VContainer rather than spending an evening on it — see the Decisions Log.
- `.gitignore`, `.gitattributes`, `README.md` and `CONVENTIONS.md` at the project root. All four have
  to be copied by hand; copying `Assets` leaves every one of them behind, which cost three files at
  the start of Exercise 3.
- `git init` and a clean starting commit.
- Move this plan file into the project once the folder exists.

### Stage 3 — Sprites `[x]`

Done. **82 sprites in `Assets/Sprites/`**, against the 59 Stage 1 estimated: 21 riding, 15 fruit, 14
enemies, 9 player on foot, 4 weapons, 4 tiles, 3 tokens, 3 hazards, 3 destroy puffs, 2 egg, 2 UI, the
פייה and the exit door. The riding count is where the estimate was thinnest - it budgeted 9 and each
animal needed idle and jumping frames on top of its walk pair and attack.

Every requirement has its art: all six enemies of 8.1, both weapons, the three animals of 7.1 with
their tokens, the אבן and מדורה of section 5, the פייה, the ביצה in both states, the exit door, both
fruit tiers and the two HUD pieces of section 12.

**The pipeline is three scripts in `Tools/`**, outside `Assets/` so Unity never compiles them:

- `prepare_sheets.py` reads the zip, clears each sheet's background, upscales 3x nearest-neighbour,
  and writes each sheet twice - once with real alpha, once with the background filled in a colour it
  has proved absent from that sheet's art. The second copy is what gets cut from, because Paint
  cannot be trusted to preserve transparency.
- The cutting itself is by hand, in Paint, using Crop rather than copy-paste. Crop was measured to
  give byte-identical output at every tightness; pasting drops the sprite onto a white canvas and
  eats any white in the art that touches the edge.
- `finish_sprites.py` clears the background, trims to the art, and pads out to whole 48px cells.
- `cut_tiles.py` takes the ground tiles by coordinate instead, for the reason in the log below.

`Tools/Cut/` is tracked - it is hand work and cannot be regenerated. `Tools/SheetsReady/` and
`Tools/Ready/` are gitignored, since the scripts rebuild both exactly.

`SpriteImportRules.cs` in `Assets/Scripts/Editor/` holds the import settings as a rule rather than a
default: PPU 48, Point filtering, no compression, Full Rect, no mipmaps. It runs on every import, so
a sprite whose settings get changed by hand goes back on the next reimport.

**Left for Stage 4:** the pivot. `finish_sprites.py` bottom-aligns art in its box, so Bottom-Center
would put a creature's feet on the cell boundary, while a tile probably wants Center. Which is right
depends on how `TilePlacerWindow` positions what it places, so `SpriteImportRules.cs` deliberately
says nothing about it and Unity's default stands.

### Stage 4 — The level pipeline `[x]`

**Scope narrowed in Stage 1.** This stage builds the pipeline and a scratch level to test it with. It
does not author the levels anyone will play: a real level can only be built against finished
mechanics, so that is item 14 of the development list, at the end. Levels 1 and 2 are both testing
grounds until then.

- Two levels. Level 1 runs left to right; level 2 climbs bottom to top.
- The gameplay reference the instructor pointed at: level 1 from 2:04 of the gameplay video, level 2
  from 7:15.

**Tiled is out**, which reverses Stage 1 Step 2 and the third sub-step of Stage 6. The file format
stays Tiled's on purpose, so the decision is reversible; the reasoning is in the log below.

#### Steps

1. The pivot, in `SpriteImportRules.cs`. `[x]`
1. `SceneObjectMemory` resolving a hierarchy path instead of a leaf name, and both tool windows
   defaulting to `Level_1` rather than Mario's `World`. `[x]`
1. Six sorting layers, and the eight prefabs the level file holds that need no script yet. `[x]`
1. `TilePrefabMap` filled in with ids 1 to 8. `[x]`
1. `Level01.txt`, a scratch level 1, and the round trip proved in both directions. `[x]`
1. `Build` made incremental, so a rebuild leaves a matching object alone instead of replacing it.
   `[x]`
1. Drag-to-paint in `TilePlacerWindow`. `[x]`
1. `TiledMap.cs` renamed to `LevelMap.cs`, since the format is no longer read from Tiled. `[x]`
1. `Level02.txt` and a scratch level 2, tall and narrow, to prove the row flip on a vertical map.
   `[x]`
1. `CONVENTIONS.md` updated with the pivot rule, the sorting layers and the incremental rebuild.
   `[x]`

**Watch for this.** `SpriteImportRules.cs.meta` was found holding a copy of the script rather than
YAML, pasted there by mistake at the end of Stage 3. Unity had been ignoring the asset ever since,
silently — the sprites kept the settings from the one run before it broke, so nothing looked wrong
until a new rule failed to apply. Deleting the `.meta` and letting Unity rebuild it was the fix.

### Stage 5 — Re-read the plan `[ ]`

Before any game code. Check that Stages 0 to 4 actually hold together, that every required technique
has a home, and that nothing in the stage list depends on something later in it.

### Stages 6 to 19 — Game development `[ ]`

Defined in Stage 1, Step 3. Each stage there lists its
sub-steps and what it actually needs, rather than which pattern it was assigned.

### Stage 20 — Final testing, comments and log check `[ ]`

A full playthrough of both levels covering every requirement in one session, plus a pass over
comments and log lines against `CONVENTIONS.md`. Same shape as Exercise 3's Stage 7, but larger.

Worth remembering here: the instructor runs an automated SOLID check over the submitted code and
grades against the worst thing it finds, not the average.

### Stage 21 — Two video scripts `[ ]`

**Two separate recordings this time**, which is new. The instructor watches the gameplay one first,
writes down the problems he sees, then watches the code one, and builds the defense questions from
both.

- One recording of gameplay only, showing every feature working.
- One recording of the code, explaining each part.
- Several short recordings per item are allowed instead of one long take.
- If the files are too large for Moodle, a download or YouTube link is acceptable, and it has to stay
  live.

Modeled on `2026-HW_3-Mario/HW_3-Script.md`: spoken lines in block quotes, stage directions outside
them, every requirement called out in Hebrew, one take per part, and every part written to a measured
word count. The rates from three recorded videos: about 160 spoken words a minute where the camera is
on a file, about 90 where it is on the game.

## Notes / Decisions Log

_(append entries here as we make design decisions.)_

- **Requirements are numbered `section.item` rather than flat.** The previous three videos called out
  `דרישה 4:` matching the exercise text's own numbered items, but the final project's text has no
  numbering at all, so any scheme here is ours. Flat 1 to 45 was the alternative and it loses: the
  plan and later the video script both cite these IDs, this document will get amended as the
  instructor answers questions, and inserting one requirement into a flat list invalidates every
  citation after it. Two-level confines a renumber to one section.
- **English body, Hebrew for the words that reach the video.** כוח, פסילה and the object and enemy
  names stay in Hebrew so the requirements document and the spoken script use the same word. Full
  Hebrew was the alternative; it would make the script a copy-paste job but make the document harder
  to work from for four weeks. Peleg's call.
- **No source tags and no text-versus-transcript table.** An earlier proposal had every requirement
  marked with where it came from, plus a table of the disagreements. Peleg cut both: it's a
  requirements document, not an audit. Transcript timestamps survive only where the transcript
  overrides the written text or where the defense is likely to ask, which is about thirty of them.
- **Four tiers, with "required" as the unmarked default.** `[good]`, `[nice]` and `[out]` mark the
  exceptions; everything untagged is required. Tagging every line as required would have been noise.
  The `[out]` items are also collected in one section, because four weeks of scope pressure makes a
  written list of what you're allowed to skip worth as much as the requirements.
- **The argument against extra content is the SOLID check, not his patience.** He never says extra
  work loses marks, and Exercise 3's unasked-for boomerang cost nothing. What he does say (00:52:01)
  is that his automated checker reports the single worst spot in the code and he grades on that. More
  code is more surface for that one spot to be in. That reasoning is in the document.
- Tool and pipeline choices stay out of `Exercise Adventure Island.md` and stay here. Tiled is
  recorded there only as the fact that he doesn't require it; whether this project uses it is still
  Stage 1's.
- **Zenject for DI. Tested and confirmed on 1.9.2026** — 574 files extracted, compiled on Unity 6
  with two `CS0618` deprecation warnings and no errors, and a `SceneContext` with an empty
  `GameInstaller` ran `InstallBindings` end to end at Play. The deprecations are
  `Object.FindObjectsOfType<T>()` at `UnityUtil.cs:130` and `ProjectContext.cs:104`; the API is
  deprecated rather than removed, Zenject's assembly only recompiles if its own files change, and an
  unmodified copy is worth more at the defense than a patched one, so both are left alone.
  VContainer is off the table. The reasoning that led here:
- **Zenject for DI, gated on a compile test in Stage 2.** Lesson 12's project ships Extenject 9.2.0 as
  source under `Assets/Plugins/Zenject/`, and its `GameInstaller : MonoInstaller` inside a
  `SceneContext` is the arrangement the instructor built on camera, so he recognises it instantly and
  spends the defense judging the design rather than parsing an API. The risk is the version gap: that
  project is Unity 2022.3.62f2 and this family is Unity 6000.5.6f1, and Extenject is a 2022-era library
  that is no longer actively developed. So Stage 2 copies it in, deletes `OptionalExtras` (321 of its
  577 `.cs` files, all samples and test suites, and the usual source of version breakage), and confirms
  Unity 6 compiles the remaining 256. If it does not come up clean, switch to VContainer without
  spending an evening on it: VContainer is maintained, installs through UPM so it lands outside
  `Assets/` and out of the submission entirely, and the requirement says "DI" in both sources, never
  "Zenject". The test happens before any project code depends on the choice.
- The repo-root `CLAUDE.md` was rewritten for the final project before Stage 0 rather than during
  Stage 2, per Peleg. It loads automatically at the start of every session, and left alone it would
  have told each new conversation that Exercise 3 was the live project and pointed every path there.
  Exercise 3's own Stage 0 updated it as a setup step; here the project folder doesn't exist yet, so
  it names the folder as forthcoming and points at this plan instead. It carries the no-scene-reload
  rule and the seven required techniques, so both survive a fresh session with no prompting.
- This plan is a skeleton by design. Peleg's call: get the stage shape down now, write a fresh
  opening prompt, and do the real planning in Stage 1 with a clear head. The alternative was one long
  session that produced both the requirements document and the full plan, and the final project is
  large enough that the second half of that session would have been worse than the first.
- The plan file starts at the repo root rather than inside the project, because Stage 2 is what
  creates the project. It moves into `2026-HW_Final-Adventure_Island/` at the end of Stage 2 and
  becomes `HW_Final_PLAN.md` there, matching where `HW_2_PLAN.md` and `HW_3_PLAN.md` live.
- **`Exercise Adventure Island.md` lives at the project root, beside this plan.** It was first put
  in `Course/Exercises/` beside `Exercise 03.md` and the text it restates, on the reasoning that it
  describes the assignment rather than the code. Overturned on 1.9.2026 for a reason that did not
  exist then: the `2026` folder is not a git repo, so anything left there has no history, no branches
  and no backup. Once the plan moved into the project, leaving the requirements outside would have
  versioned the two halves of the same work differently, with the more valuable half unversioned.
  The cost is that Exercise 3's convention of keeping requirements out of the repo is broken.
- **"Start fresh" and "we want the tools" pull against each other, and this is not resolved.** The
  editor tools and the logging system are the most valuable code in the Mario project and were built
  over three exercises: `LevelWindow`, `TilePlacerWindow`, `TilePrefabMap`, `SceneObjectMemory`,
  `TiledMap`, `LogsWindow`, `GameLog` and its four supporting files. Bringing them means copying
  parts of `Assets/`, which is what "fresh" was meant to avoid. Stage 1 decides which named files
  travel; the useful distinction is that tooling and logging are infrastructure this project would
  otherwise rebuild from scratch, while gameplay code is what the exercise is actually grading.
- No `SceneManager.LoadScene` anywhere, including on death and on game over. The instructor said this
  three separate times in lesson 12, unprompted, and named it as a thing juniors do that he marks
  down: a reload hides static state that never gets reset, and it stops working the moment the scene
  is loaded from somewhere else. Death resets the level's own variables and moves the player to the
  level start. This is a departure from all three Mario exercises, which reload on game over.
- Enemy drops should be configurable per enemy rather than hardcoded or purely random. The
  instructor's own reason is testability: he wants to be able to see every drop type without playing
  for an hour, and the original game has no randomness in it at all. The same argument covers eggs.
- Reflection is not required. The instructor said he meant to include it, decided not to add it after
  the fact, and would be glad to see it. Exercise 2 parked reflection and Exercise 3 dropped it; this
  is the first place it would be welcome rather than invented, so it is worth reconsidering once
  Stage 1 knows how much room there is.
- **Backgrounds are cleared globally, not flood-filled from the border.** The obvious method is to
  flood inward from the edge, and it is wrong twice over. The rippers left background sealed inside
  sprite outlines where a flood can never reach: two shades of khaki across 149 pockets on
  `123.png`, a purple box behind one frame, and the green boxes holding the effects on `29903.png`.
  And a flood cannot tell a solid-coloured sprite from a background at all - it consumed the כוח bar
  whole and stripped the frame off the exit door. Every colour was checked against all sixteen
  sheets' art before being added to the clear list; the pale pink that looks equally out of place is
  the pink animal's belly, and clearing it would have gutted four sprites.
- **Frames of one character share a box size.** Padding each frame to its own extent makes a
  character slide sideways when the Animator changes state - an idle frame one cell wide beside a
  throwing frame two cells wide moves him half a unit. Effects and projectiles are excluded, since
  they spawn at their own position and padding a 24px fireball into the rider's 2x4 box would leave
  its pivot in empty space. The two HUD sprites are left unpadded entirely: the 48px cell means
  nothing on a Canvas. The cost is transparent padding, which is atlas space and nothing else.
- **Tiles are cut by coordinate, not by hand.** No sheet in the zip is a tileset, so the ground comes
  out of the boss-room screenshots. A tile has to be exactly 48x48 with no transparent pixel in it,
  and the first hand-cut set was 18 source px wide and cut on a block boundary, which left a hole at
  every corner once the tiles were laid edge to edge. Three earth tiles kept, each from a different
  boss room. The last sentence of this entry used to say the texture repeated every 32px, so that a
  run of one tile showed a visible grid and two had to be mixed to hide it. Measured in Stage 4 that
  is not true of the coordinate-cut set: none of the three has any period smaller than 48px in
  either axis, and a run of one tiles cleanly. The observation belonged to the hand-cut set this
  entry replaced.
- **Spikes are optional and were cut anyway.** The only mention is 00:50:15, "אתם יכולים גם לעשות
  שיהיה קוצים למטה בחלק מהמקומות. עוד תהום" - offered as a variant of תהום rather than a
  requirement. One tile, and it gives level 2 a hazard that is not a bottomless fall. Clouds appear
  in neither source; the one apparent match in the transcript is inside another word.
- **The רוח רפאים is the white bat from `88429.gif`.** The instructor said on camera he could not
  extract one (8.24), and he is right that no sheet has a Boo. The bat reads as a ghost, is in the
  material he provided, and answers better at the defense than a downloaded Mario asset would.
- **The red animal's token is the spade.** 7.2 asks for לב, עלה and כוכב; the original uses card
  suits. The heart and star exist outright, and the spade is the closest thing in the sheets to a
  leaf. Sourcing or drawing a leaf was the alternative and it buys nothing.

- **The sprite pivot is the centre of a sprite's bottom cell**, set as a custom pivot of
  `(0.5, 24 / textureHeight)` and clamped to `0.5` for anything shorter than a cell. One integer
  coordinate then means the same thing for a 1x1 tile as for a 3x4 animal, and a creature's feet
  land on a cell boundary whatever its height. Unity's default Center was the alternative and it
  fails on parity: art is bottom-aligned in a whole number of cells, so an odd-height sprite lands on
  a boundary and an even-height one lands half a cell into the floor - the מדורה, the door, both
  נחשים and the red and blue mounts, five of the things that stand on the ground. Bottom-Center was
  the other candidate and it works, at the cost of an asymmetric grid, a `Floor` on Y against a
  `Round` on X in the placer, and a collider offset on all fourteen one-cell prefabs that this way
  gets for free. The question could not arise in Exercise 3: all 46 of its sprites were 48x48.
- **The level file holds everything, and `Build` is a diff rather than a teardown.** Six of the
  things placed in a level carry per-instance configuration - the spider's range, the bird's speed,
  dip and wavelength, the drop on every enemy, the ביצה's contents - and Mario's `Build` destroyed
  every child and recreated it from a file that stores one int per cell, so a rebuild would wipe all
  of it. Two alternatives were weighed: splitting the hierarchy into `Tiles` and `Objects` and
  keeping the configured objects out of the file, and keeping Mario's `Build` with a confirmation
  dialog. The split costs a complete record and a second place to look; the dialog leaves a rebuild
  restoring a level where every bird flies straight, which is worse than not restoring because it
  looks right. Indexing the existing children by cell and comparing against the file costs about
  forty lines and removes the hazard instead of warning about it. It also keeps `LevelWindow`'s
  open/closed property intact, since the comparison is on position and prefab identity and adds no
  type knowledge.
- **Tiled is dropped.** Stage 1 Step 2 kept it for first drafts and Stage 6 wanted the path proven
  early; both are reversed. `TilePlacerWindow` already previews better than Tiled can, because it
  stamps the real prefab at real size against the real camera framing, and this project's placeables
  are 1x1 up to 3x4 where Tiled would show a 48px thumbnail for all of them - the one thing worth
  seeing while authoring is the thing it gets wrong. Stage 1 had already ruled out round-tripping
  through Tiled, so it was only ever going to run once per level, and for that one run it brings a
  tileset atlas to assemble, the CSV-versus-Base64 export trap and the flip-bits-in-the-GID trap. Its
  one real advantage is bulk terrain, and the answer to that is drag-to-paint in our own tool rather
  than a second application. **The file format stays Tiled's**, so the decision costs nothing to
  reverse.
- **Tile ids are contiguous, grouped by kind in order.** 1 the start marker, 2 to 4 the earth tiles,
  5 to 8 spikes, אבן, מדורה and the door, 9 to 14 the six enemies, 15 the ביצה, 16 and 17 the two
  fruit tiers, 18 to 23 the weapons, tokens and the פייה. Banding with gaps was the first proposal
  and its only argument was Tiled's consecutive GIDs, which stopped mattering when Tiled went. The
  cost of no gaps is that a placeable added later appends rather than slotting into its group, since
  renumbering would mean rewriting every level file by hand.
- **A per-instance value is a field when it gets passed on and a subclass when it gets branched on.**
  The ביצה holds a `DropType` and hands it to the factory without ever asking what it is, so one
  prefab. The spider's static-or-moving is a `moveRange` float where zero means static, which needs
  no branch at all and is how 8.6 and 8.9 are worded. The two נחשים are two prefabs and two
  subclasses: one runs stand-hop-stand on a timer and the other tracks the player, fires, and needs a
  stop condition, so an `AttackType` enum would be tested every frame and half the serialized fields
  would be dead on each instance. That is the switch over enemy types the SOLID risk register
  predicts, and it would take Template's demonstration from six subclasses to five.
- **The fruit's look is a prefab override, not a field.** The fifteen fruit sprites are fifteen
  different fruits rather than animation frames, but 4.3 makes the value a property of the tier and
  the tier is the prefab, so nothing in the game reads which one it is. An enum, a sprite array and
  an index would exist only to reproduce a field the `SpriteRenderer` already has, and the override
  is protected by the incremental rebuild like any other per-instance value.
- **No `CompositeCollider2D` on the ground.** Adjacent `BoxCollider2D`s leave internal faces that a
  box-shaped character catches on, and the composite removes them for good. Exercise 3 never met the
  problem because Mario had a `CircleCollider2D` of radius 0.45 on a 1x1 sprite - he was a ball, and
  a frictionless one. That approach transfers with the shape changed: the player's body measures 1 by
  2 units inside its 2x3 box, so a vertical capsule gives the same rounded bottom with coverage that
  matches, and the frog's body is a genuine 1x1 where Mario's circle fits exactly. A composited
  collider also raises its events on the composite's GameObject rather than the tile, and ticking
  `Used By Composite` on a prefab dropped under a root that has no composite leaves it with no
  collision at all, silently. Adding one later is two components and one checkbox with no change to
  the file, the tools or any other prefab, so the reversible option goes first.
- **The אבן is a trigger, not a solid.** 00:17:49: "מעיפים אותו מהאבן... איך שהוא מגיע באבן הוא טס
  קדימה. ואז אם יש אש או משהו כזה, הוא מת" - the player enters the rock and is flung forward across
  it, into whatever is on the far side. A blocking collider stops him at its face, so there is
  nothing to be flung across and nothing to land in. 5.2 already says he is inside the collider while
  being pushed.
- **Six sorting layers, back to front: `Background`, `Level`, `Pickups`, `Enemies`, `Player`,
  `Effects`.** Unity's `Default` is left first and unused, so a prefab whose layer was forgotten
  renders behind the ground and shows itself. `Level` rather than `Tiles` because the door and the
  מדורה are not tiles, and `Pickups` split from it so a fruit lying in the same cell as scenery reads
  as collectable. Exercise 3 needed none of this: every sprite was one cell and nothing overlapped.
- **`SceneObjectMemory` remembers a hierarchy path.** `GameObject.Find` matches on the leaf name and
  skips inactive objects, so with one level deactivated it answered with the other level's object,
  and with two identically named children it answered with whichever it reached first. Walking down
  from `SceneManager.GetActiveScene().GetRootGameObjects()` fixes both. Editor tooling is not exempt
  from the SOLID check - it ships in the submission like everything else under `Assets/`.
