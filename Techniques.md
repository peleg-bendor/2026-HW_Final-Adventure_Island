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

- Open `Assets/Scripts/Installers/GameInstaller.cs:41`, `InstallBindings`: 31 bindings.
- 28 MonoBehaviours receive their dependencies through `[Inject] Construct`, and the container builds 16
  plain C# classes through their constructors, one of them, `RespawnCountdown`, once for every enemy.
- Pooled projectiles and drops are instantiated through `IInstantiator`, in `ProjectileBuilder.Build`
  and `DropFactory.Create`, so an object made at runtime is injected like one placed in the scene.
- If asked who receives: `State/GameFlow.cs:23` for a constructor, `Enemies/Enemy.cs:32` for a
  `Construct` with seven dependencies.

### What the code shows

- One idiom per kind of class: a MonoBehaviour always takes a `Construct` method, a plain class always a
  constructor. No field injection, which the lesson used once.
- Everything the lesson used, and more of Zenject besides: `WithArguments` for the rule numbers,
  `BindInterfacesTo`, `ITickable` and `IInitializable`, `FromComponentsInHierarchy` for the level roots,
  `AsTransient` so every enemy gets a respawn countdown of its own, and `IInstantiator` for objects made
  at runtime.
- The injected fields are private, carry no `[SerializeField]`, and none of their names appears in
  `Scene_Game.unity`. The installer's own serialized numbers and prefabs do, which is configuration
  entering at the composition root.
- The order is guaranteed, not lucky. `SceneContext` runs at execution order -9999, so every scene object
  that receives anything is injected before its `Awake`. `LogSettings` and `LogFileWriter` wake earlier,
  at -10000, so the log file is already open while the container builds, and neither has anything
  injected. `SceneKernel` runs `Initialize` from its `Start` at -9997, before
  `GameStarter.Start`. `InstantiatePrefab` instantiates inactive, injects, then activates
  (`DiContainer.cs:1703`, `2044`, `2052`). A base class's `Construct` runs before its subclass's
  (`CallInjectMethodsTopDown`, `DiContainer.cs:1471`).
- The concrete injections each have a reason. `SessionState` goes into `GameFlow`, the one class that
  changes it, while the displays read `ISessionState`. The `Player` marker answers only where he is and
  which way he faces. `ProjectilePrefabs`, `ProjectileCounts` and `RespawnDelay` hold data. Every
  service a class receives is behind an interface apart from those.
- The player's components reach each other the same way. The ground check, his walking, the mount's
  attack and the mount's hit are injected behind `IPlayerGround`, `IPlayerMotion`, `IMountAttack` and
  `IMountStrike`, and none of his parts fetches another with `GetComponent`.
- Still outside the container: the static `GameLog`, `Time` and `Keyboard.current` read where they are
  used; `GetComponent` for Unity's own components on the same object, such as a `Rigidbody2D`; and
  `GetComponent<Player>()` on whatever collider touched a hazard, a pickup, a door or a projectile, which
  arrives through a physics callback and cannot be injected.

Verdict: matches the course's definition and the lesson's usage. The review changed four things, all
recorded in the plan: `IInstantiator` replaced `DiContainer` in the builder and the drop factory, `Levels`
stopped searching the scene, the camera stopped holding the player's Transform, and three unused
self-bindings went. The SOLID step later injected the player's components into each other in place of
ten `GetComponent` calls and a `GetComponentInChildren`.

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
8. "Twenty-eight bindings in one installer. Is that a god class?" Its one job is deciding what goes where.
   Zenject allows several installers, and splitting this one would add files without changing a single
   dependency.
9. "Your plain classes still read `Time`. Where is the testability?" They are MonoBehaviour-free, not
   Unity-free. There are no tests, and an injected clock with one implementation would be an abstraction
   before a second use case.
10. "Why not field injection, like `FireballWeapon`?" A `Construct` method lists everything a
    MonoBehaviour depends on in one signature, which is as close as a MonoBehaviour gets to a
    constructor.
