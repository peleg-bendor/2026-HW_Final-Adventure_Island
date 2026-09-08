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
| 6 to 19    | Game development. One section each; the reasoning is Stage 1, Step 3 |
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
   was expected to wait for item 13; the exit door made the ending reachable here, so both popups were
   built together instead.
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

**Reversed in Stage 4.** Tiled is out, and levels are painted in `Tools > Tile Placer` instead. The
file format stays Tiled's so the choice costs nothing to reverse, and the reasoning is in the
Decisions Log. Everything else in this step still holds.

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

The fourteen items from Step 1 become stages 6 to 19, which makes N = 21. Each of them is written
out in its own section further down this file, in sequence with every other stage, rather than nested
under this step. What each one needs is recorded there beside what it does, since the two are read
together and a stage's own section is where anyone looks.

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
an hour or two with a repeatable keying method, not a weekend. Stage 3 cut 82; this estimate was
thinnest on the riding frames, which needed idle and jumping on top of a walk pair and an attack.

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

**Builder — projectile construction, with a Director holding the recipes.** *"A projectile is a few
numbers and there are four recipes. The Director is where 'an axe is this, a boomerang is that'
lives, so a new projectile is a new recipe rather than another multi-argument constructor call."* The
probe to expect is "isn't this the laser again" — it is the same home, and the answer is that the
laser had one recipe where this has four, and the pool is no longer one class per projectile type.
**Counted at stage 13 and corrected here: four, not five.** The projectiles in this game are the axe,
the boomerang, the נחש's fireball and the red animal's fire; the arc and the return are behaviours in
subclasses rather than recipes of their own. Two of the four exist before stage 15, so the sentence
is only fully true from there.

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
object's lifetime, and each enemy's own CancellationToken cancels its pending respawn when the level
resets and again when the object is destroyed."*
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

### Stage 5 — Re-read the plan `[x]`

Before any game code. Check that Stages 0 to 4 actually hold together, that every required technique
has a home, and that nothing in the stage list depends on something later in it.

**All seven techniques kept their homes.** Stage 4 wrote only editor code and assets, so nothing
moved. The stage order holds too, with one intended near-miss: stage 12 defines `IDestructible`
before stage 13 exists to prove "boomerang yes, axe no", which is what Step 1's ordering meant.

**Stages 6 to 19 moved out of Step 3** into their own sections in sequence with every other stage,
since a stage's own section is where anyone looks for it. Step 3 keeps the reasoning that produced
them.

**Four real findings, all now folded into the stages above.** Stage 6 still listed the scene
skeleton, the eleven files and the scratch level, all of which Stages 2 and 4 had already done, so it
shrank to the player alone. Nothing created the player object at all, which left Stage 4's capsule
and the `Frictionless` material homeless. Nothing produced a level's extent, which stage 7's clamp,
stage 8's start position and stage 12's תהום all need - that became `LevelDefinition`. And nothing
owned the start marker, which became `PlayerStart`.

Five smaller ones: Tiled survived in Stage 19 and read as current in Step 2, the fruit had two names,
Step 4's sprite estimate was stale against Stage 3's 82, and Stages 18 and 19 lacked the "what it
needs" paragraph every other stage carries. One correction with nothing to fix: the מדורה's two
frames come from Step 4's shared sprite-swap component, not from an Animator.

### Stage 6 — The player moving in the level `[x]`

Stages 2 and 4 built the scene, the tools, the prefabs and a scratch level 1, so what is left here is
only the player himself.

1. The player object: `Sprite_Player_Idle` on a `Rigidbody2D`, a vertical `CapsuleCollider2D` 1 by 2
   because that is what his art measures inside its 2x3 box, and
   `Assets/Physics/Frictionless.physicsMaterial2D`, created in this stage because his collider is
   what needs it. He is one object for the whole game rather than a child of either level, since the
   weapon, the animal and the fruit count all cross the level boundary with him. `[x]`
1. Player movement on the arrow keys. `[x]`
1. Player jump on `Space`, height varying with how long it is held. `[x]`
1. Attack on `Z`, writing a log line and nothing else. `[x]`
1. The player's Animator: idle, walk, jump, throw and death, driven by what the movement and jump
   components already know rather than by a second read of the keyboard. `[x]`
1. A tuning pass with the animation on, since that is the first point where the numbers can be
   judged rather than guessed: the capsule width, the jump height and cut, the brake. `[x]`
1. Cut every comment in the project back to the revised rules, across the seven editor tools, the
   five logging files and `GameInstaller`. A mechanical pass, reviewed as a diff. `[x]`

**What it needs: nothing from the seven, deliberately.** There is no logic here worth extracting from a
MonoBehaviour — reading input and pushing a `Rigidbody2D` is exactly what a MonoBehaviour is for, and an
`IInputService` injected into the player would be textbook DI with nothing behind it, since this game
never swaps input implementations and there are no tests to fake one for. The installer exists from this
stage and starts nearly empty. Saying that out loud at the defense is stronger than pretending
otherwise.

Ground tiles carry plain box colliders with no composite above them, so the capsule's rounded bottom
is what stops him catching on the seams between them. Exercise 3 got the same effect from a circle
collider, which worked there only because Mario was one cell tall.

**The Animator is here because this is the only stage where a frozen sprite would be conspicuous.**
Step 4 of Stage 1 settled the scope and no stage carried the work; the player takes it here, at the
end rather than the start, because a walk cycle judged against movement that still feels wrong tunes
the wrong thing. The shared sprite-swap component that covers everything else is not built here - it
waits for the מדורה in stage 12, which is the first thing other than the player that moves. The rule
that keeps the Animator out of the SOLID register: parameters are pushed by whoever already owns the
fact, so `PlayerMovement` supplies speed and facing and `PlayerJump` supplies grounded and vertical
velocity. A `PlayerAnimator` that read the keyboard again to decide he is walking would be a third
copy of the input logic.

### Stage 7 — Camera `[x]`

1. `LevelDefinition`, a component on `Level_1` and `Level_2` holding the level's width and height in
   cells. With the level root at the origin and Stage 4's pivot, the playable rect runs from `-0.5`
   to `width - 0.5` across and `-0.5` to `height - 0.5` up. No fall line: every pit has a floor.
1. `PlayerStart` on the `Sprite_Player_Start` prefab, which answers where the player begins and hides
   its own sprite when the level starts. Found among the level's children rather than referenced by
   field, since a serialized `Transform` would dangle the first time a rebuild replaced the marker.
1. Follow the player, clamped so the view never leaves the level (11.4), with the player sitting a
   quarter of the way up the view rather than centred.
1. An inspector toggle between following and framing the whole level, the second for testing.

Exercise 3 has a `CameraFollow.cs`, but it is gameplay code rather than tooling and it has neither the
clamp nor the toggle, so this is written fresh. It finds the player through a serialized reference
rather than by tag, because Stage 6 made him one object for the whole game - the tag lookup existed
in Exercise 3 only because Mario was rebuilt with the level and an Inspector reference would dangle.

**What it needs: nothing from the seven, again deliberately.** Reading one transform and writing
another is what a MonoBehaviour is for. `LevelDefinition` behind an interface would be an interface
with one implementation and no second caller, and injecting the camera would be DI over a
`[SerializeField]` that already works. The installer stays nearly empty until stage 8.

**Why the size is authored rather than measured.** Nothing at runtime otherwise knows how big a level
is, because the level file is not read at Play. Measuring the children's renderers was the first
answer and it describes the art rather than the level: the scratch level 2's tiles fill 15 columns of
a 32-wide grid, so the camera would clamp to a strip. Reading the file's own width and height was the
second, and `Save` grows the grid without ever shrinking it, so those numbers are a high-water mark -
that same level 2 file says 32x45 for something painted 15 by 22. The argument that settles it is
that a level shorter than the view cannot contain the camera at all, so the clamp rect has to be at
least the view's size whatever the tiles do. That makes the level's size a decision rather than a
measurement, and decisions get written down.

### Stage 8 — State model and game flow `[x]`

1. `SessionState` (פסילות, fruit count), a plain C# class. No `LevelState`: everything it was going
   to hold is owned by the object that already has it, and `IResettable` is what restores them.
   `CarriedState` waits for stage 13 - there is no weapon type and no animal type to declare a field
   as yet, so it is unwritable rather than merely thin.
1. `GameFlow` with the real operations: `StartGame`, `LoseStrike`, `CompleteLevel`. Driven by the
   `1`, `2` and `3` keys for now. `GameOver` is an event rather than an operation, since nothing may
   end a game except running out of פסילות, and entering a specific level is internal.
1. The level transition, generic over any number of levels. `LevelDefinition` carries a
   `levelNumber`, `GameFlow` scans once including inactive roots and orders by it, and the only
   branch is whether a next level exists. Neither ending restarts on its own: 2.3 and 2.4 both put a
   button between the ending and the next game, so both raise their event and stop.
1. The player returns to the `PlayerStart` of the current level's `LevelDefinition` on both resets,
   per 3.3 - as his own `IResettable`, rather than as something the flow does to him, and facing
   whichever way the marker faces. The marker is drawn at 40% alpha so a level can be authored
   against where the player will actually stand, and it hides itself the moment the level starts.
1. `IResettable`, and a reset that walks everything registered.
1. Events for `StrikeLost`, `GameOver`, `LevelComplete` and `GameComplete`, raised here and
   listened to later. The last two are separate because 2.3's congratulation popup has to know the
   difference between finishing a level and finishing the game.

**What it needs: DI, and one interface that carries the whole stage.** `IResettable` is the design: a
collectible, an enemy, the player and the כוח model each register, and the reset walks the list. A full
reset restores everything; a partial reset restores everything except enemies, per 3.4 and 3.5. Adding a
new resettable kind later touches no reset code, which is the open/closed answer.

**A pattern deliberately rejected here.** Full and partial reset look like a Template Method with two
subclasses, and it would be one boolean of real difference. A flag on one method is simpler and honest;
a base class and two subclasses for one branch is pattern-for-its-own-sake and exactly what the SOLID
check would find. Worth saying at the defense that it was considered and dropped.

### Stage 9 — כוח and its bar `[x]`

1. `PowerModel` — current and maximum, with `Add` reporting how much actually landed so 4.4's
   wasted fruit can be told from a real gain.
1. `PowerController` — a plain C# class on Zenject's `ITickable`. Ticks the drain, applies fruit,
   caps at the maximum, and stops while the game is over.
1. `PowerView` — the bar, built at startup as one `Sprite_Strength_Line` per unit of capacity, so
   sixteen hand-made children cannot fall out of step with the number.
1. Empty calls `LoseStrike` on the flow rather than touching `SessionState`, so the strike
   bookkeeping stays in one place.
1. `LevelDefinition` gains the level's starting כוח, and `GameInstaller` gains the four numbers that
   describe rules rather than things.

**What it needs: MVC**, in the same shape as Exercise 2's coin counter with its `ICoinsModel` and
`ICoinsView`, which he has already seen and accepted. The controller is a plain C# class ticked through
Zenject's `ITickable`, so no MonoBehaviour is involved at all — which is the Clean Architecture line the
course keeps making, demonstrated rather than asserted.

### Stage 10 — Popups and restart `[x]`

1. A `Player` marker component, and the exit door of 2.2: touching it calls `CompleteLevel` on the
   flow. Small, and it is what replaces the debug key, so the whole loop can be played rather than
   typed.
