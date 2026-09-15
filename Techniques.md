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

### What the course means

The course uses the word two ways, and the instructor wrote both.

The lecture note, `Course/Lesson 10 - Factory, Template, Performance & Optimization/Factory Design
Pattern.md`, is Factory Method: an abstract creator declares a factory method, and a concrete creator
subclass overrides it for each product. Its reasons for using one are complex creation, decoupling
creation from use, adding types without editing existing code, and one central place for creation
logic such as caching, logging or configuration.

The lesson projects (`Lesson 10.md`, and again `Lesson 11.md`) build exactly that: `MarioEnemyFactory`
as the abstract creator, `GoombaFactory` and `KoopaFactory` as its subclasses, and a client holding a
`MarioEnemyFactory`. The caller picks the product by picking the factory object. Lesson 10's own
walkthrough faults two things in it: the factory method returns `GameObject` rather than the product
type, and the client asks for `GetComponent<GoombaEnemy>()`, naming the concrete class the pattern
exists to hide.

Exercise 3's text (`Course/Exercises/Exercise 03.md:15-18`) defines the Factory the instructor graded
differently: "צרו קובץ LaserFactory שמחזיר קליע לייזר מוכן, באמצעות הבילדר. המטרה היא לאפשר למערכות
המשחק להשתמש בנשק החדש בלי להכיר את תהליך הבנייה". A class that hands back a finished object, so nothing
else learns how it is built, with no creator hierarchy. At 00:54:05 he points back at that exercise:
"פקטורי עשינו, עשינו אפילו את כולם ביחד".

The transcript on drops: define it in the development and make it smart, with dropping nothing as an
option (00:46:39), and entering what each enemy drops preferred over rolling it (00:37:46).

### Where it lives

- Open `Assets/Scripts/Collectibles/DropFactory.cs:67`, `Create(DropType, Vector2)`.
- `IDropFactory.cs` is the abstraction both callers depend on; `Collectible` is the abstract product
  `Create` returns.
- The callers: `Enemies/Enemy.cs:243` and `Collectibles/Egg.cs:78`, each holding only an `IDropFactory`
  and a `DropType`.
- `DropFactory.cs:25` builds the mapping once from the six `Pickup_` prefabs listed on `GameInstaller`,
  each declaring its own `DropType`.

### What the code shows

- Exercise 3's definition exactly. A caller says what and where. The factory alone knows which prefab
  that is, instantiates it through `IInstantiator` so the pickup is injected, parents it under the
  active level, and marks it dropped so the next reset destroys it rather than restoring it.
- Not the lecture's structure: one concrete creator, with the product chosen by a parameter rather than
  by a creator subclass.
- Right on both counts lesson 10's walkthrough faulted: `Create` returns the abstract `Collectible`, and
  neither caller names a concrete class.
- Every reason the note gives for a factory: creation that is more than `Instantiate`, two unrelated
  callers kept apart from it, a new drop added as a prefab on the installer with no edit to the factory,
  and the rules and the logging in one place. The limit worth stating: a new drop type also adds a value
  to `DropType.cs`.
- No `switch` over drop types anywhere.

Verdict: matches the course's definition of a Factory, Exercise 3's and the note's reasons for one, and
not the lecture's Factory Method structure. That difference is answered rather than changed; the plan's
Decisions Log has why.

### What could be challenged

1. "That isn't the Factory I taught. Where are the creator subclasses?" In Factory Method the calling
   code picks the product by holding a particular factory. Here it is picked by data authored on each
   enemy and egg in the Inspector, since drops are configured and never rolled, so the caller holds a
   `DropType` and not a creator. Six creator subclasses would share one line of body and still need a
   map from the type to the creator, which is the dictionary again. It is the Factory from Exercise 3.
2. "Why not a prefab reference on the enemy?" A plain `Instantiate` there makes a pickup that is not
   injected, so a token has no mount slot; is parented to the enemy, so it is switched off with it; and
   is handed back by the next reset instead of destroyed. The factory is the one place those three rules
   live.
3. "Why an enum and not a prefab field?" The enum is a dropdown of the six drops and `None` on every
   enemy and egg in both levels. A prefab field would accept a fruit, an enemy or a projectile dragged in
   by mistake, and only fail at the moment of the drop. The cost is a line in `DropType.cs` per new type.
4. "Why do projectiles use a Builder and drops a Factory?" Projectiles of a kind differ in numbers set
   step by step, and are built once and pooled. Drops differ in which prefab they are, and are made on
   demand and destroyed on a reset.