11. "The jump and the ground check are on the same GameObject. Why inject instead of `GetComponent`?"
    Your DI note names `GetComponent` among what injection replaces, and lesson 4's walkthrough flags
    `GetComponentInChildren<FireballWeapon>()` for reaching a concrete class. The mount's animator and
    attack live in `Mounts/` and read parts of the player; each now sees one narrow interface and cannot
    call anything else on the component. `GetComponent` stays for Unity's own components, like the
    `Rigidbody2D`, which are not services.

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
- `BaseProjectile.cs:55`, `Launch`: what makes a reused projectile safe.
- The three shooters: `PlayerAttack.cs:58`, `SnakeShooter.cs:81`, `PlayerMountAttack.cs:78`.

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
- The callers: `Enemies/Enemy.cs:252` and `Collectibles/Egg.cs:78`, each holding only an `IDropFactory`
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
   was no switch for it to remove and one class serves several drop types. The Reflection section has
   the full reasoning.

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
| `Collectible` | `OnTriggerEnter2D` (`:50`): the player? then switch off, then `PickUp` | `PickUp` | 4 classes on 8 prefabs |
| `Hazard` | `Touch` (`:61`): once a frame, the player, the guard, then `Hurt`. `TryDestroy` (`:78`): `DestroyedBy`, then off and a puff | `Hurt`, `DestroyedBy` | `Fire`, `Rock` |
| `Enemy` | `Update` (`:146`): near or mid-action, then `Behave`. `TryDestroy` (`:202`): `DestroyedBy`, then `Die`. `Spawn` (`:276`): home, on, then `OnSpawned`. `Awake` (`:106`): register, then `OnAwake` | `Behave`, `DestroyedBy`; optionally `IsMidAction`, `OnSpawned`, `OnAwake` | 6 classes on 7 prefabs |
| `BaseProjectile` | `Launch` (`:55`): on, placed, velocity and clock cleared, then `OnLaunched`. `Update` (`:81`): `Fly`, then the timeout. `OnTriggerEnter2D` (`:99`): still in flight? then `OnHit` | `OnHit`; optionally `OnLaunched`, `Fly`, `OnAwake` | 4 |

- Open `Assets/Scripts/Enemies/Enemy.cs:146`, `Update`: the shape of the note's `ExecuteBehavior`, a
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
enemies' shared pose code into `Enemy`. The log pass then gave `BaseProjectile.OnTriggerEnter2D` its
check, once an axe landing on two tiles showed that a despawned projectile still receives the rest of
its physics step. The plan's Decisions Log has all three.

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
- The session's changes, for the two counters: `State/GameFlow.cs:72` and `:97`.

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

### What the course means

The lecture slides, `Course/Lesson 05 - Async/Async & Tasks.pdf`:

- Coroutines are "ideal for tasks that need to be spread out over several frames, such as animations,
  timed events, AI behaviors", and, on a line of its own, "No Cancelation option within the function".
- Tasks "represent asynchronous operations that can run concurrently and independently of the main
  program flow", and "Shines with requests".
- The examples: `async void Start()` with the `await` inside `try`/`catch`; a `CancellationTokenSource`
  with `catch (TaskCanceledException)`; and a coroutine cancelling itself through a bool and `yield
  break`, because it has no token.
- "When to use Coroutines": Unity game-loop work - animations, UI updates, timed events - waiting with
  `WaitForSeconds` and the other yield instructions, and simpler logic without threading.
- "When to use Tasks": non-Unity work, CPU-bound and parallel work, file and network I/O, and "Error
  Handling and Task Composition: Task provides more robust error handling capabilities through try-catch
  blocks".

The lesson project (`Lesson 5.md`) pairs them on purpose: `EnemySpawner` loops on `await Task.Delay(ms,
token)` inside an `async void` with `try`/`catch`, cancelled by a key, and `PlayerInvincible`, beside it,
runs a timed invincibility on a coroutine with `WaitForSeconds`.

The transcript (00:57:58): "אם אתה משתמש בטאסקס, אני רוצה להבין למה השתמשת בטאסקס ולא בקורנטינה
וגם להפך".

### Where it lives

| | Where | What waits | Why this tool |
|---|---|---|---|
| Task | `Enemies/RespawnCountdown.cs:19`, `Begin`, started by `Enemy.WaitAndReturn` | a killed enemy's countdown | the enemy is switched off before the wait begins, and Unity stops coroutines on a deactivated GameObject |
| Task | `State/GameFlow.cs:131`, `EndGame`, with `UI/Popup.cs:15`, `ShowAsync` | the player clicking a popup's button | `GameFlow` is a plain C# class, so there is nothing to run a coroutine on |
| Coroutine | `Player/PlayerFairy.cs:63`, `Hold` | the פייה's ten seconds | the player is never switched off, and the wait should freeze under a popup |
| Coroutine | `Collectibles/Egg.cs:72`, `Hatch` | the crack before the drop | the egg stays alive for the whole wait |
| Coroutine | `Animation/OneShotAnimator.cs:16`, `Start` | a puff's frames | the object lives exactly as long as its frames |

- Open `Assets/Scripts/Enemies/RespawnCountdown.cs:19`: lesson 5's `EnemySpawner`, in this game, in a
  class whose only job is the countdown. Beside it, `PlayerFairy.cs:63` is its `PlayerInvincible`.