1. Both popups in one step, since the second is the first duplicated: the Game Over popup of 2.4 and
   the congratulation popup of 2.3, the latter moved forward from stage 18 now that the door makes
   the ending reachable here. `Popup` holds the wait, `Popups` maps a game event to a scene object,
   and `GameFlow` awaits `IPopups`. `Time.timeScale` to 0 while either is up and back to 1 at the top
   of `StartGame`, which deletes `PowerController`'s `running` flag and its two subscriptions. A
   private guard in the flow, so a debug key pressed under a popup cannot start a second sequence.
   Restart clears the session and re-enters level 1 with no scene load.
1. The פסילות display: one `Sprite_Strike` icon and a count, as the original draws it.

**What it needs: Async, second home — and the original justification for it was wrong.** The first
draft of this stage said "await returns which button was pressed; a coroutine returns nothing". Both popups have exactly one
button, 2.3's and 2.4's alike, so there is no *which* and that answer collapses the moment it is
pushed on. The real difference is structural: **a coroutine needs a MonoBehaviour to run on**, and the
code that waits here is `GameFlow`, a plain C# class, where a coroutine is not awkward but impossible.
A `TaskCompletionSource` bridging a button click to an `await` is still short enough to read on screen.
And it is a different answer from stage 14's respawn timer, which is "a disabled GameObject stops its
coroutines" - two uses, two reasons, which is what having two homes was for.

This stage is placed early on purpose: it is the first point the whole loop can run end to end — die,
lose a פסילה, level resets, die twice more, game over, popup, restart, session clears — which proves the
no-scene-reload architecture before any content is built on it.

### Stage 11 — Fruit and the counter `[x]`

1. Three refactors first, none of them this stage's own work: `IPower`, `ResetRegistry` and
   `Levels`. All three touch `GameFlow`, which this stage had to edit anyway.
1. `Fruit_1` and `Fruit_2` of 4.3, built as the prefabs `Sprite_Fruit_Common` and
   `Sprite_Fruit_Super`, adding 1 and 2 כוח. The requirement's names are what the video says; the
   prefab names are what the tile map holds.
1. The fruit counter, and a פסילה at every multiple of 20 (4.7).
1. The counter display: `Count_Fruit`, stage 10's `Count_Strikes` widget with a different icon,
   turning red for the five fruit before the next פסילה. `ISessionState` was decided here and built:
   both displays read the session and neither writes to it, while `GameFlow` needs `LoseStrike` and
   `Restart`.

**What it needs: the collectible base, and MVC again.** Eight things in this game are picked up by
touching them — two fruit, three animal tokens, two weapons, the פייה — and all eight share detect,
consume and come back, with only the apply step differing. That is Template Method by its nature
rather than by decision. The counter is the second MVC triad.

### Stage 12 — Hazards `[x]`

1. תהום: a pit with a floor of spikes, which cost a פסילה on contact and are the one thing the
   פייה does not destroy.
1. מדורה: touch costs a פסילה; only the פייה destroys it.
1. אבן: touch costs 3 כוח, knocks the player forward, brief immunity for the length of the shove.
1. The shared sprite-swap component, an array of frames and an interval. The מדורה is the first
   thing other than the player that moves, and the same component then covers the six enemies, the
   three animals and the boomerang.

**What it needs: `IDestructible`, and it is the best idea in the plan.** Every object in this game answers
a different version of one question: what is allowed to destroy me? The אבן says boomerang, animal or פייה
but not axe (5.3). The מדורה says פייה only. Five of the six enemies say axe, boomerang, animal or פייה;
the רוח רפאים says פייה only. One interface answered per type turns "I don't want to see the axe destroy
the rock" into data instead of conditionals scattered across every projectile, and it is the same
interface for hazards and enemies.

The riding rule lives in one place too: the player's damage entry point checks whether an animal is being
ridden, and if so dismounts and destroys the hazard instead of applying it (5.5, 5.8, 7.10). One rule, one
site, rather than repeated in every hazard.

### Stage 13 — Weapons and projectiles `[x]`

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

### Stage 14 — Enemies `[ ]`

Six steps, one enemy each, because each is one prefab, one tile id and one `Behave` and there is
nothing to be gained by testing two at once. The base and the respawn timer arrive with the first
of them; every later step is a subclass and a prefab. The six take **tile ids 11 to 16**.

1. The `Enemy` base and its fixed lifecycle, and עכביש. Static or moving is one prefab and a
   `moveRange` where zero means static (8.7). This step is where the respawn timer, the contact
   rule and both reset scopes are settled and tested.
1. ציפור: leftward at a constant speed while dipping and rising, with speed, dip and wavelength
   per instance so a dip of zero flies straight (8.9). Switches itself off at the level's left
   edge (8.11).
1. נחש, the jumper: stand, hop forward, stand, repeat (8.12).
1. נחש, the shooter: fires while the player is in range and stops when he leaves it, which is
   8.14 and 8.15 answered by the same number. Its fireball is a fourth recipe in
   `ProjectileDirector`, a fourth `BaseProjectile` subclass and a fourth entry in the pool.
1. צפרדע: jumps higher and further than the נחש, toward the player, on a randomized interval
   (8.17, 8.18).
1. רוח רפאים: static while the player faces it, chases when his back is turned, stops past its
   range (8.20, 8.22). The one enemy that answers `Fairy` alone, and the one whose two sprites are
   states rather than animation frames.

**What it needs: Template, Async and Pooling.** Six subclasses against one fixed sequence with four
hooks — `OnAwake`, `Behave`, `DestroyedBy` and `OnSpawned` — is the strongest Template in the
project, and it comes out the same shape as `BaseProjectile`'s `Fly`, `OnHit` and `OnLaunched`, which
answers better than two hierarchies that merely happen to both be base classes. A subclass cannot
reorder the steps, so a new enemy type cannot forget to respawn. The respawn timer is the Async home,
for the reason above: a killed enemy is deactivated, and Unity stops coroutines on a disabled
GameObject.

### Stage 15 — Animals `[ ]`

1. Mount by collecting לב, עלה or כוכב; swap when a second token is taken while riding.
1. Blue hits low with its tail, red spits fire higher, green spins in place.
1. Absorb one hit, then both the animal and what hit it disappear.
1. Carry into level 2.

**A pattern deliberately rejected here.** Composing the mounted state — sprite, attack behaviour, attack
height, the absorb rule — looks like a second Builder. It is four fields, and a ScriptableObject per
animal says the same thing with less machinery and stays editable without recompiling. Builder keeps one
strong home in stage 13 rather than a weak second one here, which is the same call Exercise 3 made.

### Stage 16 — ביצים, drops and collectibles `[ ]`

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

### Stage 17 — פייה `[ ]`

1. Ten seconds of invincibility.
1. Destroys everything it touches, מדורה and רוח רפאים included.
1. תהום still kills (9.3), and a level needs a place to demonstrate exactly that (9.4).

**Deliberately a coroutine, not a Task.** The פייה's ten seconds run on a MonoBehaviour that stays alive
for the whole duration, needs no cancellation token and returns nothing, so a coroutine is the right tool
and a Task would be worse. Lesson 5 makes exactly this contrast in one project: `EnemySpawner.cs` on
Tasks beside `PlayerInvincible.cs` on a coroutine. Having both, and being able to say why each is where
it is, answers the question he said outright he will ask — in both directions.

This stage also touches every other system, so it doubles as the integration test.

### Stage 18 — Level 2's mechanics `[ ]`

1. Vertical camera framing.
1. Carried state crossing: weapon, animal and fruit count survive; כוח resets.
1. Level 2 authored far enough to prove the transition end to end.

**What it needs: nothing new, and that is the point.** The camera comes from stage 7, the popup
machinery from stage 10, the carried state from stage 13 and the transition itself from stage 8;
this stage only wires them to a second level. **The transition moved out of here into stage 8**,
where `CompleteLevel` had to become a real operation rather than an event that fired and did
nothing - and once it advances the level, it may as well be generic from the start. What is left
here is level 2's own mechanics, which is what the stage was named for.

### Stage 19 — Author both levels to spec `[ ]`

1. Level 1 painted in `Tools > Tile Placer`, against mechanics that are finished by now.
1. Level 2 the same, climbing bottom to top.
1. Satisfy the כוח arithmetic: the fruit needed to cross a level against a פסילה every 20.
1. Place drops and eggs so every type is reachable in the opening half-minute (10.4).
1. The פייה-then-תהום spot he said he will look for.

**What it needs: no code at all.** This is authoring, and it is where the requirements that only
level design can satisfy get satisfied: the כוח arithmetic, every drop type reachable early, and the
place he said he will go looking for.

### Stage 20 — Final testing, comments and log check `[ ]`

A full playthrough of both levels covering every requirement in one session, plus a pass over
comments and log lines against `CONVENTIONS.md`. Same shape as Exercise 3's Stage 7, but larger.

Worth remembering here: the instructor runs an automated SOLID check over the submitted code and
grades against the worst thing it finds, not the average.

Also decided here: whether `DebugFlowKeys` ships. Keys `1` and `2` force a strike and a level
completion, which is development speed rather than anything the game needs, and they let anyone
holding the build skip the game. Nothing forbids them; the question is only whether they belong in a
submission.

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
  unmodified copy is worth more at the defense than a patched one, so neither line is touched. They
  are silenced instead, in stage 8, by `Assets/Plugins/Zenject/Zenject.rsp` holding `-nowarn:0618`.
  Zenject already ships its own asmdef, so a response file named for that assembly scopes the
  suppression to the library and leaves our own obsolete-API warnings reporting - which is what a
  project-wide `csc.rsp` would have swallowed. The file has to be named for the **assembly**,
  `Zenject`, not for the lowercase `zenject.asmdef` beside it. The cost is that a deprecation inside
  Zenject that later becomes a removal arrives as a compile error with no warning first.
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
- **Tile ids are contiguous and append in the order things were built.** Banding with gaps was the
  first proposal and its only argument was Tiled's consecutive GIDs, which stopped mattering when
  Tiled went. The cost of no gaps is that a placeable added later appends rather than slotting into
  its group, since renumbering would mean rewriting every level file by hand - and that cost has
  already been paid once. The original plan was 9 to 14 for the six enemies, 15 the ביצה, 16 and 17
  the fruit; the fruit arrived at stage 11 before any enemy existed, so `TilePrefabMap.asset` now
  reads 1 the start marker, 2 to 4 the earth tiles, 5 to 8 spikes, אבן, מדורה and the door, and
  **9 and 10 the two fruit tiers**. Everything after that appends as it is built: the weapon pickups
  at stage 13, then the enemies, the ביצה, the tokens and the פייה. The asset is the record; this
  entry is not.
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

- **A level's size is authored on the level root, not derived.** `LevelDefinition` on `Level_1` and
  `Level_2` holds width and height in cells and a fall line, and stage 7's camera clamp, stage 8's
  `StartLevel` and stage 12's תהום all read it. Nothing at runtime otherwise knows a level's extent,
  since the file is only read at edit time. Measuring the children's renderers describes the art
  instead of the level - scratch level 2's tiles fill 15 columns of a 32-wide grid - and reading the
  file's own width and height inherits a high-water mark, because `Save` grows the grid and never
  shrinks it, which is why that same file says 32x45 for a level painted 15 by 22. The deciding
  argument is that a level shorter than the camera's view cannot contain it at all, so the clamp rect
  has to be at least the view's size regardless of where the tiles stop. That makes the size a design
  decision, and the cost is two numbers per level kept in step by hand.