5. "Why doesn't `DeathEffects` use the factory?" It makes effects, not pickups: there is nothing to
   inject, it is placed beside whatever was destroyed, and it removes itself within about a second, so a
   reset has nothing to clear.
6. "Why not reflection, like `EnemyCreator`?" It was planned for this factory and dropped, since there
   was no switch for it to remove and one class serves several drop types. The Reflection part of the
   SOLID section has the full reasoning.

Rejected along the way, all in the Decisions Log: Factory Method's creator-per-product shape, for the
reason in question 1; a random drop, which the requirements refuse and the instructor called the weaker choice; and a list
of drops per enemy, since what that line of the transcript asks for is every type seen early, which is
the levels' job.

Nothing from these files goes on the worst-spot list.

### The defense sentence

> An enemy and an egg each hold a drop type set in the Inspector and ask the factory for it by name; only
> the factory knows which prefab that is, builds it so it is injected, puts it in the level rather than
> under whatever dropped it, and marks it so a reset clears it. It is the Factory from your Exercise 3, a
> class that hands back a finished object so nothing else learns how it is built, and the product is
> chosen by data, which is why there is one factory and not a creator class per drop.

## Template Method

### What the course means

The lecture note, `Course/Lesson 10 - Factory, Template, Performance & Optimization/Template Design
Pattern.md`: the skeleton of an algorithm in an abstract class, with subclasses overriding specific steps
without changing its structure. Three parts: the abstract class; the template method, which holds the
sequence and calls some steps the base implements and some abstract ones; and the concrete classes that
supply the abstract steps. Its example is `EnemyAI.ExecuteBehavior()`: `Patrol(); if (DetectPlayer())
Attack();`.

The lesson project has none. `Lesson 10.md` searched every script and concludes that Template Method is
lecture-only in that lesson, so the note's `EnemyAI` is the instructor's only worked example.

Exercise 3's text (`Course/Exercises/Exercise 03.md:26-29`): "צרו קובץ BaseProjectile עם פעולות כלליות
כמו Fire(). הקליע החדש יירש ממנה ויממש את הירי ישר למעלה בלבד". The graded answer was
`BaseProjectile.Fire(direction)`, with `GetLaunchImpulse`, `TryHandleTarget`, `OnTerrainHit` and `Expire`
as the steps a subclass supplies.

### Where it lives

Four bases, each with its fixed sequence in the base and its varying steps in the subclasses:

| Base | Template method | Steps a subclass writes | Subclasses |
|---|---|---|---|
| `Collectible` | `OnTriggerEnter2D` (`:51`): the player? then switch off, then `PickUp` | `PickUp` | 4 classes on 8 prefabs |
| `Hazard` | `Touch` (`:61`): once a frame, the player, the guard, then `Hurt`. `TryDestroy` (`:78`): `DestroyedBy`, then off and a puff | `Hurt`, `DestroyedBy` | `Fire`, `Rock` |
| `Enemy` | `Update` (`:149`): near or mid-action, then `Behave`. `TryDestroy` (`:205`): `DestroyedBy`, then `Die`. `Spawn` (`:306`): home, on, then `OnSpawned`. `Awake` (`:110`): register, then `OnAwake` | `Behave`, `DestroyedBy`; optionally `IsMidAction`, `OnSpawned`, `OnAwake` | 6 classes on 7 prefabs |
| `BaseProjectile` | `Launch` (`:55`): on, placed, velocity and clock cleared, then `OnLaunched`. `Update` (`:81`): `Fly`, then the timeout. `OnTriggerEnter2D` (`:97`): `OnHit` | `OnHit`; optionally `OnLaunched`, `Fly`, `OnAwake` | 4 |

- Open `Assets/Scripts/Enemies/Enemy.cs:149`, `Update`: the shape of the note's `ExecuteBehavior`, a
  check the base owns and then the step the subclass writes. The strongest of the four.
- Then `Assets/Scripts/Projectiles/BaseProjectile.cs:55`, `Launch`: Exercise 3's `Fire()`, public, called
  by a client, fixed steps and a hook.

### What the code shows

- All four match the note: a fixed sequence in the base, abstract steps in the subclasses.
- The fixed steps cannot be reordered or skipped. Every base keeps `Awake` private, and the two that
  need a per-frame step own `Update` and hand out `Behave` or `Fly`. Where a subclass has something of
  its own to set up, the base calls an `OnAwake` hook after its own work.