### What the code shows

- Both Tasks follow the slides' own examples: `async void`, started from code that cannot `await`, the
  whole wait inside `try`/`catch`. The respawn catches cancellation apart from faults, so an abandoned
  countdown returns quietly and a real failure is logged as an error; the ending logs a fault, unfreezes
  time and clears its guard, so an error cannot leave the game locked.
- Each enemy has a `RespawnCountdown` of its own, bound `AsTransient`, with a new token source at every
  death. The enemy cancels it on a full reset, since a level start brings every enemy back itself; in
  `OnDestroy`, so nothing touches a destroyed object when Play stops; and not on a strike, since a killed
  enemy stays dead and its countdown carries on. After the delay, the countdown returns unless it is
  still the current one, which covers a cancel or a new death landing between the delay ending and the
  rest of the method running.
- The popup bridges a click to an `await` with a `TaskCompletionSource` its button completes.
- The three coroutines wait with `WaitForSeconds`, the slides' example, and each stops with
  `StopAllCoroutines` when its owner resets.
- Two differences between the tools decide every choice. Lifetime: a coroutine lives and dies with an
  active GameObject, and a Task belongs to no object. Clock: `WaitForSeconds` follows `Time.timeScale` and
  freezes under a popup, and `Task.Delay` counts real time.

Verdict: the code matches the slides line for line. The two Tasks sit on the side of the slides' "when
to use" lists the slides would not pick, and the answer for each is structural rather than preference.
The review closed a one-frame gap in the respawn's cancellation; the plan's Decisions Log has it.

### What could be challenged

1. "Your slides say timed events are for coroutines. Why is the respawn a Task?" A coroutine runs on an
   active GameObject, and the enemy is switched off before its countdown begins. Switching it off is
   what makes it gone in one line - its collider, its picture, its animation and its `Update` at once -
   and a coroutine would need a manager holding a timer for every enemy, or an enemy kept on with each
   part disabled by hand. The Task also brings what your slides credit Tasks with: a token that cancels
   it when a level starts, and `try`/`catch` around the wait.
2. "And the popup? Your slides put UI under coroutines." The code that waits is `GameFlow`, a plain C#
   class, where `StartCoroutine` does not exist. The wait is for a click, not for time, and the Task keeps
   end, wait and start again in one method beside the rules that decided the game had ended.
3. "Then why is the fairy a coroutine?" It is your `PlayerInvincible`: a timed event on an object that is
   always on, returning nothing. Its ten seconds should freeze under a popup, which `WaitForSeconds` does;
   a Task would count through the frozen screen and need a token to stop.
4. "`async void` is bad practice." Usually. Nothing that starts these waits can `await` - a trigger
   callback cannot, nor can a Unity message - and an unawaited `Task` swallows its exception where `async
   void` rethrows it. Both methods catch explicitly, which is your slides' `async void Start` example.
5. "Your respawn ignores `Time.timeScale`." It does. The only freeze in this game is under a popup, and
   both popups end in a full reset that cancels every countdown, so a respawn under a frozen screen only
   reaches the state the restart was about to produce.
6. "What if a countdown ends in the same frame as a restart?" The rest of the method checks that its
   countdown is still the enemy's current one, and stops if a reset or a new death replaced it.
7. "Unity 6 has `Awaitable`. Why `Task`?" The course and the requirement name Tasks, and `Task` is what
   lesson 5 taught.

Rejected along the way, all in the Decisions Log: building the level from its file at Play, which would
have given Async a file I/O home; a MonoBehaviour listening for game over, which a coroutine would have
done just as well; and one token shared by every enemy, replaced by one per enemy.

Nothing from these files goes on the worst-spot list.

### The defense sentence

> A coroutine lives on an active GameObject and runs on Unity's clock, and a Task belongs to no object and
> runs on real time. A killed enemy is switched off before its countdown starts, and the flow that waits
> for the popup is not a MonoBehaviour at all, so both of those are Tasks, with a token that cancels a
> respawn when a level starts. The fairy, the egg and the puff are timed events on objects that stay alive
> and should freeze under a popup, which is your own slide's case for a coroutine and exactly what
> `WaitForSeconds` does.

## SOLID

### What the course means

The lecture note, `Course/Lesson 02 - Git Jira SRP/Solid Principles.md`, gives each principle one example:

- Single responsibility: a class "should only have one reason to change". `Invoice` holds its data,
  calculates, prints itself and saves itself; printing moves to `InvoicePrinter` and saving to
  `InvoicePersistence`.