- **The flow waits on the popup, which is what makes the Task earned rather than decorative.**
  `GameFlow` takes an injected popup interface and awaits it, so the sequence *game over, wait for
  the player, restart* sits with the other game rules instead of in a UI script. The alternative was
  a MonoBehaviour presenter listening to the `GameOver` event, which is tidier and has no
  fire-and-forget anywhere - and a coroutine would do it identically, which is exactly the problem.
  Async is one of the seven and the instructor said outright he asks "why not a coroutine", so tidiness
  is the wrong thing to buy here. **The price is real and worth saying out loud:** everything that
  calls `LoseStrike` is synchronous - `PowerController.Tick` today, every hazard's trigger later - so
  none of them can await, and the flow starts the sequence without blocking. That is fire-and-forget,
  and swallowed exceptions are its known cost.
- **`Time.timeScale` goes to 0 while a popup is up.** One line stops `FixedUpdate`, so movement and
  jumping stop with no edit to either component, and it zeroes `Time.deltaTime`, so stage 9's drain
  stops on its own - **which lets `PowerController`'s `running` flag and its two event subscriptions be
  deleted**. A fix that removes code. The alternative was the `Playing`/`GameOver`/`Won` state this log
  predicted stage 10 would want, and it loses on the same ground it was proposed on: every
  input-reading component would have to check it, which is one condition duplicated across three
  classes today and more with every stage, and it would not stop physics anyway. The known risk of a
  global is a path that forgets to restore it, so `StartGame` sets it back to 1 unconditionally at the
  top and a restart always unfreezes whatever happened.
- **A `Player` marker component identifies him to triggers, not a tag.** The door is the first thing
  that needs to ask "was that the player", and every hazard, collectible, token and egg needs the same
  thing, so it is decided once here. A tag is the familiar answer - Exercise 3 used one, `CompareTag`
  is the documented fast path - and it loses on three counts: it is a magic string, it is set in the
  Inspector and so can be silently lost when an object is recreated, and **most callers need the player
  object rather than the fact**, so a tag hands them a `GameObject` they immediately `GetComponent` on
  anyway. The project defines no tags at all today, so the marker also avoids adding a project setting
  that has to be right in the submission.
- **The strikes display is an icon and a number, not a row of icons.** Checking the original: it draws
  a face sprite followed by a count, not one icon per life. So `StrikesView` is a static `Image` beside
  a `TextMeshProUGUI`, which is Exercise 2's `HealthView` shape and structurally nothing like
  `PowerView`'s repeated children. The near-duplicate this was going to be extracted for does not
  exist. Where it does appear is stage 11: the fruit counter is also an icon and a number, so that is
  the second use case and the place a shared view would be earned.

- **The exit door had no stage, and lands in stage 10.** Requirement 2.2 - a level ends when the
  player reaches a cave door - was never assigned to anything. Grepping the plan finds the door only
  as art in stage 3, as a prefab and tile id 8 in stage 4, and as a trigger in the composite-collider
  entry; no stage ever made it do anything. Stage 10 is the right home because it is the stage whose
  point is running the whole loop end to end, and the door is what turns `CompleteLevel` from a debug
  key into something the player does. It is small - a trigger that calls one method on the flow -
  which is presumably why it slipped through: every other requirement of that size hangs off a
  larger one, and this one hangs off nothing.

- **The bar's proportions were measured off the original, and its size has to divide.**
  `Sprite_Power_Line.png` is a 3x16 source upscaled 3x - one pixel of olive border, one of cream
  fill, one of border across - so a rendered width that is not a multiple of 3 gives a five-pixel
  border on one side and four on the other, and a height that is not a multiple of 16 does the same
  top to bottom. Measuring the NES screenshot put the bar at about 22% of the screen's width and 4.2%
  of its height with gaps roughly as wide as the lines; the first attempt was three times too tall
  and packed too tightly. Settled at a 30x96 line with 24 of spacing, which keeps both divisors and
  runs the bar to 840px on a 1920 reference - twice the original's share of the width, which reads
  better on a modern screen than a faithful 420 would.
- **The Canvas scaler moved from Constant Pixel Size to Scale With Screen Size, 1920x1080 at match
  0.5.** The template left it measuring the HUD in real screen pixels, so a bar tuned on one monitor
  would be a different size in a build, in the Game view at another aspect, and in the recording that
  gets graded. Caught before the bar was tuned rather than after.
- **The view builds its own lines rather than being handed sixteen children.** `PowerView`
  instantiates one line per unit of the capacity it is given, so the bar cannot fall out of step with
  a changed capacity - sixteen hand-placed children would silently disagree the moment the number
  moved. An empty slot switches off the `Image` rather than the GameObject, so it keeps its place in
  the row and the bar stays the same width however full it is.

- **The כוח controller is a plain C# class on Zenject's `ITickable`.** Exercise 2's
  `HealthController` was a MonoBehaviour and he graded it, so this goes further on purpose: the
  controller has no MonoBehaviour anywhere in it, which demonstrates the course's "logic out of
  MonoBehaviours" line rather than asserting it. The risk is that `Lesson 12.md` shows he used only
  `Bind/To/AsSingle`, `FromComponentInHierarchy` and `[Inject]` - `ITickable` was never on camera. It
  works here without setup because `SceneContext` binds a `SceneKernel` onto itself with `NonLazy`,
  and that MonoBehaviour's `Update` drives `TickableManager.Tick`. Worth being able to say that
  sentence, because "what calls Tick" is the obvious question. **And worth claiming accurately:** the
  controller still reads `Time.deltaTime`, so it is MonoBehaviour-free, not Unity-free. Making it
  Unity-free needs an injected clock with one implementation and no tests, which is the same thing
  stage 6 refused for `IInputService`.
- **A level's starting כוח is authored on the level, not shared.** Peleg's call, wanting level 2 to
  open at 9 against level 1's 11. 4.1 reads as one shared number until you notice the instructor
  treating it as a difficulty dial at 00:15:06 - "ואז יש שלבים שהקושי שלהם זה לתת לך ממש קצת זמן
  בהתחלה" - so per level is the closer reading, not a departure. It lands on `LevelDefinition` beside
  the width, height and level number for the same reason those are there, and the controller reads
  `CurrentLevel.StartingPower` on reset. `EnterLevel` activates the new root before it walks the
  resettables, so the level is already the new one when כוח is restored.
- **The four rule numbers stay as fields on `GameInstaller`, reversing an argument for a settings
  asset.** The case for a `GameSettings` ScriptableObject rested on two claims and both failed on
  checking. "A dozen more numbers are coming" was wrong - grepping the requirements, every bracketed
  value except these four describes a *thing* rather than a *rule*, so it belongs on that thing's
  prefab: the אבן's 3 כוח, the פייה's 10 seconds, each fruit's value, every enemy speed and range. And
  "scene diffs would be noisy" was hollow, because levels are edited in this scene constantly and
  `Scene_Game.unity` already churns on nearly every commit. What was left - swapping a whole
  difficulty asset - nothing asks for. So `startingStrikes`, `powerCapacity`, `drainSeconds` and
  `fruitPerStrike` are four serialized fields on the installer, and the line that keeps them from
  growing is that a number describing a rule goes here while a number describing an object goes on
  the object. Promoting them to an asset later is twenty minutes and touches one file.
- **The drain stops while the game is over.** Not obvious until it runs: a controller that keeps
  ticking calls `LoseStrike` again three seconds after the game-over screen, on a session with zero
  strikes, which clamps and re-raises `GameOver` on a loop forever. It stops on `GameOver` and
  `GameComplete` and starts again when its own `ResetTo` runs. The larger version is a real game state
  on the flow - `Playing`, `GameOver`, `Won` - which stage 10 will want anyway so a popup can stop
  input; generalising the four-line version then costs nothing, and building it now would be
  speculation.

- **The start marker answers which way the player faces, by its own flip.** First written as a
  hardcoded `FaceRight()` on the reset, which is true of both levels and still wrong: where he faces
  is a property of the start, not of the code that puts him there. A `facesRight` field on
  `PlayerStart` was the obvious fix and it can disagree with what the authoring aid shows. The marker
  uses `Sprite_Player_Idle` as its sprite, so it is already a translucent picture of the player -
  reading `transform.localScale.x` means flipping the marker in the Scene view both sets the facing
  and displays it, with nothing that can drift apart. Per-instance scale survives the incremental
  rebuild like any other hand-set value.
- **The camera's snap is a flag consumed in `LateUpdate`, not work done inside `ResetTo`.** The reset
  walks its registrations in whatever order things happened to register, so a camera that snapped
  during its own `ResetTo` would cut to wherever the player was standing a moment earlier, and only
  when the camera happened to register first. `LateUpdate` runs after every `Update` and so after the
  player has been placed, whatever the registration order was. The alternative was ordering the
  registry, which trades a one-line flag for a rule every future resettable has to know about.

- **A level's place in the order is authored on the level, and the transition has one branch.**
  `LevelDefinition` carries a `levelNumber`; `GameFlow` scans once with inactive roots included,
  sorts by it, and `CompleteLevel` asks only whether an index past the current one exists. Three
  alternatives lost: a serialized array of level roots is a second place to keep in step and a level
  added without being dragged in is silently missing; sorting by name makes the naming a hidden
  contract and puts `Level_10` before `Level_2`; hierarchy sibling order breaks the moment anyone
  reorders the Hierarchy. Authoring the number on the root matches the decision that a level's size
  is authored there too, so adding a third level is one root with one number and nothing else to
  remember. It also lets `CurrentLevel` be an array lookup rather than the per-frame scene scan the
  camera would otherwise drive.
- **Neither ending restarts by itself.** `CompleteLevel` on the last level and `LoseStrike` on the
  last פסילה both raise their event and stop. The first draft had the game-complete path calling
  `StartGame` on its own, which reads as convenient and is wrong: 2.3 and 2.4 both specify a popup
  **with a button**, so the restart is a player action in both cases and the popup would never be
  read. Both paths now have the same shape - raise, stop, wait for something to call `StartGame` -
  which is exactly what stage 10 wires its two popups to, so that stage gains a listener instead of
  having to undo a stub. Until then the `3` key stands in for the button.
- **The camera's execution order is not assumed.** `LevelCamera.Start` may ask for `CurrentLevel`
  before `GameStarter.Start` has called `StartGame`, because Unity's order across two default-order
  MonoBehaviours is arbitrary. So `GameFlow` treats whichever level root is already switched on as
  the current one until a level is properly entered, and the answer is right either way rather than
  depending on an ordering nobody guaranteed.

- **`LevelState` is dropped, because this stage's own two halves disagreed.** Step 1 had it holding
  כוח, which enemies are dead, which collectibles are taken and the player's position; the paragraph
  below it described `IResettable`, where each of those objects owns its own state and knows how to
  restore it. Both cannot be true. A class holding four unrelated collections duplicates facts their
  owners already have, which is two copies of every fact and the god object the SOLID register warns
  about; and with `IResettable` doing the work there is nothing left for it to hold - כוח is stage 9's
  `PowerModel`, an enemy knows it is dead, a collectible knows it is taken, and the player's position
  is his transform. The scope still exists as the thing a full reset restores. It is a verb, not a
  noun.
- **`IResettable.ResetTo(ResetScope)` takes a named scope rather than a bool.** The plan called for a
  flag on one method and this is that flag with a name: `ResetTo(true)` says nothing at a call site
  and `ResetTo(ResetScope.Full)` says everything. The Template Method rejection is unchanged and still
  worth saying out loud. What makes the design open/closed is that each object decides what a partial
  reset means to it - an enemy ignores `AfterStrike` and stays dead per 3.4, a collectible restores
  itself on both per 3.5 - so the reset code never learns which kinds exist.
