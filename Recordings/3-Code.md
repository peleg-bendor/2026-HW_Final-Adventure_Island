# Code Recording Script — `3-Code`

Recording script for the third of the final project's three recordings: the code, part by part. Capped at
15 minutes. It walks the project the way `Techniques.md` is ordered, with the state and reset design as a
part of its own, since that is what holds the game together.

## How to read this

- Lines in `>` blocks are said out loud. Everything else is a stage direction and is never spoken.
- One sentence per line inside a `>` block. Each line is one breath.
- `### **SHOW**` is a major beat, a new file. Plain `**SHOW**` is a small move inside it: scroll, flicker
  to a method, press a key.
- The time in a part's heading is its spoken words at 160 a minute, so a take that runs much longer than it
  is being read too slowly for the budget.
- Every part is its own take, edited together afterwards. Nothing changes the project between takes.
- Spoken in English. The greeting and the sign-off are the only Hebrew, and no requirement numbers are
  said.
- Each part explains one thing: what the code does, where it is, and why it is shaped that way. Where the
  shape differs from the obvious one, the reason is the point of the part.

### About the timings

Written to 160 spoken words a minute, the rate from Exercises 1 to 3 where the camera is on a file.

| Part | Content                | Words | Time |
| ---- | ---------------------- | ----- | ---- |
| 1    | Intro                  | 57    | 0:21 |
| 2    | Dependency injection   | 266   | 1:40 |
| 3    | The flow and the reset | 205   | 1:17 |
| 4    | Builder and Pooling    | 245   | 1:32 |
| 5    | Factory                | 240   | 1:30 |
| 6    | Template               | 274   | 1:43 |
| 7    | MVC                    | 184   | 1:09 |
| 8    | Async and Tasks        | 253   | 1:35 |
| 9    | SOLID                  | 287   | 1:48 |
| 10   | Reflection             | 116   | 0:44 |
| 11   | The level pipeline     | 86    | 0:32 |
| 12   | Sign-off               | 5     | 0:02 |

**2218 words, about 13:53**, measured from this file by script, against the 15-minute cap.

If a read comes out slower than the budget, cut in this order:

1. **Part 11, the level pipeline.** The tools are not part of the game, and the levels are explained by
   what they hold rather than by how they are built.
1. **Part 10's middle**, where reflection was planned and why it was dropped. What stays is that Zenject
   finds every `Construct` through it.
1. **Part 5's three rules at the end.** The factory is still explained; what goes is what a plain
   `Instantiate` on the enemy would have got wrong.

What not to cut, at any length: part 3, part 6's note on what C# cannot guarantee about a base class,
part 8's two differences between a Task and a coroutine, and part 9's two largest classes. Each of those is
a place where the code is not the obvious shape, and the reason is the whole point of saying it.

## Before you record

- Two windows: VS Code for every part, and Unity for one beat at the end of part 2 and one in part 11.
- Unity, for those two beats: Console **Clear on Play** on, **Collapse** off, **Error Pause** off, and the
  Console visible. `LogSettings` as saved.
- The line numbers in the SHOW lines were checked against the current files on 16.9.2026. Check them again
  if any code changes, and re-check `Techniques.md` with them.
- Open every file a part needs as VS Code tabs beforehand, in order:
    - **2** — `Installers/GameInstaller.cs`, `State/GameFlow.cs`, `Enemies/Enemy.cs`
    - **3** — `State/GameFlow.cs`, `State/ResetRegistry.cs`, `State/IResettable.cs`, `Levels/Levels.cs`
    - **4** — `Projectiles/ProjectileDirector.cs`, `Projectiles/IProjectileBuilder.cs`,
      `Projectiles/ProjectileBuilder.cs`, `Projectiles/ProjectilePool.cs`, `Projectiles/BaseProjectile.cs`,
      `Projectiles/ProjectileCounts.cs`
    - **5** — `Collectibles/DropFactory.cs`, `Collectibles/IDropFactory.cs`, `Collectibles/DropType.cs`,
      `Enemies/Enemy.cs`, `Collectibles/Egg.cs`
    - **6** — `Enemies/Enemy.cs`, `Projectiles/BaseProjectile.cs`, `Collectibles/Collectible.cs`,
      `Hazards/Hazard.cs`
    - **7** — `MVC/Power/PowerController.cs`, `MVC/Power/PowerModel.cs`, `MVC/Power/PowerView.cs`,
      `State/SessionState.cs`, `MVC/Fruit/FruitController.cs`
    - **8** — `Enemies/RespawnCountdown.cs`, `Player/PlayerFairy.cs`, `State/GameFlow.cs`, `UI/Popup.cs`
    - **9** — `State/ResetRegistry.cs`, `Mounts/MountStrike.cs`, `Enemies/Enemy.cs`, `State/IGameFlow.cs`
    - **10** — `Plugins/Zenject/Source/Util/ZenReflectionTypeAnalyzer.cs`
    - **11** — `Assets/Levels/Level01.txt`, `Editor/LevelFile.cs`, `Editor/LevelScene.cs`