- Open/closed: "add new functionality without touching the existing code for the class". Adding
  `saveToDatabase` to `InvoicePersistence` is the violation, and an `IInvoicePersistence` with one class
  per kind of storage is the fix.
- Liskov: a subclass replaces its base "without altering the desirable properties of the program".
  `FriendlyEnemy.AttackPlayer` waves at the player.
- Interface segregation: "many client-specific interfaces are better than one general-purpose
  interface. Clients should not be forced to implement a function they do no need". `FullTimeWorker`
  has to implement `UploadInvoice`, and throws.
- Dependency inversion: "classes should depend upon interfaces or abstract classes instead of concrete
  classes". `NotificationService` makes its own `EmailService`; the fix receives an `IMessageService`
  through its constructor.

The lesson projects. Lesson 3 is open/closed: `WeaponsHandler` holds a `List<IWeapon>`, and its
walkthrough calls `LaserWeapon` the proof, a third weapon added with no change to the handler, where a
switch on the weapon type would have needed one. Lesson 4 is the other three: `IWeapon` split into
`IUseableWeapon` and `IReloadWeapon` by what each kind of weapon needs, and `WeaponsHandler` again as the
DIP example. Its walkthrough names the spots that fall short: `FireFlowerPowerUp` calling
`GetComponentInChildren<FireballWeapon>()` rather than the interface, and concrete types in `TempInit`,
accepted there because a composition root has to name them. Lesson 2's walkthrough of the starting
project faults `SC_Player` for mixing input, movement, visuals and death in one class.

The transcript (00:52:01 to 00:52:37): he runs a program he built over the submitted code, it tells him
where something is not SOLID, "ואז אני הולך למקום היחידי שלא עשיתם אותו סוליד ועל זה אני שופט
אתכם", even when the other 95 percent is.

### Where it lives

No one file holds it, so there is one place to open for each principle:

| Principle | Open | What it shows |
|---|---|---|
| Single responsibility | the `Player` GameObject in `Scene_Game` | twelve scripts with one job each: moving, jumping, the ground check, the reset, the attack key, the guard, the fairy, the mount's body and attack, the two animators, and the marker |
| Open/closed | `State/ResetRegistry.cs:22`, `ResetAll` | walks every `IResettable` and never learns which kinds exist |
| Liskov | `Mounts/MountStrike.cs:57` | calls `TryDestroy` on whatever it touched; an enemy, a rock and a fire each answer by their own rule, and none refuses the contract |
| Interface segregation | `State/ResetRegistry.cs:5` | one class behind `IResetRegistry` and `IResetRunner`, so an enemy can register itself and cannot reset the level |
| Dependency inversion | `Installers/GameInstaller.cs:41` | 29 interfaces, and the one place that picks what answers each |

### What the code shows

Measured over the 110 game scripts and 10 editor scripts:

- No `switch` anywhere, and no check of an object's type in game code. What varies by kind is a subclass,
  an interface, or data authored per object: the `Destroyer` flags each target answers, the `DropType`
  on each enemy and egg, the recipe for each projectile prefab.
- No singleton and no search of the scene in game code.
- 29 interfaces, 23 of them with one or two members. The largest is `IGameFlow`, with three events and
  four operations.
- Four abstract bases with 16 subclasses between them, and not one subclass declares a Unity message of
  its own.
- Two static classes in game code: `GameLog`, called 146 times from 61 files, and `Ground`, the physics
  query four classes share. One static field: `Spikes`' last touched frame, shared by a pit's tiles so
  that one fall is logged once, which is logging rule 9 and Exercise 3's way of meeting it.
- The largest class in game code is `Enemy` at 308 lines, 148 of them code, then `Frog` at 219 and
  `SnakeJumper` at 165. In the editor tooling it is `TilePlacerWindow`, at 235.

Principle by principle:

- Single responsibility. The first prediction in the plan's SOLID risk register was the player as the god
  object, and the policy was to split him from the first stage rather than a 400-line class later. He is
  twelve scripts, the largest `PlayerJump` at 129 lines. `GameFlow` gave up the reset registry and the
  level list at stage 11, and this review split `ProjectileDirector` and `LevelWindow` and took the
  respawn countdown out of `Enemy`.
- Open/closed. A new enemy is a subclass of `Enemy` and a prefab. A new pickup of an existing kind is a
  prefab listed on the installer, a new mount a `MountDefinition` asset, and anything a level start
  restores implements `IResettable`. The edits that remain are named in the questions below: a new drop
  type adds a `DropType` value, a new destroyer a `Destroyer` flag, and a fifth projectile touches four
  files.