- **Interfaces where there are many consumers, concrete bindings where there are not.** `IResettable`
  and `IGameFlow` get interfaces: the first is the stage's whole design and the second is depended on
  by the camera, the player, the HUD and later every hazard. `SessionState` and `CarriedState` are
  bound concrete - two consumers each, no second implementation, and no tests to fake one for, which
  is the same argument that refused `IInputService` in stage 6. This departs from step 1's "behind
  interfaces" on purpose. Lesson 12's own installer does both on consecutive lines,
  `Bind<IFireballBuilder>().To<FireballBuilder>()` beside `Bind<FireballDirector>()`, so neither is
  unidiomatic to him, and being able to say why each is where it is answers better than a uniform rule.
- **`GameFlow` owns which level is active, and the camera asks it.** `LevelCamera` currently finds the
  level itself with `FindObjectsByType` at `Start`, and the player is about to need the same answer to
  put himself back at `PlayerStart`. Two lookups is the small problem; the real one is stage 18, where
  a lookup done once at `Start` is stale the moment the transition switches levels, silently and in
  both classes at once. One owner fixes both, and it is the seam stage 18 needs anyway. The cost is
  editing finished code from stage 7.
- **The player returns to the start as an `IResettable`, not as something done to him.** `GameFlow`
  holds no `Transform` and no player reference: on a reset the player puts himself at the current
  level's `PlayerStart`, through the same machinery every collectible and enemy uses. That is what
  keeps the flow a plain C# class with no Unity types in it beyond `LevelDefinition`, which is the
  Clean Architecture line the course keeps making and this project's DI defense answer.
- **Tuning numbers enter the object graph at the installer.** `Exercise Adventure Island.md` says every
  bracketed number belongs in a serialized field because the instructor may want it changed in front of
  you, and `SessionState` is a plain C# class with no Inspector. `GameInstaller` is a MonoBehaviour, so
  the number is a `[SerializeField]` there and reaches the class through `.WithArguments(...)`. The
  composition root is where configuration enters, which gives the installer a job rather than leaving
  it a formality. Revisited at stage 9: four or five fields is where this stops being a binding list
  and starts being a settings dump, and a `GameSettings` ScriptableObject takes over.

- **The camera is 16:9 at an orthographic size of 5, and the original's framing is not reproducible.**
  Size 5 fixes the view's height at 10 world units and the width follows from the aspect - 17.8 at
  16:9, 13.3 at 4:3 - so the aspect is not cosmetic, it decides how far ahead the player can see. The
  NES outputs 256x240 with about 224 rows visible and Adventure Island's tiles are 16px, so the
  original shows roughly 16 tiles across by 15 up, on a framebuffer that is nearly square but was
  stretched to 4:3 on a CRT. At 16:9 that shape cannot be matched: size 7.5 matches its height and
  comes out 67% too wide, size 4.5 matches its width and is 40% too short, and even 4:3 at 7.5 is
  still a quarter too wide. So faithfulness was dropped as a goal. 16:9 wins on the two things that
  are real - the gameplay recording is a deliverable and screen capture is 16:9, and neither source
  mentions aspect ratio at all. Player Settings needs no change: `defaultIsNativeResolution` is on and
  the mode is fullscreen, so a build takes the monitor's resolution and the 1024x768 left in the file
  by the template is never read. Only the Game view's own aspect has to be pinned to 16:9, so what
  gets tuned is what gets recorded.
- **The player sits a quarter of the way up the view, not centred.** Peleg's call: a platformer needs
  headroom above the character more than floor below him, and the view is 10 units tall, so a quarter
  up gives 2.5 below and 7.5 above. Held as a viewport fraction rather than an offset in units, so
  changing `followSize` does not silently retune it. The bias is applied **before** the clamp, or
  pushing the view up could take it past the top of the level, which is the one thing 11.4 forbids.
  On flat ground in level 1 the bottom clamp is already binding and the bias does nothing; it takes
  over once there is room, which is most of level 2.
- **Follow and frame-the-whole-level are one class and one bool.** The testing view is a single branch
  of real difference, and a base class with two subclasses for a debug toggle is the same
  pattern-for-its-own-sake the full-versus-partial reset already rejected. Worth saying at the defense
  that it was considered, for the same reason and with the same answer.
- **The camera holds a serialized reference to the player, not a tag lookup.** Exercise 3's
  `CameraFollow` used `FindGameObjectWithTag` and its own comment says why: Mario was rebuilt with the
  level, so an Inspector reference would point at a destroyed object. Stage 6 made the player one
  object for the whole game, at the scene root, never destroyed - so the reason is gone and the lookup
  would be inherited ceremony. The level is still found rather than referenced, by
  `FindObjectsByType<LevelDefinition>`, which excludes inactive objects and so answers with whichever
  level is switched on.

- **The תהום has a floor, and the spike sprite is only ever the תהום.** Peleg's design, and it
  replaces the fall line `LevelDefinition` was going to carry. The fall line answered "how far does he
  drop before he dies" with a number nobody can see, tuned by feel, where being wrong means either
  dying above the floor or falling for an uncomfortably long time; a pit with a floor removes the
  question instead. The instructor offered spikes at 00:50:15 as "עוד תהום", another kind of pit, so
  treating them as one mechanic is his framing. The rule that keeps 9.3 intact: **the spike sprite
  never appears as a standalone hazard**, only as a pit floor, so it is always the תהום and the פייה
  never destroys it. Fire covers hazards on flat ground. Without that rule 9.2's blanket "destroys
  anything he touches" would eat a pit floor, and 9.4 says outright he will collect a פייה, jump into a
  תהום and check that it still kills.
- **כוח drains 1 every 3 seconds, and the level sizes are what set it.** Grepping both sources for every
  mention of כוח and of seconds turns up no rate at all: the written text says only
  "לאט לאט הכמות כוח הזאת יורדת" and the transcript repeats it twice without one. The bar's `16` and
  the starting `11` are unsourced too; the only number in section 4 that is his is the `20` of 4.7,
  revised down from the written 30 on camera. He also treats the rate as a difficulty dial himself
  (00:15:06), so filling the blank in is not bending anything. The arithmetic, against level 1 at 200
  wide and level 2 at 50 tall: crossing them takes about 61 and 50 seconds, which at 3 seconds a unit
  needs roughly 7 and 4 fruit, eleven in total with room for a slower player to need fifteen. At 2
  seconds the same levels demand 23, which trips 4.7 and costs a פסילה for playing correctly.
- **Level 1 is about 200 by 15 and level 2 about 30 by 50, provisionally.** Peleg's numbers, read off
  the levels the instructor referenced and then cut to fit the כוח arithmetic above - the first draft
  was 300 wide, which needs a 5-second drain to survive, and at that point 4.5's "כוח reaching zero
  costs a פסילה" never fires in normal play. Revisited at stage 19 against finished mechanics.
- **Twenty fruit is a floor to clear, not a ceiling to stay under.** The rule reads like an
  anti-farming penalty and it is not one. 00:51:30: "פה רשמתי שאם לוקחים 30 פירות לפסילה, בוא נגיד 20
  פירות, שלא צריך ממש הרבה לקחת, יהיה קל גם בשבילי. 20 פירות נותן לי פסילה, אני רוצה לראות את זה" - he
  cut 30 to 20 so that reaching it would not take much, said it would be easy for him too, and said he
  wants to see it. So the constraint runs the other way from how it first reads: **at least 20 fruit
  have to be collectable in one playthrough**, or something he said he wants to watch happen cannot be
  shown at all. Two numbers follow. About 15 fruit per level, 30 across the game, so a thorough player
  passes 20 partway through level 2. And the game has to stay winnable after that פסילה is spent,
  which it is - two remain. A player who takes only the eleven or so his כוח needs never triggers it,
  which is what makes the rationing a real decision.

- **Comments are cut to two lines, and stop arguing.** Peleg's call, made partway into Stage 6:
  the existing comments are tiring to read and want to be half the length. Two things were making
  them long. The four-line cap was generous, and the "X rather than Y, because Z" shape was carrying
  the full case for a decision that the Decisions Log already holds - so the same argument was being
  read twice, once here and once in every file the decision touched. The rule now names the
  alternative in a clause and leaves the case where it lives. Applied to everything written from
  here, and the last step of this stage walks the thirteen files that predate it. Doing that now
  rather than at Stage 20 keeps it a mechanical pass instead of an edit to finished code.

- **Input is read straight off the device, and the template action asset is deleted.**
  `Keyboard.current.leftArrowKey.isPressed` in the component that acts on the key, as Exercise 3 did.
  `Assets/InputSystem_Actions.inputactions` was the Unity template, untouched: nine actions of which
  this game uses three, an `Attack` bound to the mouse rather than `Z`, and a whole UI map of
  tracked-device actions. Keeping it meant either editing it down or shipping dead bindings in the
  submission, and an action asset earns its keep through rebinding, a second device or an options
  screen, none of which this game has. The scene's `EventSystem` never referenced it - its
  `InputSystemUIInputModule` points at the Input System package's own default actions - so the only
  reference was the project-wide slot in `EditorBuildSettings.asset`, cleared before the delete. The
  cost is that adding a gamepad later means writing the polling for it by hand.
- **The jump is one impulse, cut on release.** `linearVelocity.y` is set on press and multiplied by a
  factor on release while he is still rising, so the impulse fixes the maximum height and letting go
  early throws away the rest of the rise. Holding to sustain - a base impulse plus lift applied each
  step for up to a maximum hold time - was the alternative and it is the more literal reading of 1.4,
  at the cost of a third tunable, a floaty top end and a ceiling case to handle. Input is read in
  `Update`, where `wasPressedThisFrame` is reliable, into flags consumed in `FixedUpdate` in order:
  jump first, then cut, so a tap shorter than one physics step still produces the minimum jump rather
  than being swallowed. No coyote time and no jump buffering: a press while airborne is consumed and
  lost, which is what the original does and what neither source asks to change.
- **The grounded answer carries a grace window, reversing "no coyote time".** Measured rather than
  guessed: walking across flat, coplanar tiles, `Rigidbody2D.GetContacts` returned **zero** contacts
  for one to four frames at irregular intervals, and on several of those frames the body's vertical
  velocity was *positive*. Nothing in the player's code pushes him up while walking, so that is the
  contact solver - the capsule sinks slightly into the floor each step, position correction pushes it
  back out and leaves a little upward velocity behind, and gravity scale 3 deepens the penetration
  that drives it. The visible symptom was a jump frame flickering during a walk. The invisible one
  matters more: a jump pressed inside one of those gaps was refused on flat ground, silently. The
  normal was never the problem - it read exactly 1.000 whenever there was contact at all - so the
  `CompositeCollider2D` would not have helped. `PlayerJump` gains a cooldown longer than the window,
  because a grace window on its own turns one press into a free double jump.

- **Grounded means a contact whose normal points up.** `Rigidbody2D.GetContacts` and a test on
  `normal.y`, rather than Exercise 3's `OverlapBox` probe. That probe needed an `SC_Floor` marker on
  every tile to tell terrain from everything else, and this project's tile prefabs carry no script.
  Contacts need no probe geometry, no depth or width factor, no layer mask and no marker; they cannot
  return the player's own collider, a wall's normal points sideways so it cannot be mistaken for a
  floor, and triggers generate no contact points at all, which excludes every hazard and pickup in
  this project by construction - the spikes, the אבן, the מדורה and the door are all triggers already.
  A `Ground` physics layer on the three earth-tile prefabs is the explicit version and stays available:
  it is a `LayerMask` on the filter and three prefab edits, and the reversible option goes first for
  the same reason the `CompositeCollider2D` was deferred. The check is a private method on
  `PlayerJump` while jump is its only caller, and moves out when the Animator becomes the second one.