---

## Part 1 — Intro `0:21`

### **SHOW** — VS Code, the `Assets/Scripts` tree expanded.

**SAY:**

> שלום, זה פלג בן דור, וזאת ההגשה שלי לפרויקט הסופי.
>
> This is the code recording for Adventure Island.
> I'll start with how the game is wired, then the state and the reset, then the seven design patterns it uses, and SOLID at the end.

---

## Part 2 — Dependency injection `1:40`

### **SHOW** — `GameInstaller.cs`, then flicker to `InstallBindings` (line 41) and scroll it once.

**SAY:**

> This is the installer, and it's the only place in the project that decides which class answers which interface.
> Thirty-one bindings: the flow, the levels, the reset registry, the HUD triads, the projectile pool, the two slots and the drop factory.
> Nothing else goes looking for anything: a class states what it needs and gets it.

**SHOW** — `GameFlow.cs` line 23, then `Enemy.cs` line 32.

> A plain C# class takes its dependencies in its constructor, like the flow here.
> A MonoBehaviour can't have one, so it declares a private `Construct` method instead. The enemy's lists seven things.
> Keeping them in one signature means a class's whole dependency list reads at a glance.

**SHOW** — Back to `InstallBindings`, flickering to the argument, array and transient bindings as they're named.

> The rule numbers arrive here as arguments: three strikes, a strike every twenty fruit, a bar of sixteen, three seconds a unit.
> Both level roots come in as an array from a single binding, the inactive level 2 included, so nothing has to search the scene for them.
> The respawn countdown is bound per instance, so every enemy owns its own rather than sharing one.
> Anything created while the game runs goes through `IInstantiator`, which can instantiate and inject and nothing else, so a dropped pickup arrives with its dependencies filled in.
> All the injected fields are private, and none of them is serialized, so this wiring is in code and not in the scene file.
> The order is deterministic. Zenject's scene context runs ahead of every `Awake`, and the two logging objects run one place earlier still, so the log file is open while the container is being built.

### **SHOW** — Unity. Press Play once, with the Console visible, then stop.

> A session opens with the container, the four pools and the six drops.

---

## Part 3 — The flow and the reset `1:17`

### **SHOW** — `GameFlow.cs`, scrolled from the top: `StartGame`, `LoseStrike`, `CompleteLevel`.

**SAY:**

> This is the game's flow: starting a game, losing a strike, taking a fruit, finishing a level.
> It's a plain class rather than a MonoBehaviour, so the rules aren't attached to an object in the scene.
> Nothing here loads a scene. There's no `SceneManager` call in the project, so a death and a game over have to put the level back themselves.

**SHOW** — `ResetRegistry.cs` line 22, then `IResettable.cs`.

> That's this registry. It holds a list of `IResettable` and calls all of them, and it never learns which kinds exist.
> Each object decides what a reset means for it: the player returns to the start marker, fruit and eggs come back, a killed enemy stays dead until its own countdown ends, and a pickup that fell from an enemy destroys itself.
> A strike resets the level the player is in. A level change is the same reset with a wider scope, which is also what clears the fairy and refills the bar.

**SHOW** — `Levels.cs` line 49.

> Moving to level 2 switches which level root is active. Both levels sit in the one scene.
> While a popup is up the game is frozen with the time scale, which stops the power drain as a side effect, so there's no paused state for every input reader to check.

---

## Part 4 — Builder and Pooling `1:32`

### **SHOW** — `ProjectileDirector.cs` line 68, then scroll down through the four recipes.

**SAY:**

> Four things fly in this game: the axe, the boomerang, the snake's fireball and the red mount's flame.
> Each one is five numbers: speed, lift, gravity, how far it reaches and how long it lives.
> The director holds one recipe per kind, and `Build` runs a recipe and builds in the same call, so no build can happen in between with another recipe's numbers.

**SHOW** — `IProjectileBuilder.cs`, then `ProjectileBuilder.cs` line 50.

> The builder takes those five values a step at a time and then stamps them onto a new instance.
> It knows how to apply them and nothing about what they should be.
> Its setters take values, unlike the builder in lesson 9, where the numbers lived in the builder itself. With four kinds that would have meant four builder classes differing only in constants.

### **SHOW** — `ProjectilePool.cs` line 33, then `Get` at line 85.