- Liskov. Each subclass keeps its base's promises, which is the Template section's subject: none overrides
  a fixed step, and every `IDestructible` answers every destroyer with a `bool`. The three places a
  subclass would not have fitted were kept out of the hierarchy rather than bent into it: the egg is not
  a `Collectible`, spikes are not a `Hazard`, and `Enemy` is not a `Hazard`.
- Interface segregation. The splits follow who may do what: `IResetRegistry` and `IResetRunner` on one
  class, so a collectible registers itself and cannot reset the level; `ISessionState` beside
  `SessionState`, so a counter reads the numbers and cannot spend a strike; `ILevels` apart from
  `IGameFlow`, so the camera asks where it is and cannot end the game; `IPlayerShove` and `IPlayerMotion`
  on one component, so a rock pushes him and an animator reads him. No implementer anywhere is forced to
  write a member it has no use for, which is the note's own test.
- Dependency inversion. The DI section in full. Every service a class receives is behind an interface,
  apart from `SessionState` inside `GameFlow`, the `Player` marker and the data holders, and lesson 4's
  `GetComponentInChildren<FireballWeapon>()` shape is gone from the player.

Verdict: the five hold where the course's own examples test them. The review resolved the five worst
spots it found, and what is left is answered below rather than changed. The likeliest single spot for a
checker to name is `Enemy` by size, then `IGameFlow` by member count.

### What could be challenged

Ranked, the likeliest first.

1. "`Enemy` is 308 lines with seven dependencies. That's your `Invoice`." It has one reason to change,
   the lifecycle every enemy shares, and each dependency serves one step of it: `IGameFlow` the strike on
   contact, `IPlayerGuard` the absorb, `IResetRegistry` the level start, `IRespawnCountdown` the
   countdown, `Player` the activation range and the ghost's rule, `IDropFactory` the drop, `IDeathEffects`
   the picture of it going. 148 of the lines are code; the rest are comments, braces and blank lines. The
   countdown's Task was a second job, and the review moved it out. Splitting further, into contact and
   death components, would spread the fixed order of a death over components whose order nothing
   guarantees, and that order is what a Template base exists to own.
2. "`IGameFlow` has seven members. That's your `IEmployee`." No consumer uses more than two, and none uses
   both an operation and an event: a fire calls `LoseStrike`, the door `CompleteLevel`, the weapon slot
   listens to `GameStarted` and `StrikeLost`. Splitting it into `IGameFlow` and `IGameEvents` was weighed
   at stage 11 and refused, because the two halves have one implementation and one lifetime and always
   travel together, which is bookkeeping rather than segregation. By the note's own test nothing is
   forced to implement a member it doesn't need: `GameFlow` is the only implementer and uses all of them.
   It had ten until this review, which removed three events nothing listened to.
3. "`GameLog` is static and used from 61 files. Where's your DIP?" Logging is cross-cutting: every class
   has something to report and no class depends on what the log does with it, the same reason Unity's
   `Debug.Log` is static. An injected logger would add a parameter to 61 classes for one implementation,
   and it has to work before the container does, since the installer logs while it builds. What it
   guarantees instead: `Info` and `Verbose` are `[Conditional]` on `UNITY_EDITOR` and `DEVELOPMENT_BUILD`,
   so a release build does not compile those calls or their arguments, while `Warning` and `Error` ship,
   because a shipped game should still report faults. The levels are set per category on `LogSettings`
   in the scene.
4. "And `Ground`, also static?" Four callers: `Frog`, `SnakeJumper`, `Spider` and `DropSettle`. It holds
   no state and no configuration, and answers three questions about Unity's physics world with the one
   rule of what counts as ground; `Physics2D`, which it wraps, is static itself. It left `Enemy` when a
   falling drop became the second thing that needed the rule. An `IGround` would be an interface with one
   implementation and nothing to substitute, which this project refused for input and for the clock.
5. "Add a fifth projectile." A new `BaseProjectile` subclass and its prefab, and four files edited: a field
   on `ProjectilePrefabs`, a count on `ProjectileCounts`, a recipe in `ProjectileDirector` and a fill line
   in `ProjectilePool`. The shooters don't change, since `PlayerAttack` throws whatever the slot holds.
   The requirements fix the four there are (6.7, 7.4, 8.14), and a third weapon is on the list of things
   deliberately not built. The plan records where that changes: at around eight or ten recipes, one
   asset per projectile beats a file that grows.
6. "You still call `GetComponent<Player>()`." In eleven places, and each asks what a physics callback just
   handed it: a collider arrives as the argument, so there is no dependency to inject. The marker is for
   exactly this, chosen over a tag, which is a string that fails silently.