- **Linear damping 0, and braking by hand.** Exercise 3 carried damping 5 *and* an explicit
  `MoveTowards` brake, which is two brakes doing one job. Damping also decays vertical velocity, so
  the jump apex would stop being a function of the jump speed and start being a function of a number
  tuned for horizontal feel. Braking stays explicit and applies in the air as well, which is Exercise
  3's behaviour: full air control is easier to play, and this is not a momentum game.
- **Settled in the tuning pass: capsule 0.9 wide, jump speed 13, collision detection Discrete.**
  The width question below was left open on purpose and closed by playing it - 0.9 is what fits
  through a one-cell gap, and `CONVENTIONS.md` now carries that as the stated exception to sizing a
  collider to its art. Discrete replaces Continuous because the player covers 0.12 units per physics
  step against one-unit tiles and has nothing to tunnel through, so Continuous only bought
  speculative contacts and the jitter that came with them. `jumpSpeed` 13 against gravity scale 3
  puts the apex at about 2.87 units, just under three cells. Everything else kept its starting value:
  speed 6, deceleration 40, rise cut 0.5, contact grace 0.1, jump cooldown 0.15, and the walk clip at
  12 frames a second with no Animator speed multiplier.

- **The capsule is the width of the art, 1 by 2.** A collider exactly as wide as a one-cell gap
  cannot be relied on to pass through one, and level 2 climbing bottom to top is likely to want
  narrow shafts, so 0.9 was the alternative - Exercise 3's circle of radius 0.45 was the same
  number on a one-cell body. Starting at the art's own measurement keeps `CONVENTIONS.md`'s collider
  rule unbroken, and Stage 6's last step is a tuning pass with the animation on, which is where a
  real snag would show up rather than being predicted.

- **The start marker is found, not referenced.** `PlayerStart` sits on the `Sprite_Player_Start`
  prefab and the level asks its children for it. A serialized `Transform` on the level root was the
  obvious alternative and it dangles the first time a rebuild replaces the marker - the incremental
  build keeps a marker that hasn't moved, so the break would only appear after the start point was
  edited, which is the worst time to find it. The same component hides its own sprite when the level
  starts, since the 40% alpha exists for authoring only.


- **Stage 10's six steps are three, and the `Time.timeScale` step dissolved into the popups.** Peleg's
  call. Freezing time was written as a step of its own, and it is one line at each end of a sequence
  that does not exist until the popups do, so it was never separable from them. The two popups merged
  for the opposite reason: the congratulation popup is the game over popup duplicated with different
  text plus one more method on `IPopups`, so keeping them apart meant building the machinery, testing
  it against one caller, and then adding the second caller in a step holding nothing else. Splitting
  them also left a hole, since deleting `PowerController`'s `running` flag depends on *both* endings
  freezing time - a step that froze only one would either keep dead code for a step or leave the
  drain running after level 2.
- **Two popups, three classes: `Popup`, `Popups` and `IPopups`.** `Popup` is a one-button panel that
  switches itself on, hands back a `Task` and completes it when its button is clicked; `Popups` is the
  only thing that knows which scene object is the game over one; `GameFlow` depends on `IPopups`. The
  alternative was one class with one panel, two serialized message strings and a label rewritten per
  call - fewer objects and less to build in the Canvas. It loses on two things that are not about
  design purity: the words the player reads become a `[SerializeField] private string` default in
  code, and the Hierarchy stops naming `Popup_GameOver` and `Popup_Congratulation`. **What to claim at
  the defense, and what not to.** SRP and DIP are real here - three separable responsibilities, and a
  flow that depends on an interface rather than a MonoBehaviour, which is the only reason it can be a
  plain C# class at all. OCP is not: a third popup adds a method to the interface rather than an
  implementation of it. ISP is not either, since `IPopups` has one consumer that uses all of it. The
  single-class version would pass a SOLID reading too.
- **The popups are hidden by `Popups.Awake`, and the first attempt at that was a bug worth keeping.**
  The panels should not depend on being left switched off in the saved scene: that means authoring
  their layout switched on and shipping whichever state they were last left in, and a scene opening
  with GAME OVER over the game is one forgotten checkbox away. The obvious answer was for `Popup` to
  hide itself on `Awake`, the way `PlayerStart` hides its marker - and it deadlocked the game.
  **`Awake` does not run at scene load for an object that is inactive in the scene; it runs the first
  time that object becomes active**, which is inside the `SetActive(true)` that shows the popup. So
  the panel was shown, `Awake` fired, and it hid itself again inside the same call, handing back a
  `Task` that nothing could complete with time already frozen at 0. `PlayerStart` never hit this
  because its object is active and only its `SpriteRenderer` is switched off. The hiding moved to
  `Popups.Awake`, which is on the always-active `Canvas` and so runs exactly once at load.
- **What survives `Time.timeScale = 0` was measured rather than assumed.** `FixedUpdate` stops, so
  `PlayerMovement` stops with no edit to it; `LevelCamera.LateUpdate` still runs but `SmoothDamp`
  reads `Time.deltaTime` and interpolates by zero; `PowerController` still ticks and drains nothing;
  the Animator is on scaled time and freezes on the frame he died. The button keeps working because
  the scene's `InputSystemUIInputModule` runs on the Input System's default
  `ProcessEventsInDynamicUpdate` - there is no input settings asset in the project, so that default is
  load-bearing, and `ProcessEventsInFixedUpdate` would freeze the button that unfreezes the game.
  **The one thing that broke:** `Update` keeps *reading* input even though nothing acts on it, so
  `PlayerJump` latched a press made under the popup and spent it on the first physics step after the
  restart - and since `Time.time` is frozen too, `PlayerGround`'s contact grace still held the contact
  from before the death, so he really did jump on respawn. `PlayerJump` gained a `ClearInput` that
  `PlayerReset` calls, rather than a second `IResettable` on the player, since `PlayerReset` already
  coordinates his reset across components. The honest limit, worth stating at the defense: freezing
  time stops things *acting*, not *reading*.
- **`async void` with a `try`/`catch`, which is the opposite of the usual C# advice.** Everything that
  calls `LoseStrike` is synchronous, so the sequence is started without being awaited, and there are
  two ways to do that. `private async Task` called without `await` swallows every exception into a
  Task nobody observes. `async void` rethrows on the synchronization context, which Unity logs - so
  here it is the one that reports faults rather than hiding them, and the catch makes that explicit
  instead of relying on it. Worth saying out loud, because "never use async void" is the answer a
  grader expects and the reasoning is what makes it wrong in this one place.
- **A private `ending` flag in `GameFlow`, and it is not the state machine this log rejected.** With a
  popup up, `Update` still runs, so `DebugFlowKeys` still reads the keyboard: key `1` under the game
  over popup calls `LoseStrike` again, and since strikes are already zero it logs a second game over
  and shows a second popup, replacing the `TaskCompletionSource` and orphaning the first `await`
  forever. Key `2` does the same to the congratulation popup. One private bool, set when a sequence
  starts and cleared by `StartGame`, closes both. The rejected `Playing`/`GameOver`/`Won` state failed
  on the objection that every input-reading component would have to consult it; nothing outside
  `GameFlow` ever reads this one. It also keeps `Popup` simple, since it never has to survive being
  shown twice. **Debug key `3` is deleted rather than guarded**, because the guard cannot cover it:
  `StartGame` is what *clears* the flag, so pressing `3` under a popup unfreezes and re-enters level 1
  while the popup stays on screen with its `Task` still pending, and the button then starts a second
  game. `DebugFlowKeys` said itself that key `3` stood in for the popup button, and the button now
  exists. Keys `1` and `2` stay for now: the only way to lose a strike by playing is waiting out the
  drain, which is about 100 seconds per game over in the one stage whose point is running the loop
  over and over. The better lever is `Level_1`'s starting כוח set to 1, which reaches a game over in
  about nine seconds through `PowerController.Spend` rather than through a synthetic call. Whether
  either key ships is stage 20's.
- **The popups' text is English.** `GAME OVER` and a `Restart` button, matching the original rather
  than the course's language. Hebrew on screen would need TMP's right-to-left mode and a font asset
  carrying Hebrew glyphs, which the default TMP font does not have, and the Hebrew that matters is
  already carried by the requirements document and the video script. TextMeshPro itself ships inside
  `com.unity.ugui` on Unity 6, so the namespace compiles with nothing installed, but the essential
  resources are a separate import that writes about 2MB of font assets into `Assets/` and ships with
  the submission.

- **The strikes display is MVC with an existing model.** `SessionState` is the model - a plain C#
  class, no Unity types, owning the count and its one rule - so only the view and the controller are
  new. Writing a `StrikesModel` beside it would put the number of strikes in two places, and the first
  thing to decrement one and not the other would make them disagree. The alternative worth naming,
  because it is the version where every counter owns a model: split `SessionState` into a
  `StrikesModel` and a `FruitModel`, have `GameFlow` depend on both, and delete it. That buys a
  textbook triad per counter and loses the thing `SessionState` exists to say - that these are the two
  numbers surviving a death and a level change, which is why it is not an `IResettable`. **The
  controller is thin and saying so is better than pretending otherwise:** two subscriptions and one
  push. Folding it into the view would save a class and an interface at the cost of putting the flow
  and the session inside a MonoBehaviour.
- **`IGameFlow` gains `GameStarted`, which was a gap rather than an addition.** The flow announced
  every transition except the one that begins a game, so nothing could tell a display that the count
  had gone back to three. `StrikeLost` covers the way down, including the drop to zero under the game
  over popup where no reset runs. Two alternatives lose: making the controller `IResettable` misuses
  an interface meant for restoring state on something that has none, and it would stay silent on game
  over; polling `SessionState` in `Update` reads sixty times a second a value that changes three times
  a game. Stage 11's fruit counter needs the same event to zero itself.

- **A SOLID and Clean Architecture audit before stage 11's own work, and three refactors out of it.**
  Peleg asked whether the design held up, and taking the unflattering answer rather than the
  flattering one turned up three things. **`FruitCollectible` would have depended on the concrete
  `PowerController`**, a MonoBehaviour reaching straight into a controller, which is the exact shape
  DIP exists to stop - so `IPower` with `Gain` and `Spend`, earned twice over because stage 12's אבן
  needs `Spend`. Its verbs differ from `IPowerModel`'s `Add` and `Remove` deliberately: two identical
  `Add` methods at two layers is a question every reader of the fruit code would have to ask.
  **`GameFlow` was doing four jobs** - the operations, the ending sequence, the level list and the
  registry of resettables - so the registry became `ResetRegistry` behind `IResetRegistry` and a new
  `IResetRunner`, and the level list became `Levels` behind `ILevels`. The ending sequence stayed,
  because moving it out would undo the reason the Task is earned. **`ILevels` paid three ways**: it
  took a third of `GameFlow` away, it let `PlayerReset`, `LevelCamera` and `PowerController` stop
  depending on the class that can end the game when all they wanted was `CurrentLevel`, and it moved
  the `LevelDefinition` leak off the core game interface onto one whose subject is levels. The test
  for the whole refactor was that the log came out identical.
