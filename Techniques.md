# Techniques

SOLID and the seven techniques the final project requires, reviewed one at a time against the code as
it stands and against the course's own material: what the instructor will ask about at the defense, and
what to say. Written in Stage 20. Why the review is shaped this way is in `HW_Final_PLAN.md`.

The seven are named in `Exercise Adventure Island.md` 13.1: DI, Pooling, Builder, Factory, MVC, Async &
Tasks and Template. 13.2 is the reason SOLID gets a section of its own: the instructor runs an automated
check over the submitted code and grades on the single worst place it finds.

## How each section reads

Every technique gets the same five parts.

1. What the course means. Read from `Course/` itself, in two halves: the lecture notes, and what the
   lesson project actually built. The two differ, and the instructor built the second on camera.
2. Where it lives. The files, and the one `file:line` to open when asked to show it.
3. What the code shows, ending in a verdict: matches, departs on purpose and why, or weak.
4. What could be challenged. The questions to expect, any alternative the Decisions Log rejected, and
   the SOLID problems in this technique's own files, which also go into the candidates list below.
5. The defense sentence, one or two sentences, in English.

## Dependency Injection

### What the course means

The lecture note, `Course/Lesson 12 Dependency Injection/Dependency Injection.md`: a class receives the
objects it needs from outside instead of creating them. It names three kinds, constructor injection
(which it calls the most common), method injection and property injection. Zenject's version is a
`MonoInstaller` whose `InstallBindings` registers bindings, `[Inject] Construct` on a MonoBehaviour, and
the binding types `AsSingle`, `AsTransient`, `FromComponentInHierarchy`, `FromNew` and `FromInstance`.
The VContainer half of the same note adds `ITickable` for logic that needs no MonoBehaviour, and names
what DI replaces: classes finding what they need with `FindObjectOfType`, `GetComponent` or references
dragged in the Inspector.

The lesson project, `Lesson 12.md`: three bindings, `Bind<IFireballBuilder>().To<FireballBuilder>()`,
`Bind<FireballDirector>()` and `Bind<FireballPoolSystem>().FromComponentInHierarchy()`, all `AsSingle`.
One injection of each kind: a constructor (`FireballDirector` takes `IFireballBuilder`), a method
(`FireballPoolSystem.Construct(FireballDirector)`) and a field (`[Inject] private FireballPoolSystem` on
`FireballWeapon`). It replaced a static `Instance` and a builder and director made by hand in `Awake`.
Two of its three consumers take a concrete class.

The transcript adds that there are two ways to do DI (00:07:15) and that a student lost marks for not
using it at all (00:53:19).

### Where it lives

- Open `Assets/Scripts/Installers/GameInstaller.cs:41`, `InstallBindings`: 27 bindings.
- 25 MonoBehaviours receive their dependencies through `[Inject] Construct`, and the container builds 15
  plain C# classes through their constructors.
- Pooled projectiles and drops are instantiated through `IInstantiator`, in `ProjectileBuilder.Build`
  and `DropFactory.Create`, so an object made at runtime is injected like one placed in the scene.
- If asked who receives: `State/GameFlow.cs:23` for a constructor, `Enemies/Enemy.cs:34` for a
  `Construct` with seven dependencies.

### What the code shows

- One idiom per kind of class: a MonoBehaviour always takes a `Construct` method, a plain class always a
  constructor. No field injection, which the lesson used once.
- Everything the lesson used, and more of Zenject besides: `WithArguments` for the rule numbers,
  `BindInterfacesTo`, `ITickable` and `IInitializable`, `FromComponentsInHierarchy` for the level roots,
  and `IInstantiator` for objects made at runtime.
- The injected fields are private, carry no `[SerializeField]`, and none of their names appears in
  `Scene_Game.unity`. The installer's own serialized numbers and prefabs do, which is configuration
  entering at the composition root.
- The order is guaranteed, not lucky. `SceneContext` runs at execution order -9999, so every scene object
  is injected before any `Awake`. `SceneKernel` runs `Initialize` from its `Start` at -9997, before
  `GameStarter.Start`. `InstantiatePrefab` instantiates inactive, injects, then activates
  (`DiContainer.cs:1703`, `2044`, `2052`). A base class's `Construct` runs before its subclass's
  (`CallInjectMethodsTopDown`, `DiContainer.cs:1471`).
- The concrete injections each have a reason. `SessionState` goes into `GameFlow`, the one class that
  changes it, while the displays read `ISessionState`. The `Player` marker answers only where he is and
  which way he faces. `ProjectilePrefabs`, `ProjectileCounts` and `RespawnDelay` hold data. Every
  service a class receives is behind an interface apart from those.
- Still outside the container: the static `GameLog`, `Time` and `Keyboard.current` read where they are
  used, and `GetComponent` between the player's own components, which are parts of one object rather
  than services.

Verdict: matches the course's definition and the lesson's usage. The review changed four things, all
recorded in the plan: `IInstantiator` replaced `DiContainer` in the builder and the drop factory, `Levels`
stopped searching the scene, the camera stopped holding the player's Transform, and three unused
self-bindings went.