7. "An editor API inside a runtime component?" `SpriteVariant` records its choice as a prefab override
   inside `#if UNITY_EDITOR`, Unity's standard idiom, and none of it reaches a build. It stays there
   because the component's own Pick One menu needs it; moving it into `LevelScene` would leave a re-roll
   from that menu unsaved.
8. "Twelve scripts on one player. Isn't that fragmentation?" Each has one reason to change, and the other
   shape is lesson 2's `SC_Player`. What splitting costs is the parts reaching each other, which they do
   through `IPlayerGround`, `IPlayerMotion`, `IMountAttack` and `IMountStrike`, injected.
9. "Most of your interfaces have one implementation. Isn't that abstraction before a second use?" Your
   note's DIP asks for the dependency on an abstraction and says nothing about a second implementation,
   and lesson 12 binds `IFireballBuilder` to its only builder. The interface is the consumer's view of a
   class: `IResetRegistry` cannot run a reset, `ISessionState` cannot spend a strike, and a
   MonoBehaviour's interface hides its transform and its enabled flag from whoever holds it. What is
   bound concrete is data, the `Player` marker, and `SessionState` inside `GameFlow`, the one class that
   changes it. The rule against abstraction before a second use is what refused an `IInputService` and
   an `IGround`, where there was nothing to narrow and nothing to substitute.
10. "`Destroyer` and `DropType` are enums. Enums mean switches." Neither has one. Each target states its
    `Destroyer` flags in `DestroyedBy` and its base tests them with a bitmask, and `DropFactory` looks
    `DropType` up in a dictionary filled from the prefabs.

Resolved during the review, so no longer candidates:

- `ProjectileDirector`, which held the recipes, filled and grew the pool, and was the `Throw` three
  shooters injected concretely. Split in the Builder and Pooling step.
- `LevelWindow` (314 lines), which held the window, the file format, the scene reconciliation and the
  JSON writing, and `TilePlacerWindow` (306), which repeated part of it. Split into `LevelFile` and
  `LevelScene`, which both windows use; now 139 and 235 lines.
- `Enemy`'s respawn Task, its token and its guard, about 45 lines. Moved into `RespawnCountdown`.
- The player's components reaching each other's concrete classes: ten `GetComponent` calls across
  `PlayerJump`, `PlayerAnimator`, `PlayerMountAnimator`, `PlayerAttack` and `PlayerReset`, and
  `PlayerMountAttack`'s `GetComponentInChildren<MountStrike>`. `PlayerReset`'s two fetched `PlayerJump`
  and `PlayerMovement` only to clear state they own, and went when those two became resettables of their
  own. The rest are injected: behind `IPlayerGround`, `IPlayerMotion`, `IMountAttack` and `IMountStrike`,
  and the `Player` marker into `PlayerAttack`.
- `IGameFlow`'s `GameOver`, `LevelComplete` and `GameComplete`, raised and heard by nothing. Removed, which
  took the interface from ten members to seven.

### The defense sentence

> Everything that varies by kind in this game is a subclass, an interface or data authored on the object,
> so there is no switch over types anywhere, and every class receives what it needs from the installer
> instead of finding it. The two largest spots a checker can name are `Enemy` and `IGameFlow`, and each is
> one job: the lifecycle every enemy shares, and the game's operations with the events they raise.

## Reflection

Not on the required list, and the instructor said he would be glad to see it. It is answered here because
the defense is likely to ask where it went.

### What the course means

The lecture note, `Course/Lesson 08 - Reflections & DLL/Reflections.md`: a program inspecting and
manipulating itself at runtime, finding types, fields and methods and creating instances without knowing
the type at compile time. Its three examples set a public field through `GetField`, reach a private one
with `BindingFlags.NonPublic`, and create a component from a type name with `Type.GetType` and
`Activator.CreateInstance`.

The lesson project, `Lesson 08.md`: `EnemyCreator.CreateEnemy("Goomba", position)` turns a string into a
component with `Type.GetType` and `AddComponent(type)`, then calls `GetMethod("Initilize").Invoke` beside a
commented-out `enemy.Initilize()` that would have done the same. The walkthrough draws the line itself:
reflection is "the escape hatch for when you don't have a compile-time type to work with", the
`GetMethod` half is there only for the demo, and reflection is "strictly worse than a direct call in every
situation where a direct call is actually possible", trading away compile-time checking and speed.

The transcript (00:54:18): "משום מה לא שמתי לכם פה רפלקשן... הייתי שמח גם כן לראות רפלקשן".

### Where it lives