- **What the audit found and deliberately did not fix, which is the more useful half at a defense.**
  `IGameFlow` has ten members and no client uses both halves - operations or events, never both - so a
  member-counting checker will name it; splitting it into `IGameFlow` and `IGameEvents` was rejected
  because the two would always travel together with the same lifetime and the same implementation,
  which is bookkeeping rather than segregation. `IGameFlow.CurrentLevel` returning a MonoBehaviour was
  the Clean Architecture crack, and moving it to `ILevels` was the fix; inventing an `ILevel` to hide
  one property would have been worse code sold as better architecture. The rule applied throughout:
  three classes beat one only while each still has a name you can say in a sentence.
- **`TakeFruit` is an operation on the flow, not on a fruit controller.** Peleg pushed on whether
  strikes and fruit belong in `GameFlow` at all, and the test that settles it is whether a method
  decides what happens to the game or only stores a number. Storing is `SessionState`'s and always
  was: `LoseStrike` touches the count in one line and spends the rest deciding between ending the game
  and resetting the level. `TakeFruit` is the weaker of the two and its whole claim is 4.7 - strip the
  every-twenty rule away and it belongs to a fruit controller. It stays because of what the split
  costs: fruit → `Fruit.Take` → `StrikeOwed` → `LoseStrike` → `GameOver` → popup is five hops through
  three classes, where it is now two methods in one file, and "what happens when I take my twentieth
  fruit" is exactly what an oral defense asks. The payoff is that `FruitController` and
  `StrikesController` came out line for line the same shape, both pure readers, which is what earned
  `ISessionState`.
- **The collectible base was built with one subclass, knowingly against `CONVENTIONS.md` rule 5.**
  `Fruit_1` and `Fruit_2` differ by a number, so they are one class on two prefabs, and the other
  seven collectibles arrive at stages 13, 15, 16 and 17. Rule 5 exists to stop speculative
  abstraction, and eight named subclasses written down in a requirements document are not
  speculation. Template Method also has to be placed somewhere, and a base with eight real subclasses
  is the strongest placement available.
- **The template's steps are detect, *consume*, apply - and the obvious order was wrong.** Applying
  first breaks the twentieth fruit: `PickUp` costs a פסילה, the פסילה runs the reset, the reset brings
  every collectible back, and then the template deactivates the one just taken. One fruit vanishes
  from an otherwise fully restored level, once every twenty. Consuming first lets the reset put it
  back with the rest. Stage 12 has the same shape - a hazard runs the reset from inside a trigger
  callback.
- **A collectible registers in `Awake` and releases in `OnDestroy`, unlike every other `IResettable`.**
  `PlayerReset` and `LevelCamera` use `OnEnable`/`OnDisable`; a collectible cannot, because it switches
  itself off when taken, so `OnDisable` would drop the very object the reset has to bring back and the
  fruit would silently never return. Safe because Zenject's `SceneContext` carries
  `executionOrder: -9999` in its meta file, so injection finishes before any default-order `Awake`,
  and a level 2 object's whole activation happens inside `EnterLevel`'s `SetActive(true)` before
  `ResetAll` runs. **The hazard this creates, for whoever writes the next subclass:** a subclass
  declaring its own `Awake` hides the base's, Unity calls only the subclass's, and registration stops
  with no warning of any kind. Nothing deriving from `Collectible` may declare `Awake`.
- **`PickUp()` takes no parameter.** Passing the `Player` through was the obvious signature and no
  collectible turned out to want it: fruit goes to `IPower`, a weapon will go to a weapon holder, the
  פייה to an invincibility component, all injected. The marker component is still what identifies him
  at the trigger; nothing needs the object afterwards. If stage 15 disagrees, adding the parameter
  touches the subclasses that exist by then.
- **Two counter views, not a shared base - reversing what this log predicted.** Stage 10 recorded that
  the fruit counter would be "the place a shared view would be earned". Written out, `StrikesView` is
  eighteen lines and what `FruitView` shares is a serialized field, a null check and one line of
  formatting; a `CountView` base with two one-line subclasses is three types doing the work of two,
  and there is no third counter coming - 12.1, 12.2 and 12.3 are פסילות, fruit and the bar, and the
  bar is a different shape entirely.
- **The fruit counter turns red for the five before a פסילה, and the view owns the threshold.**
  Peleg's addition, and it earns its place beyond decoration: the instructor said he wants to *see*
  the twenty-fruit rule happen, and a number that reddens makes that legible in a recording instead of
  ticking over unremarked. The rule is `count % 20 >= 15`, so red at 15-19, 35-39 and 55-59 and never
  at a multiple of twenty, where the פסילה has already been paid. **The split that made it fit:**
  Zenject's `WithArguments` matches by type, so a controller taking both the period and the margin
  would have two ambiguous `int`s. So the controller hands the view *how many fruit remain before the
  next פסילה* - a fact - and the view decides at what distance that turns red, which is a display
  choice. `fruitPerStrike` moved from a `const` in `GameFlow` to a `[SerializeField, Min(1)]` on
  `GameInstaller`, beside `startingStrikes` and `drainSeconds`; the `Min` is what keeps a zero out of
  the modulo without a runtime guard in two classes.
- **Fruit variety is baked when the tile is placed, not rolled at Play.** The first version randomised
  in `Awake`, which meant the Scene view showed nine identical apples and the game showed a mix.
  Peleg's call, and it is the better fit: `CONVENTIONS.md` already says the diff-build exists so that
  "the file records what is where, and the scene instance records which one it is", and a chosen
  sprite is exactly that. `SpriteVariant.PickOne` is called by both level tools on any tile carrying
  the component, so neither tool learns anything about fruit, and
  `PrefabUtility.RecordPrefabInstancePropertyModifications` is what makes the choice a saved override
  rather than a change that vanishes on the next scene load. Painting over a tile that already holds
  the same prefab re-rolls it instead of doing nothing, reported separately from placements so the
  Console does not claim work it did not do. The cost: `Save Level` writes tile ids only, so building
  a level into an empty parent re-rolls everything.
- **Both counters read `00`, matching the original's HUD.** Read off a screenshot of Adventure Island:
  a face icon and `03` top left, a fruit icon and `00` top right, the meters centred. `x3` was chosen
  first and reversed once the screenshot settled it.