- Where each base stops is decided and recorded. `Egg` is not a `Collectible`, since it needs a visible
  beat between switching off and yielding its drop; `Spikes` is not a `Hazard`, since nothing may destroy
  it or absorb it; `Enemy` is not a `Hazard`, since it changes both of `Hazard`'s fixed steps.
- No abstract step has a shared default. `DestroyedBy` stays abstract though five of the six enemies give
  the same answer, so each file states its whole rule.
- What the subclasses share lives in the base rather than being repeated: an enemy's terrain
  measurements, its facing, the player's position, and its poses through `Show`.
- The limit: nothing in C# stops a subclass declaring its own `Awake` or `Update`, and Unity would call
  it and silently skip the base's. The guards are the base owning the message privately, a hook for
  what a subclass needs, and each base's header saying so.

Verdict: matches both definitions, the note's and Exercise 3's. The review made the four bases own their
Unity messages the same way, since `BaseProjectile.Awake` had been `protected virtual`, and moved the
enemies' shared pose code into `Enemy`; the plan's Decisions Log has both.

### What could be challenged

1. "Show me the template method." `Enemy.Update`: is he near, or am I part-way through something? Then
   behave. It is your `ExecuteBehavior`. For projectiles, `Launch` is Exercise 3's `Fire()`.
2. "Unity calls your `Update`, not a client." Unity is the client. The base owns the message privately
   so that no subclass can replace it.
3. "What stops a subclass writing its own `Awake`?" Nothing in the language. The base owns it privately,
   offers `OnAwake` for what a subclass needs, and says so in its header, and that is true of all four.
4. "Five enemies return the same `DestroyedBy`. Why no default?" A default would hide the one answer that
   differs, the ghost's, and would hand a new kind of destroyer to five enemies without anyone deciding it.
5. "Your static spider's `Behave` does nothing. Isn't that your `FriendlyEnemy`?" The base promises the
   contact damage, the destruction and the respawn, and all of them still happen. `Behave` only promises
   what it does while he is near, and a spider may stand still.
6. "Why isn't the egg a `Collectible`?" The base switches the object off before applying the effect, so a
   strike can undo it. The egg's whole behaviour is a beat between those two steps, and bending the base
   for it would cost the property that makes the base worth having.
7. "Why isn't `Enemy` a `Hazard`?" An enemy changes both of `Hazard`'s fixed steps - its destruction
   starts a respawn and its reset leaves a dead one dead - and a subclass that overrides its template's
   fixed steps is not following that template.

Rejected along the way, all in the Decisions Log: a Template for the full and partial reset, and for the
camera's follow and frame modes, each of which is one flag; an intermediate `DestroyingProjectile`
between the base and three of its subclasses; and a `Leave` helper on `Enemy` with no caller.

Nothing from these files goes on the worst-spot list. `Enemy`'s size is already on it.

### The defense sentence

> Every enemy runs a lifecycle its base owns: behave while the player is near, cost a strike on contact,
> die in a fixed order, drop, count down and come back. The six subclasses write only what they do while
> alive and what may destroy them, so a new enemy cannot forget to respawn. The projectiles are your
> Exercise 3 `Fire()`: the launch is fixed in the base, and the flight and the hit are left to each kind.

## MVC

### What the course means

The lecture slides, `Course/Lesson 07 - Extensions & MVC/MVC.pdf`:

- The model "manages the data and business logic of the application. It directly manages the data,
  logic, and rules".
- The view "represents the UI components and displays the data from the model to the user. It sends
  user actions to the controller".
- The controller "acts as an intermediary between the Model and View. It listens to the input from the
  View, processes it (including any necessary changes to the Model), and updates the View accordingly".
- Several triads in one project are encouraged: "Each MVC triad (Model-View-Controller) should be
  responsible for a specific part of your application".

Its example is an inventory: `InventoryModel` a plain class, `InventoryView` a MonoBehaviour, and
`InventoryController` a MonoBehaviour that creates the model with `new`, holds the concrete view through
`[SerializeField]`, and on a button changes the model and redraws. The slides' own SOLID page says that
concrete view "could be improved", and that the controller "should depend on interfaces rather than
concrete implementations".

The lesson project (`Lesson 07.md`): the coin counter, the same shape. `CoinsModel` plain, `CoinsView` a
MonoBehaviour, `CoinController` a MonoBehaviour subscribing to the coin event, adding a coin and
redrawing. Exercise 2 then asked for health "עם MVC", with a maximum of three, which is a model owning a
rule.

### Where it lives