- In the project's own code, nowhere, by decision.
- In Zenject: `Assets/Plugins/Zenject/Source/Util/ZenReflectionTypeAnalyzer.cs:76` finds every
  `[Inject]` method through `GetCustomAttributes`, and the container calls it through its `MethodInfo`.
  `Enemy.Construct` is private, and this is how it gets called at all.

### What the code shows

- It was placed in the drop factory from the first plan: a `[Drop(DropType.Heart)]` attribute on each
  collectible class, scanned once, to remove the one switch over drop types the design expected.
- At stage 16 neither half held. There was no switch to remove, since every drop is a prefab that declares
  its own type and the factory is a dictionary. And the attribute could not work: `WeaponCollectible`
  serves two drop types and `MountCollectible` three, so an attribute on a class cannot tell the blue
  mount's token from the red's. Making it buildable meant five empty subclasses, and nothing would stop
  one of them sitting on the wrong prefab and working.
- Two other homes were measured and lost. An attribute audit of the drop prefabs would catch exactly one
  mistake, a prefab whose drop type disagrees with its class. A scan for unassigned `[SerializeField]`
  references would cover about 30 fields, where every class that owns one already warns when it is
  missing and says what stops working.
- Nothing in this game has a type unknown until runtime. A drop is a `DropType` the compiler checks, a
  projectile a prefab reference, an enemy a prefab the level tool placed.

Verdict: absent from the project's code on purpose, for the reason lesson 8's own walkthrough gives, and
present in the container every class depends on.

### What could be challenged

1. "I said I'd be glad to see reflection. Where is it?" It was planned for the drop factory, to replace a
   switch over drop types, and when the factory was built there was no switch, and the attribute could not
   tell two mounts served by one class apart.
2. "So there's none?" Every `Construct` in this project is found by Zenject through reflection, from its
   `[Inject]` attribute, and called through its `MethodInfo`; a private `Construct` could not be called any
   other way.
3. "Why not add it somewhere anyway?" Your walkthrough of `EnemyCreator` says the `GetMethod` call is
   worse than the direct call beside it. Reflection added where the type is already known would be that
   half of the demo.

### The defense sentence

> Reflection was planned for the drop factory, to replace a switch over drop types, and when the factory
> was built there was no switch to replace and the attribute could not tell two mounts of one class apart.
> Nothing in this game has a type that isn't known until runtime, which is the one case your lesson says
> reflection is for, and every `Construct` here is still found and called through reflection by Zenject.

## Patterns considered and rejected

The one complete list, including those already named in a technique's section. Each has its full reasoning
in the plan's Decisions Log.

Template and class hierarchies:

- A base class and two subclasses for the full and partial reset: one flag of real difference. It became
  `ResetTo(ResetScope)`, an enum rather than a bool so the call site says which.
- The same for the camera's follow and whole-level modes: one bool for a testing aid.
- `Enemy` deriving from `Hazard`: an enemy changes both of `Hazard`'s fixed steps.
- `Spikes` as a `Hazard` answering `Destroyer.None`: nothing may destroy or absorb the pit, and saying so
  needs a flag for one subclass.
- The egg as a `Collectible`: its whole behaviour is the beat between the base's two fixed steps.
- A `protected virtual Awake` calling `base.Awake()`: a subclass that forgets the call loses its
  registration. Rejected for `Enemy`, and removed from `BaseProjectile` in this review.
- A projectile subclass owning `Update`: it would hide the base's timeout.
- The axe alone ignoring contacts after it is gone: the red mount's flame could kill twice the same way,
  so the base checks before `OnHit`.
- A default `OnHit`, or a default `DestroyedBy`: a new destroyer would be granted to five enemies by
  silence.
- An intermediate `DestroyingProjectile`: the two fires fly alike and are opposites in what they hit.
- Four projectile classes behind an `IPooledProjectile` with no base: the reset written four times.
- One snake class with an `AttackType` enum, and a switch over the three mounts: the switch over kinds
  the risk register predicted. Two snake subclasses, and two feature flags on `MountDefinition`, instead.
- A Visitor for `IDestructible`, or a `DestroyedBy` property beside a `Destroy()` method: five classes and a
  method per target, or the bitwise test repeated in every attacker.
- A dying state inside `Enemy`: a throwaway effect object instead.
- A `Leave` helper on `Enemy`, and an empty `Drop` stubbed early: code with no caller.
- A `CountView` base for the two counters: three types doing the work of two.

Builder, Pooling and Factory:

- A Builder for the mounts: eleven fields of data, so a `MountDefinition` asset per mount.
- The recipes as installer fields, as a ScriptableObject per projectile, or on the prefabs: each leaves
  the builder copying numbers that already exist somewhere.