> The pool builds every kind at startup through the director, and parks the copies inactive under one object.
> Handing one out is finding the first inactive copy. Free means inactive, so nothing has to be returned and nothing can be returned twice.

**SHOW** — `BaseProjectile.cs` line 55, `Launch`.

> `Launch` is what makes reuse safe: it clears the velocity, the spin and the clock the last flight left.
> Three axes in the air is a game rule, so a fourth request is refused and logged. The snake's fireball is the only kind allowed to grow, since how many shooters a level holds isn't known here.

**SHOW** — `ProjectileCounts.cs`.

> The counts are serialized on the installer, so the axe's three is an Inspector field and not a constant.

---

## Part 5 — Factory `1:30`

### **SHOW** — `DropFactory.cs` line 67, `Create`.

**SAY:**

> Enemies and eggs drop things: the three mount tokens, the two weapons and the fairy.
> This factory makes them. A caller asks for a drop type at a position and gets back a finished pickup.
> The textbook shape is an abstract creator with a subclass per product. This is the simpler one, like the laser factory in my third exercise: a single class that hands back a ready object, so nothing else learns how it's built.
> It isn't the creator-per-product shape because a drop is chosen by data on each enemy and egg, so the caller holds a drop type and not a factory. Six creators would be one line each and would still need a lookup from type to creator.

**SHOW** — `DropFactory.cs` line 25, `Initialize`, then `DropType.cs`.

> That lookup is built once at startup, from the six pickup prefabs listed on the installer, each declaring its own type. A seventh drop is a prefab and one value in this enum, and this file doesn't change.

**SHOW** — `Enemy.cs` line 252, then `Egg.cs` line 78.

> Both callers hold the interface and a type, and neither one names a concrete pickup class. `Create` returns the abstract `Collectible`.
> Three rules live here that a plain `Instantiate` on the enemy would get wrong. The pickup is built through the container, so a mount token arrives with the mount slot injected. It's parented to the level, so it isn't switched off with the dead enemy. And it's marked as dropped, so the next reset destroys it instead of restoring it.

---

## Part 6 — Template `1:43`

### **SHOW** — `Enemy.cs` line 146, `Update`.

**SAY:**

> Four base classes here hold a fixed sequence and leave the varying steps to their subclasses: the enemy, the hazard, the collectible and the projectile.
> This is the enemy's frame: the base asks whether the player is near, or whether this one is part-way through a hop or a swoop, and only then calls `Behave`, the one method each kind writes.

**SHOW** — `Enemy.cs` line 202, `TryDestroy`, then line 276, `Spawn`.

> A death runs in an order no subclass can change: switch off, show it going, drop what it was carrying, start the countdown that brings it back.
> Coming back has one path for both its causes, the countdown and a level start.
> There are six enemies, and each one writes two things: what it does while it's alive, and what is allowed to destroy it.

**SHOW** — `BaseProjectile.cs` line 55, then `Collectible.cs` line 50 and `Hazard.cs` line 61.

> The projectile base is the same idea around a launch, with the flight and the hit left to each kind.
> A collectible switches itself off before its effect is applied, so that a strike can bring it back. A hazard takes one contact a frame, offers it to the player's guard, and only then hurts him.
> All four keep their Unity messages private and call an empty hook instead, because a `protected virtual Awake` that a subclass forgets to call up leaves the object half built.
> Worth knowing: nothing in C# stops a subclass declaring its own `Awake`, and Unity would call that one and skip the base's. What guards it is the private message, the hook, and the header saying so.
> Where a base stops was a decision too. The egg isn't a `Collectible`, because its whole behaviour is a beat between that base's two fixed steps.

---

## Part 7 — MVC `1:09`

### **SHOW** — `PowerController.cs` line 62, `Spend`.

**SAY:**

> Three things on screen have a model, a view and a controller of their own: the power bar, the strikes and the fruit count.
> Power is the complete triad. Time, fruit and rocks are its input, the controller changes the model, and then it tells the view to redraw.

**SHOW** — `PowerModel.cs`, then `PowerView.cs`.

> The model holds the number and its own rule: never above sixteen, never below zero. It has no opinion about what zero means.
> An empty bar costing a strike reaches another system, so that lives in the controller.
> The view only draws lines, and decides nothing.
> The controller depends on interfaces for both of them, its model is injected rather than constructed, and it's a plain class that Zenject ticks, so none of this needs a MonoBehaviour.

**SHOW** — `SessionState.cs`, then `FruitController.cs`.

> The other two share one model, the session, which is what survives a death and a level change: the strikes left and the fruit taken.
> Their controllers redraw when it changes. The changes themselves are made by the flow, because losing a strike and taking a twentieth fruit are decisions about the game rather than about a display.

---

## Part 8 — Async and Tasks `1:35`