- **`IDestructible` is one method, and `Destroyer` is a `[Flags]` enum.** Every object answers "what
  is allowed to destroy me" as a single value - the אבן is `Boomerang | AnimalAttack | Riding |
  Fairy`, the מדורה is `Fairy | Riding`, the רוח רפאים is `Fairy` alone - and `TryDestroy(Destroyer)`
  is the only way to destroy anything, so no caller can read the rule and then ignore it. The
  rejected shape was a `DestroyedBy` property beside a `Destroy()` method, which reads more like data
  and puts the bitwise test into the axe, the boomerang, three animal attacks and the פייה, which is
  the scattered conditional the interface exists to remove. The `bool` return is used rather than
  decorative: 6.9 needs an axe to tell a rock it bounced off from an enemy it killed. `Riding` is a
  separate value from `AnimalAttack` because 7.8 and 5.8 disagree - an animal's attack leaves a
  מדורה standing and riding into one destroys it. `Fairy` appears in every answer but the spikes',
  which looks redundant and is not: each type's one line then states its whole requirement. The
  visitor version, where each destroyer is a type and the target accepts it, removes the bitwise test
  and costs five classes and a method per target. The mandate for all of this is his own, at
  00:18:44: "למה זה חשוב? כי מבחינת פיתוחים מאחורי הקלעים זה חשוב, איך שאתם בונים את זה מאחורה".
- **The תהום is not a `Hazard` and does not implement `IDestructible`, which is what makes 9.3 free.**
  `Spikes` is a standalone `MonoBehaviour` that costs a פסילה and never consults the player's guard.
  Both protections then come out exact with no exception written anywhere: the **פייה** survives a
  contact if and only if `TryDestroy(Fairy)` returned true, and there is nothing in a תהום to
  destroy; the **animal** absorbs a hit from anything that implements `IDestructible` at all, and
  spikes don't. The uniform alternative - `Spikes : Hazard` answering `Destroyer.None` - keeps the
  פייה rule working and breaks the animal one, because "is it destructible" becomes true for spikes,
  so it needs a second per-type flag that exists solely for one subclass to say no. That flag is the
  cost the structural version avoids, and 9.4 is the requirement he said outright he will go looking
  for. The price paid instead: `Spikes` shares no base with `Fire` and `Rock`, duplicating about six
  lines of trigger and player detection. **It still lives in `Hazards/`**, because folders here group
  by domain and not by hierarchy - section 5 of the requirements is titled Hazards and holds all
  three - and `Collectibles/SpriteVariant.cs` already set that precedent by not being a `Collectible`.
- **`IPlayerGuard` is built two stages before either protection exists.** The riding rule and the
  fairy rule live in one component on the player, and every `Hazard` consults it before applying its
  effect. Today it absorbs nothing. The alternative was hazards applying their effects directly and
  the consult being inserted later, which means editing `Fire`, `Rock` and six enemies to add one
  line each in exactly the right place - and a miss is silent, in the stage where a dozen other
  things are being tested. `TryAbsorb` answers whether the contact was *survived* rather than whether
  the source was destroyed, because 7.10 and 8.21 disagree: riding into a רוח רפאים costs the animal
  and leaves the ghost standing.
- **Temporary code carried by stage 12, to be removed when its stage arrives.** Stage 13: the comment
  on `Hazard.TryDestroy` saying it has no caller until the boomerang exists - the method itself ships
  unexercised, which is the accepted cost of settling the interface before the things that call it.
  Stage 15: `PlayerGuard.TryAbsorb` returns `false` unconditionally, and the animal fills in the
  first of its two branches. Stage 17: the פייה fills in the second. Neither the class comment nor
  the log line inside it says "not yet", deliberately, so that forgetting leaves no false statement
  in the code.
- **The shove lives on `PlayerMovement`, behind `IPlayerShove`.** `PlayerMovement.FixedUpdate` writes
  `linearVelocity.x` on every step - the walk speed while a key is held, a 40 u/s² brake when none is
  - so an impulse written by the אבן is gone within two frames. It gains `Shove` and a suspension it
  checks at the top of `FixedUpdate`, which keeps one class in charge of his horizontal velocity; a
  separate `PlayerShove` component disabling `PlayerMovement` was the alternative, and two things
  writing one field is where every platformer velocity bug comes from. **Direction is his own
  facing**, so the signature is `Shove(speed, seconds)` and the אבן never computes a direction -
  "forward" is the player's idea of forward, and it is right from either approach. The sign of
  `player.x - rock.x` was the alternative and it is unstable when he lands on top. The interface
  rather than the component because **every MonoBehaviour in the installer is already bound behind
  one** - `IPopups`, `IPowerView`, `IStrikesView`, `IFruitView`, `IPlayerGuard` - while the plain C#
  classes are bound concrete; the line that describes what is already there is that a MonoBehaviour
  carries members a consumer has no business calling and `SessionState` does not.
- **Shove before spend, which is stage 11's trap arriving from the other side.** `IPower.Spend` can
  empty the bar, and `PowerController` calls `LoseStrike` at zero, which runs the reset and teleports
  the player to `PlayerStart` - all synchronously, inside the אבן's own trigger callback. Spending
  first would then fling him across the level from the start point with his controls suspended.
  Shoving first means a fatal אבן does not shove at all, because `PlayerReset` zeroes his velocity
  and clears the suspension on the way past. `Collectible` had to consume *before* applying for the
  same underlying reason and it produced the opposite order.
- **The immunity is read off the player and only the אבן asks.** First written as a per-instance
  `ignoreUntil` on the אבן, which is what "you cannot hit it again" reads like and is wrong on the
  map: `Level01.txt` has three אבנים side by side, a shove of 10 for 0.3s carries three cells, and
  the log showed one contact costing nine כוח. So `IPlayerShove` answers `IsShoving` and the אבן asks
  before charging. It is not a global immunity, which is the thing 5.2 forbids - fire and spikes
  never ask, so being flung into a מדורה still kills. **Contact is `OnTriggerStay2D` as well as
  `OnTriggerEnter2D`**, or a player pinned inside an אבן by a wall sits there safely forever; the
  אבן's own `IsShoving` check is what turns that into one charge every 0.3s rather than one a frame.
  2D sends Enter and Stay together on the step a contact begins, so `Hazard` applies once a frame at
  most - without that a מדורה costs two פסילות for one touch. That guard is also why
  `PlayerGuard`'s log line was deleted: per-contact became per-frame, which convention 1 forbids.
- **`SpriteCycleAnimator` reads `Time.time` and holds no timer.** `(int)(Time.time / interval) %
  frames.Length`, with the last shown index cached so the renderer is written only when the frame
  changes. Peleg's call that a row of מדורות should flicker together, and reading the clock rather
  than counting per instance makes that true by construction instead of by their having been switched
  on at the same moment. An Animator was the alternative and it costs a controller and a clip asset
  per object - twenty-two assets for eleven things - to say what an array and one float say here,
  where the player keeps his Animator because idle, walk, rise and fall are real states with real
  transitions and a מדורה has one state and two pictures. It lives in `Assets/Scripts/Animation/`,
  the one folder not named for a domain, because it goes on hazards, enemies, animals and a
  projectile alike.
- **The מדורה's collider is sized to its shorter frame.** The tall flame is 1.5 units and the short
  one 1.31, and a collider fixed at 1.5 stands nine source pixels above the flame for half of every
  cycle, killing the player on a gap he can see he cleared. Size Y 1.3 at offset Y 0.15 puts the
  error the forgiving way, which is the direction 8.5 asks for. The average of the two heights was
  the other option and it halves the unfairness rather than removing it.

- **A weapon is carried, a projectile is in flight, and they are different folders.** Peleg's call,
  reversing `CONVENTIONS.md`'s `Player/Projectiles/`: a נחש's fireball and a red animal's fire have
  no weapon behind them at all, so `Projectiles/` sits at the top level beside `Enemies/` and
  `Hazards/`, and `Player/Weapons/` keeps the slot and the keys that fill it. Everything
  that makes an axe a weapon - the egg, the slot, the key, the cap - lives outside the thing flying
  through the air.
- **One thin `BaseProjectile`, and the rule for what is allowed in it.** Peleg argued the four
  projectiles share nothing, and he is right about everything except flight and reuse. The test
  applied to each member: *would the object still need this if you deleted everything about where it
  came from?* Launch, travel, time out, go back to the pool - yes. Weapons, slots, eggs, enemies -
  no. The alternative was four independent classes behind an `IPooledProjectile` interface, and it
  loses on two counts: the pool has to hand back a common type either way, so the interface is a base
  class with no compiler help; and the clearing of a previous flight's velocity and timer would be
  written four times, at three different stages, which is exactly the pooling bug Exercise 3's own
  `BaseProjectile` comment warns about. **`OnHit` is abstract with no default**, so each subclass
  states its own effect rather than inheriting one - an axe destroys, a fireball harms the player, and
  a shared default would assert a kinship that is not there. The intermediate `DestroyingProjectile`
  that would have held the three destroying subclasses was rejected for the same reason: the mount's
  fire and the נחש's fire fly alike and are opposites in the only way that matters.
- **`Update` lives on the base and `Fly` is the hook.** A subclass declaring its own `Update` would
  hide the base's and silently stop the timeout, which is the same trap `Collectible` has with
  `Awake`. The boomerang's steering goes in `Fly`.
- **Every projectile is a trigger, including the axe.** A dynamic body with a trigger collider still
  falls under gravity and still reports overlaps with static colliders, so the axe arcs and detects
  the ground without any physics response. The alternative was a solid axe reading terrain from
  `OnCollisionEnter2D`, and it fails on the player: his capsule is the one other non-trigger collider
  in the game, so a thrown axe would shove him. **Terrain is then `other.isTrigger == false`**, since
  every hazard, pickup and door in this project is a trigger and the player is excluded by his marker
  component. That answers 6.9's "the projectile has to tell the two apart" with no physics layer, no
  `SC_Floor` marker and no `LayerMask` - the same argument `PlayerGround` makes for using contacts.
- **8.2 costs nothing, the way 9.3 did.** An enemy's projectile must never kill another enemy, which
  he called "חשוב מאוד" and a code-design point (00:41:09). `Destroyer` has no value for an enemy's
  shot, so a נחש's fireball never calls `TryDestroy` at all and a צפרדע jumping into one cannot die.
  The second requirement this project satisfies by leaving something out.
- **The recipes are constants in `ProjectileDirector`, and the pool is filled at startup.** Four
  options were weighed: constants in the Director, serialized fields on `GameInstaller`, a
  ScriptableObject recipe per projectile, and serialized fields on each projectile prefab. The last
  is the baseline a grader will imagine and it is the version with no Builder and no Director in it.
  The installer dies on arithmetic - four projectiles times five numbers against a class that has
  four fields, and `WithArguments` matches by type so a run of floats would be ambiguous. The
  ScriptableObject is the real contender and it loses on one thing: if a recipe is already an asset
  holding five numbers, the shortest honest path is for the projectile to read the asset, and the
  Builder becomes a hop that copies fields. **Constants are the only option where the Builder is
  load-bearing**, because the numbers exist nowhere else. The convention that a number describing an
  object goes on the object does not reach here: it exists to protect *per-instance* configuration
  that a level rebuild would lose, and a projectile is never placed in a level and never varies
  between copies. The cost, worth stating rather than hiding: retuning an axe needs a recompile.
  **Filling the pool at startup** then makes the cap and the pool's size the same number by
  construction, and lets the claim be the strong one - after startup this game never instantiates a
  projectile. It also removes a temporal coupling, since a lazily-creating pool would have to call
  the builder while the director's setters were still loaded.
- **The cap is his number, not ours.** 00:31:35: "אתם לא יכולים לזרוק 10 אלף, אתם זורקים איזה שלוש,
  אחת, שתיים, שלוש" - three axes, and he describes the cap and the delay of 6.7 as the same
  observation. One boomerang, since it returns to the player and having two in the air has no
  meaning. Whether a throw cooldown is kept on top of the cap is stage 13 step 3's, and the argument
  for keeping it is that a cap alone lets all three leave in the same frame, which reads as a bug in
  the recording he watches first.
- **The builder instantiates through the `DiContainer`.** A pooled projectile is then injected like
  anything else, which is what lets the boomerang take the player and a fireball take `IGameFlow`
  without either being threaded through the builder as a setter. The cost is one class depending on
  the container, which is the service-locator shape DI usually avoids - it is the standard Zenject
  idiom for a factory, and the alternative is a builder that carries dependencies as well as numbers.
- **`Range` is on the base, and the axe is the one that does not use it.** The boomerang turns at
  its range, and stage 14's נחש fireball and stage 15's mount fire both stop at theirs, so three of
  the four use it and the axe passes zero. Keeping it off the base was the alternative, and it needs
  either a cast in the builder to reach a boomerang-only setter or a generic `Build<T>`; both trade
  a settled shape for one unused float on one subclass. The base also records `LaunchOrigin`, since
  it owns the launch and a range is measured from it.
- **`OnLaunched` is the third hook, and it exists for the pooling invariant.** The base clears the
  velocity, the angular velocity and the clock; a subclass's own leftovers - the boomerang's
  "have I turned yet" - are cleared in `OnLaunched` for the same reason. That makes the Template
  three varying steps against a fixed sequence: `OnLaunched`, `Fly` and `OnHit`.
- **The boomerang re-aims every frame rather than turning at a rate.** 6.11 says it returns to the
  player's *current* position, and re-aiming makes that literally true and unmissable. A turn rate
  is a fourth number to tune whose only effect is to let it fail. It injects the `Player` marker
  concretely, which is the stated exception to putting a MonoBehaviour behind an interface: the rule
  exists because a MonoBehaviour carries members a consumer has no business calling, and the marker
  has none. It is only reachable at all because the builder instantiates through the container.
- **Catching only counts once it has turned.** It is launched overlapping the player and would
  otherwise be caught on the frame it is thrown - the same guard, for the same reason, that Exercise
  3's boomerang needed. Its own comment there says so.
- **The slot holds a projectile prefab, not a name for a weapon.** Nothing in this game needs to know
  which weapon he has: 3.6 takes it on a פסילה, 6.4 keeps it while riding, and 12.1 to 12.3 are the
  only displays and none of them is a weapon. A `WeaponType` enum was the obvious alternative and it
  costs a map from the enum back to a prefab, which is the switch this whole stage has been avoiding.
  It also collapses the director's public surface to one `Throw(prefab, origin, direction)`, so
  adding a projectile stops growing that class - Peleg's question, and the honest half of the answer.
  The other half: the *recipes* still grow one method each, which is what a Director is, and the
  point they would stop being worth it is around eight or ten, where moving them into one asset per
  projectile beats a file that grows linearly. Four is fixed by the requirements.
- **`WeaponSlot` is a plain C# class subscribing to `GameStarted` and `StrikeLost`.** That is 3.6 and
  6.3 in two lines and it is why `CarriedState` is not written. It follows `StrikesController`'s
  idiom exactly - subscribe in `Initialize`, never unsubscribe, because both live for the scene.
- **No throw cooldown, decided by playing it.** 6.7 names a delay as well as a cap, and the cap turns
  out to be the whole of it: a throw is read on the frame the key goes down, so one press is one axe
  and three axes need three presses. Peleg watched it before deciding, which is better evidence than
  the arithmetic about frame times that argued the other way. `Exercise Adventure Island.md` 6.7 now
  records the decision rather than describing a delay the code does not have.
- **The weapon pickup lives in `Collectibles/` with the rest of the family.** It was going to sit in
  `Player/Weapons/` beside the slot, which splits `Collectible`'s subclasses across two folders for
  no gain - the fruit, the weapons, the three tokens and the פייה are one Template family, and "show
  me the Template" should open one folder.
- **The player answers where his middle is, and that is the marker's one member.** Every sprite is
  anchored at the middle of its bottom cell, so `transform.position` is the player's *feet* - which
  is why the boomerang left from above his middle and came back to his shins. Both the throw offset
  and the return target want the same fact, and holding it as two numbers in two files is how they
  drifted apart. `Player.Middle` reads `Collider2D.bounds.center`, so it cannot disagree with
  wherever his body actually is, and the launch and the return agree by construction. The cost is
  small and worth admitting: the argument for injecting `Player` concretely was that a marker has no
  members a consumer could misuse, and it has one now.
- **The boomerang's loop is two recipe numbers, not a curve in code.** Giving it lift and a little
  gravity makes the way out an arc while the way back stays a straight line to his middle, and two
  different paths between the same two points read as a loop. `Fly` overwrites the velocity once it
  is returning, so gravity only ever acts on the outbound leg. The alternative was driving the
  position along a parametric ellipse, which fights 6.11 directly - a precomputed path cannot track
  a player who has moved. Mirroring the loop the other way is two sign flips, a negative lift and a
  negative gravity scale so it falls upward; tried and reverted, the arc rises.

- **`Enemy` is its own base and not a `Hazard`, although `Hazard`'s own header describes one.**
  "Something dangerous to touch that can also be destroyed" is an enemy exactly, and deriving would
  bring the contact detection, the guard consult, `TryDestroy` and the registration for free. It
  loses on the two steps that are *fixed* in `Hazard`: an enemy has to change `TryDestroy`, which
  now drops something and starts a timer, and `ResetTo`, which now leaves a dead one dead - and a
  subclass that overrides a template's fixed steps is not a subclass of that template. `Hurt` goes
  the wrong way too, abstract in `Hazard` because fire and rock differ and identical across all six
  enemies. The precedent is `Spikes`, which duplicated its trigger code rather than joining for a
  structural reason of the same kind. **The price, stated rather than hidden:** the player detection
  and the `lastTouchFrame` guard exist in two files now, and that guard was a real fix - without it
  one touch costs two פסילות, because 2D sends Enter and Stay together on the step a contact begins.
- **Enemies are kinematic, trigger-only, and moved by writing the position.** Every enemy collider
  has to be a trigger, or `ProjectileAxe` reads it as ground: its terrain test is
  `other.isTrigger == false`, so a solid רוח רפאים would stop an axe that is not allowed to kill it.
  A trigger collider on a dynamic body then falls through the world, which rules physics out for the
  hopping נחש and the צפרדע - so all six run on arithmetic instead, which every one of them was
  already going to be: oscillate, sine, hop, stand, parabola, chase. The `Rigidbody2D` is Kinematic
  and exists only so that Unity is not rebuilding the static collider tree around a moving collider
  every frame. The collider goes on the prefab root, since `GetComponent<IDestructible>()` is what
  every destroyer calls.
- **One `activationRange` on the base, and it is four requirements rather than an optimisation.**
  The base skips `Behave` while the player is further away than that, and that single field answers
  8.14's "starting when the player comes into view", 8.15's stop condition for the shooter, 8.17's
  "triggered when the player comes close" and 8.22's "stops chasing past a set distance" - so no
  subclass ever asks how far away the player is. It is also what makes the ציפור playable at all: a
  bird placed at cell 150 that starts flying at level load has left the level before the player is
  within fifty cells of it, and the same is true of a נחש hopping forward for two minutes. Default
  about 12 units against a camera 17.8 wide, so an enemy wakes shortly before it is on screen; zero
  means always active, which is the static עכביש. **The consequence worth saying out loud:** a bird
  the player outruns parks off-camera instead of despawning. It is invisible, it is what keeps the
  stretch ahead from being empty on arrival, and the bird's own edge check still makes 8.11 literally
  true when it does get there.
- **Each enemy owns its own `CancellationTokenSource`, which reverses Step 3's "one token".** Stage 1
  wrote that one `CancellationToken` cancels every pending respawn, and that implies a shared source
  and a class to own it. Per-enemy wins on three things: the source is created fresh at each death so
  it can never be a stale cancelled one, it is cancelled in `OnDestroy` as well as on a full reset,
  which is what stops a continuation touching a destroyed object when Play mode ends, and it keeps
  the enemy self-contained - which is the whole Async argument, since the alternative being argued
  against is a manager holding timers on the enemy's behalf. **The defense sentence changes with it**:
  "a token cancels its pending respawn when the level resets", not "one token cancels every".
- **Destroyed and switched off are different states, and the ציפור is why.** 3.4 keeps *killed*
  enemies dead across a פסילה; a bird that left at the level's left edge was not killed. So the flag
  is `destroyed`, set only by `Die`, and it is the only thing `AfterStrike` consults. The other half
  of the same rule: **a living enemy goes back to its authored position on a פסילה**. 3.4 says
  nothing about the ones still alive, and leaving them where they stand means the level after two
  deaths has every mobile enemy bunched wherever it happened to be, with the stretch about to be
  replayed empty. It is the line the `Full` path already runs, so it costs nothing.
- **`DestroyedBy` stays abstract although five of the six answers are identical.** Five enemies say
  `Axe | Boomerang | AnimalAttack | Riding | Fairy` and the רוח רפאים says `Fairy` alone, so a
  virtual default would save four lines and make the ghost conspicuous. Abstract wins for `Hazard`'s
  own reason - every subclass states its whole rule where a reader can see it, and "show me the
  Template" opens six files that each answer - and for one more: a `Destroyer` value added later
  cannot be granted to five enemies by silence.
- **Drops stay out of stage 14 entirely, which is the opposite call to stage 12's `IPlayerGuard`.**
  The guard was built two stages early because a miss would have meant editing `Fire`, `Rock` and six
  enemies to insert one line each in exactly the right place. A drop is *one line in the base*, so
  nothing is saved by stubbing it, and an empty `Drop` nobody overrides is dead code against code
  quality rule 5. `DropType` and `IDropFactory` arrive with stage 16 and the base gains its call
  then. **The trap to carry to that stage:** the drop has to be spawned as a sibling and not a child,
  or it is switched off with the enemy that dropped it.
- **The נחש's fireball consults the guard and answers `IDestructible`, and 8.2 still costs nothing.**
  The requirement he called "חשוב מאוד" is satisfied by omission exactly as stage 13 predicted - the
  fireball never calls `TryDestroy`, because `Destroyer` has no value for an enemy's shot, so a
  צפרדע jumping into one cannot die. What it does need is 7.10: a ridden animal absorbs a hit and
  *both* disappear, and `IPlayerGuard.TryAbsorb` takes an `IDestructible`, so the fireball is one and
  its line is `Riding | Fairy` - riding into it and a פייה take it out of the air, and nothing else
  can. The consult ships unexercised, since `TryAbsorb` returns false until stage 15, which is the
  same accepted cost as `Hazard.TryDestroy` shipping with no caller in stage 12.
- **The shooting נחש gets the fireball prefab from an injected `ProjectilePrefabs`, not a serialized
  field.** `WeaponCollectible` holds a serialized prefab and that is right, because the slot passes on
  whatever it is handed without ever comparing it. The pool is keyed on prefab *reference*, so a
  serialized field on the נחש prefab pointing at a different asset than `GameInstaller`'s means `Get`
  returns null and the נחש silently never fires. One source for that reference removes the failure
  rather than documenting it.
- **`Player` gains `FacesRight`, and the marker's justification erodes a little further.** 8.20 needs
  "is the player looking at me", and reading `player.transform.localScale.x` from the רוח רפאים
  reaches around the marker to the thing it exists to represent. Same shape as `Middle`: a fact about
  the player read off whatever actually holds it, so it cannot drift. The cost is that the argument
  for injecting `Player` concretely - a marker has no members a consumer could misuse - is now
  answering for two members rather than none.
- **The respawn runs on real time and nothing is done about it.** `Task.Delay` ignores
  `Time.timeScale`, so a respawn can complete while a popup has the game frozen at zero. Both popups
  end in `StartGame`, which runs a `Full` reset that reactivates every enemy - so the only thing a
  respawn under a popup can do is arrive at the state the restart was about to produce anyway.
  Worth being able to say at the defense, since "your timer does not pause" is the obvious probe and
  the answer is that there is no pause in this game that does not end in a full reset.
- **The enemies take tile ids 11 to 16, and the weapon pickups take none.** `Sprite_Axe.prefab` and
  `Sprite_Boomerang.prefab` carry the `Sprite_` prefix, which the naming rules reserve for things
  placed in a level, so they looked owed 11 and 12 first. Checked rather than assumed: neither
  appears anywhere in `Scene_Game.unity` and neither is in `TilePrefabMap.asset` - stage 13's `A`
  and `B` keys stood in for both, and 10.3 has weapons coming out of eggs. If stage 16 decides they
  are painted after all they append at 17 and 18, which the append rule already allows, so nothing is
  lost by taking 11 to 16 now. **A note for stage 16:** if a weapon only ever drops, those two
  prefabs are misnamed and lose the prefix.

- **`OnAwake` is a fourth hook, and the static עכביש is what asked for it.** A spider that hangs
  still shows one frame and a spider that drops and rises cycles two, so `SpriteCycleAnimator` has to
  be switched off when `moveRange` is zero - and the base owns `Awake`, so a subclass has nowhere to
  cache a component or read its own configuration once. The alternatives both lose. Ticking the
  cycler's own checkbox per instance is a second hand-set value that can disagree with `moveRange`,
  which is the failure `PlayerStart`'s facing and the fireball's prefab reference were both redesigned
  to remove. Making `Awake` `protected virtual` the way `BaseProjectile` does lets a subclass forget
  `base.Awake()` and silently lose its registration, which is the exact trap `Collectible` made
  `Awake` private to close. So the base's private `Awake` calls an empty `OnAwake`, and the subclass
  cannot skip the registration or reorder it. **It only ever disables the cycler, never enables one**,
  because `SpriteCycleAnimator.Awake` switches itself off when it has no frames and component order
  within a GameObject is not guaranteed - re-enabling would resurrect a component that had already
  decided it could not run.
- **`Spider`, not `EnemySpider`.** `Hazards/` holds `Fire`, `Rock` and `Spikes` under `Hazard` with
  no prefix, and that is the closer precedent than `Projectiles/`, where `ProjectileAxe` is prefixed
  only because a bare `Axe` would collide with the weapon the player carries. Six of `Enemy`, `Spider`,
  `Bird`, `SnakeJumper`, `SnakeShooter`, `Frog` and `Ghost` in a folder already called `Enemies/` reads
  better than the prefix repeated seven times. **The prefabs keep `Sprite_Enemy_`**, matching the
  sprite files they are built from and grouping the six together in a flat `Prefabs/` folder that is
  about to hold twenty.
- **The base answers questions about the player rather than handing him over.** `Enemy` injects the
  `Player` marker and exposes `PlayerPosition` and `PlayerFacesRight`; no subclass holds him. Handing
  the marker down as a protected property was the alternative, and this way the one place that depends
  on `Player` is the base, the ghost's "is he looking at me" is read through the same member as the
  activation range, and a subclass cannot start asking him for anything else.

- **The respawn delay is a rule and lives on the installer, not on the enemy.** It was first written
  as `minRespawnSeconds` and `maxRespawnSeconds` per prefab, which is wrong by this project's own
  test: a number describing a rule enters at the composition root and a number describing an object
  goes on the object, and 8.4 states one delay for every enemy there is. `activationRange` and the
  ציפור's speed stay on their prefabs under the same test, since those really do differ per enemy.
  `Enemy` is a scene MonoBehaviour, so `WithArguments` cannot reach it - it is not created from a
  binding - and two bare floats would be ambiguous by type anyway. So the route is the one
  `ProjectilePrefabs` already established: a `[Serializable]` class serialized on `GameInstaller`,
  bound `FromInstance`, injected. The cost is one more class for two numbers, and it buys the second
  use of a pattern that was previously a one-off.
- **The עכביש measures its drop instead of carrying a range, which removes a number rather than
  guarding one.** `moveRange` was authored per instance and nothing stopped it running the spider
  through the floor - Peleg found that immediately. Clamping the range against the geometry was the
  obvious fix and it keeps a field whose only correct value is the one the level already knows. So
  the spider casts a ray down from its authored position at every spawn and stops at the first
  non-trigger collider, which is terrain and nothing else - every hazard, pickup and door in this
  game is a trigger, and the spider's own collider is one too, so it excludes itself with no
  self-check. That is the same terrain test `ProjectileAxe` and `PlayerGround` already make, now in a
  third place, and it is the argument against ever adding a `Ground` layer. What survives is `moves`
  and `moveSpeed`. With nothing solid below it the spider warns and hangs still, rather than dropping
  out of the level. **The cost:** `Physics2D.RaycastAll` allocates, so this is a per-spawn allocation
  rather than a free one, and the nearest hit is picked explicitly rather than trusting the returned
  order.