- A pool class per projectile: Exercise 3's pool four times.
- Instantiating the enemies' shots, and a singleton pool: Exercise 3's way. Counting the shooters at
  startup to size their pool was rejected too.
- A fixed size for the snake's fireball pool: the number was a cap pretending to be a rule.
- `ProjectileDirector` as recipes, pool filler and `Throw` in one class: split into the course's roles.
- A `WeaponType` enum in the weapon slot: mapping it back to a prefab is a switch, so the slot holds the
  prefab.
- The builder and the factory taking the whole `DiContainer`: a service locator, so `IInstantiator`.
- Factory Method's creator subclass per drop: the product is chosen by data, so it would still need the
  dictionary.
- A prefab field and a plain `Instantiate` on each enemy and egg: uninjected, parented to what dropped it,
  and handed back by a reset instead of destroyed.
- A list of drops per enemy, and random drops or random egg contents.
- Pooling the mount's hit, making it a projectile of speed zero, or an invisible `OverlapBox`: one object
  switched on and off, with the trigger every other damage source uses.
- Death effects through the container or a pool: nothing to inject, and a few a minute.

MVC:

- A `StrikesModel` and a `FruitModel`, or `SessionState` split in two: the count in two places.
- The session's writes inside the counter controllers: the twentieth fruit would run five hops through
  three classes.
- The strikes controller as an `IResettable`, or polling the session in `Update`: a `GameStarted` event.
- A controller folded into its view: the flow and the session inside a MonoBehaviour.

Async & Tasks:

- Building the level from its file at Play, which would have given Async a file-reading home: the less
  steady of the two, and a rebuild on death would bring the dead back.
- A MonoBehaviour listening for game over: a coroutine would do it identically.
- "The await returns which button was pressed" as the popup's reason: both popups have one button.
- An unawaited `async Task`: it swallows its exception.
- One token for every enemy, or a manager holding every timer: a countdown per enemy.
- The respawn following `Time.timeScale`: every freeze in this game ends in a full reset.

DI and interfaces:

- VContainer: the fallback had Zenject not compiled on Unity 6.
- `IInputService` and an injected clock: one implementation each, and no tests.
- An interface for `LevelDefinition`, and one for the `Player` marker.
- Splitting `IGameFlow` into operations and events.
- An interface per player component and nothing else changed: three of those couplings were state
  other components own.
- Splitting `IMountAttack` by consumer.
- `FindObjectsByType` for the levels, a tag for the player, and a serialized `Transform` for the start
  marker.
- Rule numbers serialized on a prefab, and a `GameSettings` ScriptableObject: a number describing a rule
  enters at the installer, and one describing an object sits on the object.

State, flow and the reset:

- `SceneManager.LoadScene` on death or game over: a reload hides static state that never gets reset.
- `LevelState`: two copies of every fact.
- A `Playing`, `GameOver`, `Won` state machine: every input reader would have to check it, and it would not
  stop physics. `Time.timeScale` instead, with a private `ending` flag nothing outside `GameFlow` reads.
- `GameFlow` moving the player: he resets himself.
- An ordered reset registry: the camera flags a snap for its `LateUpdate` instead.
- An operation anything could call to end the game: a game ends only by running out of strikes or
  finishing the last level, so the ending is private to `GameFlow`.
- `PlayerReset` clearing the jump's input and the movement's shove: each resets itself.
- A separate `PlayerShove` component: two writers of one velocity.
- An immunity kept on each rock: three rocks in a row cost nine power for one contact.
- The fairy as a field on `PlayerGuard`: its own component, like the mount slot the guard consults.
- `Physics2D.SyncTransforms()` in `PlayerReset`: a global physics call for one reader.
- `Enemy.ResetTo` skipping enemies whose level is switched off: their countdowns still need cancelling,
  and a strike's reset reaches them too. The spider measures its drop only inside its own level.
- Every spike tile in a pit logging its own touch, with the flow's refusal after the second: one fall is
  one line, so the first tile touched in a frame speaks for the pit. The strike guard stays in `GameFlow`.
- Dropping the installer's log line because it never reached the file: the writer opens ahead of
  `SceneContext`, so a binding that fails is in the file too.
- The Input System's action asset: no rebinding, no second device, no options screen.

Tooling:

- Tiled: the placer previews better, and the level files are still in Tiled's format.
- A teardown build: a diff, so what was set by hand on a placed object survives.
- `LevelWindow` as one class.
- `Builder/`, `Factory/` and `Pooling/` folders: folders are domains, so "show me the Factory" names a file.
- `SpriteVariant`'s editor call moved into `LevelScene`: the Pick One menu would stop saving.