### **SHOW** — `RespawnCountdown.cs` line 19, `Begin`.

**SAY:**

> There are two Tasks in this project and three coroutines, and two differences decide which is which.
> A coroutine needs an active GameObject and follows Unity's scaled clock. A Task needs neither.
> This is an enemy's respawn countdown. A killed enemy is switched off at once, and Unity stops coroutines on a disabled object, so this wait can't be one.
> It's an `async void` with the whole wait inside try and catch, and a cancellation token: a level start cancels every pending countdown, since it brings the enemies back itself.
> The token source is also held in a local, so a cancel or a second death between the delay ending and the rest of the method running is caught.

**SHOW** — `GameFlow.cs` line 131, `EndGame`, then `Popup.cs` line 15.

> The other Task is the popup, for a different reason: the class that waits is the flow, which isn't a MonoBehaviour, so `StartCoroutine` doesn't exist here.
> The popup's button completes a `TaskCompletionSource`, which turns a click into something awaitable, and the whole ending reads as one method: freeze, wait, start again.

**SHOW** — `PlayerFairy.cs` line 63, `Hold`.

> The fairy's ten seconds are a coroutine, because the player is never switched off, and the wait should stop while a popup is up. `WaitForSeconds` does that and `Task.Delay` doesn't.
> The egg's crack and the puff when something dies are the same case.
> One consequence: a respawn keeps counting in real time under a frozen screen. Every freeze here ends in a reset that cancels the countdowns, so an enemy arriving during a popup is one the restart was about to bring back.

---

## Part 9 — SOLID `1:48`

### **SHOW** — Unity, the `Player` GameObject selected, the Inspector scrolled through its twelve scripts.

**SAY:**

> A few facts about the codebase: no switch over types and no type check anywhere in the game code, 29 interfaces, and four base classes with sixteen subclasses between them.
> Whatever varies by kind is a subclass, an interface, or data set on the object.
> The player is twelve small scripts instead of one: moving, jumping, the ground check, the reset, the attack key, the guard, the fairy, the mount's body and attack, two animators and a marker. He was split from the first stage, since a player class is where everything ends up otherwise.

**SHOW** — VS Code: `ResetRegistry.cs` line 5, then `MountStrike.cs` line 57.

> The reset registry is the clearest open-closed spot: it walks `IResettable` and never learns the kinds, so a new enemy is a subclass and a prefab.
> It sits behind two interfaces, one to register and one to run a reset, so an enemy can add itself to the list and can't start one.
> The mount's hit calls `TryDestroy` on whatever it touched, and an enemy, a rock and a fire each answer by their own rule without breaking the contract.

**SHOW** — `Enemy.cs`, scrolled to show its length, then `IGameFlow.cs`.

> The two largest things in the project are worth explaining.
> The enemy base is 308 lines with seven dependencies, 148 of them code. It has one reason to change, the lifecycle every enemy shares, and each dependency serves one step of it: the strike on contact, the guard, the reset, the countdown, the player's position, the drop and the death effect. The countdown was a second job, and it moved into a class of its own.
> The flow's interface has seven members, and no class that uses it uses more than two. Splitting it would produce two interfaces with one implementation and one lifetime between them, which is bookkeeping rather than separation.

---

## Part 10 — Reflection `0:44`

### **SHOW** — `ZenReflectionTypeAnalyzer.cs` line 76.

**SAY:**

> There's no reflection in this project's own code, and that was a decision rather than an omission.
> It was planned for the drop factory, to replace a switch over drop types with an attribute on each pickup class. By then there was no switch to replace, and the attribute couldn't work: one class serves all three mount tokens, so it can't declare which it is.
> Nothing in this game has a type that isn't known at compile time, which is the situation reflection is for.
> Where it does run is here, inside Zenject: it finds every `Construct` method by its attribute and calls it through a `MethodInfo`. A private `Construct` couldn't be called any other way.

---

## Part 11 — The level pipeline `0:32`

### **SHOW** — `Assets/Levels/Level01.txt`, then `LevelFile.cs` and `LevelScene.cs`. Then Unity: `Tools > Tile Placer`, one tile painted and erased.

**SAY:**

> The levels are text files, one tile id per cell, in Tiled's format, although nothing here is authored in Tiled.
> One editor tool builds a file into the scene and writes the scene back out to the file, and another paints and erases single tiles.
> Building is a diff and not a teardown: a cell that already holds the right prefab is left alone. That's what lets data set on a placed object survive a rebuild, like an egg's contents or how far a bird dips.

---

## Part 12 — Sign-off `0:02`

### **SHOW** — Anything. The `Assets/Scripts` tree is fine.

**SAY:**

> That's the project. תודה רבה.