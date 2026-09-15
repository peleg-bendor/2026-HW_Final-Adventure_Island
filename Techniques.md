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

Not started.

## Builder and Object Pooling

Not started.

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

- `ProjectileDirector` holds the recipes, fills and grows the pool, and is the `Throw` every shot goes
  through. `PlayerAttack`, `SnakeShooter` and `PlayerMountAttack` inject it as a concrete class, and it
  is the only service a MonoBehaviour injects without an interface apart from the `Player` marker.
- `Enemy`: 323 lines and seven injected dependencies.
- `LevelWindow` (314 lines) and `TilePlacerWindow` (306). Editor tooling ships with the submission.
- `IGameFlow`: ten members, and no client uses both the operations and the events. Splitting it was
  considered and rejected in the Decisions Log.

### Reflection

Not started.

### Patterns considered and rejected

Not started.