### What could be challenged

1. "Is this real DI, or Inspector references with a new name?" None of the injected fields is serialized,
   and none of them is in the scene file.
2. "Your builder and your factory get the container. Isn't that a service locator?" They get
   `IInstantiator`, which can create and inject a prefab and cannot resolve anything.
3. "Why does `GameFlow` take `SessionState` and not an interface?" It is the one class that changes it.
   The displays read it through `ISessionState`, and lesson 12's own pool takes its director as a
   concrete class.
4. "Where are the levels found?" In the installer, by `FromComponentsInHierarchy`, the plural of the
   binding lesson 12 uses for `FireballPoolSystem`. It includes the inactive level 2.
5. "What calls `Tick`?" `SceneContext` binds a `SceneKernel`, whose `Update` drives Zenject's
   `TickableManager`.
6. "A base class and its subclass both have a `Construct`. Which runs?" Both, the base's first. The
   base's is private, so the subclass's cannot hide it.
7. "`Collectible` uses its registry in `Awake`. How do you know it was injected by then?" For scene
   objects, `SceneContext` runs at -9999. For spawned ones, `InstantiatePrefab` injects before it
   activates.
8. "Twenty-seven bindings in one installer. Is that a god class?" Its one job is deciding what goes where.
   Zenject allows several installers, and splitting this one would add files without changing a single
   dependency.
9. "Your plain classes still read `Time`. Where is the testability?" They are MonoBehaviour-free, not
   Unity-free. There are no tests, and an injected clock with one implementation would be an abstraction
   before a second use case.
10. "Why not field injection, like `FireballWeapon`?" A `Construct` method lists everything a
    MonoBehaviour depends on in one signature, which is as close as a MonoBehaviour gets to a
    constructor.

Rejected along the way, all in the Decisions Log: VContainer, which was the fallback had Zenject not
compiled on Unity 6; an `IInputService` and an injected clock, each of which would have had one
implementation.

Nothing from these files goes on the worst-spot list. The installer's size is question 8.

### The defense sentence

> Every class declares what it needs, in a constructor or an `[Inject] Construct` method, and
> `GameInstaller` is the one place that decides which class answers each request, so the game's rules
> live in plain C# classes that never go looking for anything. It isn't Inspector wiring under another
> name: none of the injected fields is serialized, and none of them appears in the scene file.

## Builder and Object Pooling

One section, because the two are one chain in the course and in this project: the builder makes what
the pool holds.

### What the course means