| Triad | Model | View | Controller |
|---|---|---|---|
| כוח | `IPowerModel`, `PowerModel`: clamped between empty and the capacity | `IPowerView`, `PowerView`: one line per unit | `PowerController`: drains on `Tick`, takes `Gain` and `Spend` through `IPower`, costs a strike at zero, resets to the level's opening amount, redraws |
| פסילות | `ISessionState`, which is `State/SessionState.cs` | `IStrikesView`, `StrikesView` | `StrikesController`: redraws on `GameStarted` and `StrikeLost` |
| Fruit | the same `ISessionState` | `IFruitView`, `FruitView`: turns red close to a strike | `FruitController`: redraws on `GameStarted` and `FruitTaken`, with the count and the distance to the next strike |

- Open `Assets/Scripts/MVC/Power/PowerController.cs:62`, `Spend`: input arrives, the model changes, the
  view is told, and at zero the one consequence the controller owns, a strike.
- The session's changes, for the two counters: `State/GameFlow.cs:75` and `:94`.

### What the code shows

- The כוח triad is the slides' definition: input from time, fruit and a rock, a change to the model, an
  updated view.
- It goes past the slides' example in the direction the slides point. The controller depends on
  `IPowerModel` and `IPowerView`, as their DIP page asks; it is a plain C# class Zenject ticks rather than
  a MonoBehaviour, which is lesson 11's Clean Architecture line; and its model is injected rather than
  made with `new`.
- In the strikes and fruit triads the controller's role is split. `GameFlow` makes the model's changes,
  since losing a strike ends the game or resets the level and a twentieth fruit costs a strike, and those
  are game rules; each counter's controller does the last clause of the slide's sentence, updating the
  view when the model changes.
- No view decides anything about the game. `FruitView` owns only when its number turns red, a display
  choice; the controller hands it how far off the strike is.
- The model owns its own rules. `PowerModel` never passes the capacity and never goes below zero. That
  zero costs a strike reaches another system, so it lives in the controller.

Verdict: the כוח triad matches the definition and improves on the slides' own example as the slides ask.
The two counters are MVC over a shared model with the controller updating the view; defensible, and the
weaker half, which is better said than hidden.

### What could be challenged

1. "Your strikes controller never changes the model. What is it controlling?" The slide says the
   controller processes input "including any necessary changes to the Model". Here the change is a game
   rule - losing a strike ends the game or resets the level - so it lives in `GameFlow`, the class that
   decides what happens to the game. The counter's controller does the rest of the sentence and updates
   the view. The full triad is כוח.
2. "Why not a `StrikesModel` and a `FruitModel`?" The count would exist in two places, and the first
   thing that changed one and not the other would make them disagree. `SessionState` also says what
   neither would: these are the two numbers that survive a death and a level change.
3. "Why doesn't the fruit controller take the fruit?" Then the twentieth fruit runs from the fruit through
   the controller, an owed-strike event, the flow and the game over to the popup: five hops through three
   classes, where it is two methods in one file now.
4. "Your controller isn't a MonoBehaviour, like mine." It needs no Unity lifecycle beyond a tick, and
   Zenject supplies the tick, which keeps the rule out of a MonoBehaviour.
5. "Where is the strikes model?" `State/SessionState.cs`, read by the counters through `ISessionState`,
   which has no way to change it.
6. "`PowerController` implements four interfaces. Too much?" Each is one face of the same job: `ITickable`
   the drain, `IPower` what the rest of the game does to power, `IResettable` the level start,
   `IInitializable` registering for it. All of it is power, in 92 lines.

Rejected along the way, all in the Decisions Log: a shared base for the two counter views, which would be
three types doing the work of two; moving the session's writes into the two counter controllers; and a
game state every input-reading component would have to consult.

Nothing from these files goes on the worst-spot list.

### The defense sentence

> כוח is a full triad: time, fruit and rocks are the input, the controller changes the model, the model
> keeps it between empty and full, and the view only draws, with the controller a plain class Zenject
> ticks and every part behind an interface, which is what your slides' own DIP page asks for. The two
> counters share one model, the session, because losing a strike and taking a twentieth fruit are game
> rules the flow decides, so their controllers do the other half of the job: putting the model on screen
> whenever it changes.

## Async & Tasks

Not started.

## SOLID

Not started.

### Worst-spot candidates

Collected by every section above and ranked at the end. Found by reading the code before the review
began, so none of these is verified as a problem yet:

- `Enemy`: 335 lines and seven injected dependencies, having gained the shared pose code the Template
  step moved out of four subclasses.
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