Builder, the lecture note `Course/Lesson 09 - Pooling & Builder Patterns/Builder Pattern.md`:
construct a complex object step by step, separating the construction from the result, so the same
process can produce different results. Four roles: a builder interface declaring the steps, a concrete
builder implementing them and handing back the product, a director running the steps in order, and the
product. In its car example the values live in the concrete builder (`SportsCarBuilder` sets "V8
Engine"), so a different car is a different builder class.

Builder, the lesson project (`Lesson 09.md`): `IFireballBuilder` has `SetSpeed()`, `SetLifeTime()` and
`Build(prefab)`; `FireballBuilder` hardcodes 400 and 2; `FireballDirector.ConstructFireball()` calls the
two setters and `Build` passes through. The walkthrough names its thinness itself: there is no way to
ask for a different fireball without writing a second builder class.

Pooling, the lecture note `Object Pooling System.md`, and the pooling section of lesson 10's
`Performance & Optimization.md`: create the objects ahead of time, hand out an inactive one, switch it
off instead of destroying it, reset its state before reuse, and optionally cap how many are active. Its
`BulletPool` returns the first inactive bullet, or null.

Pooling, the lesson project: `FireballPoolSystem` owns the prefab and the quantity, builds five through
the director in `Start`, and `GetPooledFireball` returns the first inactive one. `FireballWeapon` asks it,
positions the fireball, activates it and fires. The walkthrough names two gaps: velocity is never
cleared before reuse, and nothing is said when the pool runs dry. Lesson 12 changes only the wiring:
the singleton goes, and the director is injected into the pool.

The transcript places the pool itself, watching the original throw axes (00:34:50): "לא יכול שהרבה
יהיו חיים. ופה אני רוצה שיהיה פולינג". The cap and the pool are one request, and 00:31:35 gives the
three.

### Where it lives

- Open `Assets/Scripts/Projectiles/ProjectilePool.cs:85`, `Get`, for the pool, and
  `ProjectileDirector.cs:68`, `Build`, with the recipes below it from line 92, for the builder.
- `IProjectileBuilder.cs` and `ProjectileBuilder.cs:50`: the steps, and the build.
- `ProjectilePool.cs:33`: the pool filling itself at startup. `ProjectileCounts.cs`: how many of each,
  serialized on `GameInstaller`.
- `BaseProjectile.cs:47`, `Launch`: what makes a reused projectile safe.
- The three shooters: `PlayerAttack.cs:59`, `SnakeShooter.cs:90`, `PlayerMountAttack.cs:79`.

### What the code shows

The chain is the lesson's with four kinds instead of one: a shooter asks the pool, the pool was filled
through the Director, and the Director drives the builder.

- The builder's setters take values, a deliberate departure from lesson 9. There is one builder, and
  the four kinds differ in the Director's four recipes; the lesson's shape would have been four builder
  classes with identical code and different constants. Exercise 3's `LaserBuilder` took values too.
- `ProjectileBuilder.Build` is where the builder earns its place: it instantiates through `IInstantiator`
  so the projectile is injected, finds `BaseProjectile`, and applies the numbers. It does nothing else.
- `ProjectileDirector.Build(prefab)` picks the recipe for that prefab, runs it, and builds, in one
  call, so no build can happen in between with numbers another recipe left in the builder. A delegate
  per kind stands where a switch over kinds would be.
- `ProjectilePool` does the lesson pool's job: it builds every kind at startup, parks each one under
  `Projectiles` switched off, and `Get` hands out the first inactive copy. A copy is free when it is
  inactive, so there is no return call to forget and nothing can be returned twice.
- It closes both of lesson 9's gaps. `Launch` clears velocity, spin and the clock, and a subclass clears
  its own leftovers in `OnLaunched`. A refusal logs `throw ignored - every copy is already in flight`.
- One pool keyed by prefab, not a pool class per projectile. The axe, the boomerang and the mount's fire
  never grow, so a fourth axe cannot exist; the snake's fireball grows, since no one can know how many
  shooters a level holds, and pools never shrink.
- The counts are in the Inspector, as `Exercise Adventure Island.md` asks of every number it writes in
  backticks.

Verdict: Builder matches the course with one departure that is argued for, and Pooling matches it and
closes the gaps its own walkthrough named. The review split `ProjectileDirector`, which had been holding
the recipes, filling and growing the pool, and throwing, and moved the counts out of code; the plan's
Decisions Log has both.

### What could be challenged

1. "My builder had fixed numbers. Where do yours vary?" In the Director's four recipes. The other shape
   is one builder class per projectile, identical except for constants.
2. "Your builder ends in one five-argument `Configure`. What does it add?" The Director names the steps
   and sets them from a recipe, and `Build` is the one place that turns those numbers into a live,
   injected projectile.
3. "Isn't this the laser again?" The laser had one recipe, and a singleton pool for one class. This has
   four recipes and one pool for all of them, keyed by prefab.
4. "A pool that grows isn't a pool." Only the snake's grows, and it stops once it holds as many as have
   ever been in the air at once. With extra shooters added to level 1 for testing it reached 27 and
   stopped allocating. The player's counts are rules and never grow.
5. "What stops a reused axe carrying the last flight's velocity?" `Launch` clears it before every
   flight. That was lesson 9's own gap.
6. "How does a projectile go back to the pool?" It switches itself off. Free means inactive.
7. "What if an axe falls into a pit?" Every recipe has a maximum time, and the axe despawns when it runs
   out.
8. "Make it five axes." Change **Axe** under **Projectile Counts** on `GameInstaller`, before pressing
   Play.
9. "Why isn't the mount's hit pooled?" There is only ever one. It is a single object switched on and
   off, which is what a pool of one would be.

Rejected along the way, all in the Decisions Log: a Builder for the mounts, which became a
`MountDefinition` asset per mount; the recipes as ScriptableObjects, as installer fields or on the
prefabs, each of which leaves the builder copying numbers that already exist somewhere; not pooling the
enemies' shots, which is what Exercise 3 did; and counting the shooters at startup to size their pool.

The one worst-spot candidate these files held, `ProjectileDirector`, was resolved by the split.

### The defense sentence

> A projectile is five numbers and there are four kinds, so the Director holds the four recipes and one
> builder turns any of them into a configured, injected projectile. The pool builds each kind through
> the Director at startup and hands out an inactive copy, which is your fireball's chain with four kinds
> instead of one: the shooter asks the pool, the pool asks the Director, the Director drives the builder,
> and the axe's three is a rule the pool enforces by never building a fourth.

## Factory

Not started.

## Template Method

Not started.

## MVC

Not started.

## Async & Tasks

Not started.

## SOLID

Not started.

### Worst-spot candidates

Collected by every section above and ranked at the end. Found by reading the code before the review
began, so none of these is verified as a problem yet:

- `Enemy`: 323 lines and seven injected dependencies.
- `LevelWindow` (314 lines) and `TilePlacerWindow` (306). Editor tooling ships with the submission.
- `IGameFlow`: ten members, and no client uses both the operations and the events. Splitting it was
  considered and rejected in the Decisions Log.

Resolved during the review, so no longer candidates: `ProjectileDirector`, which held the recipes,
filled and grew the pool, and was the `Throw` three shooters injected concretely. Split in the Builder
and Pooling step.

### Reflection

Not started.

### Patterns considered and rejected

Not started.
