# Coverage

Every numbered requirement in `Exercise Adventure Island.md`, where the recordings show it, and what
proves it. The features script is written from the `F` rows and the code script from the `C` rows, and
each gameplay take's log copy is read against the evidence afterwards. Written in Stage 21, step 2.

## How to read this

Shown in: `F` is a features cut, spoken; `P` is the playthrough, silent; `C` is the code recording;
`none` gives the reason. The playthrough says nothing, so a requirement counts under `P` only if it reads
as working with no word said. Anything that needs a word or a setup is `F`, even where the playthrough
will show it as well.

Evidence: a log line in backticks, copied from the code, and only a line the saved `LogSettings` prints,
which has `Enemy` and `Projectile` at `Verbose` and the other six categories at `Info`. "No `X`" means
the proof is a line that must not appear. "Screen" is what the camera shows, "Inspector" a serialized
field, and "code" the file and method to open. Placed objects log under their prefab's name, so a fire
is `Sprite_Fire` and the ghost `Sprite_Enemy_Ghost`.

Not printed under the saved settings, so a row that would need one says "screen": `Power N of 16`, jumps
and landings, `Mount attacked`, the mount's refused second press, `Rock contact ignored`, and a drop
finding no ground below it.

Seen: filled in after a take, as the log copy's file name and line number, or "screen".

## 1. Controls

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 1.1 | Arrows move him left and right | F, P | screen; code: `PlayerMovement.FixedUpdate` | |
| 1.2 | Jump on Space | F, P | screen; code: `PlayerJump.Update` | |
| 1.3 | Attack on Z | F, P | `Projectile_Axe launched`; code: `PlayerAttack.Update` | |
| 1.4 | Jump height follows how long Space is held | F | screen: a tap beside a held jump; code: `PlayerJump.CutRise` | |
| 1.5 | Jumping while riding jumps the animal | P | screen | |
| 1.6 | `[out]` Crouch | none, out | | |

## 2. The game

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 2.1 | Two levels, level 1 leading straight into level 2 | P | `Level complete`, then `Level started: Level_2` | |
| 2.2 | A level ends at its cave door | P | `Level complete` | |
| 2.3 | Finishing level 2 shows a congratulation popup whose button restarts | P | `Game complete - every level finished`; after the button, `Game started - 3 strikes` and `Level started: Level_1` | |
| 2.4 | Running out of פסילות shows a Game Over popup whose button restarts | F | `Game over - no strikes left`; after the button, `Game started - 3 strikes` and `Level started: Level_1` | |
| 2.5 | No `SceneManager.LoadScene` anywhere | F, C | `Zenject container built`, `Drops registered: 6` and the four `Pooled` lines appear once, above the first `Game started`, and never after a restart; code: no `LoadScene` in `Assets/Scripts`, and `GameFlow.StartGame` | |
| 2.6 | `[out]` Start menu and pause button | none, out | | |

## 3. פסילות

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 3.1 | Three פסילות at the start | P | `Game started - 3 strikes`; screen: the counter | |
| 3.2 | A פסילה from fire, an enemy, a pit, empty כוח and the twentieth fruit | F | `Fire touched - a strike is owed`, `<enemy> touched - a strike is owed`, `Snake fireball hit the player - a strike is owed`, `Spikes touched - a strike is owed`, `Power ran out - a strike is owed`, `Fruit reached 20 - a strike is owed`, each followed by `Strike lost - N remaining` | |
| 3.3 | A פסילה returns him to the current level's start, not level 1's | F | after a strike in level 2, no `Level started: Level_1`; screen | |
| 3.4 | The level is not rebuilt: a killed enemy stays dead | F | screen; no `back after` line for that enemy until its countdown ends; code: `Enemy.ResetTo` | |
| 3.5 | Fruit, eggs and placed items all come back | F | the same egg's `Sprite_Egg opened` line a second time after the strike; screen | |
| 3.6 | A פסילה costs the weapon and the animal and resets כוח; the fruit count survives | F | `Weapon lost` and `Mount lost` after `Strike lost`; screen: the bar back at the level's amount, the fruit count unchanged | |
| 3.7 | Running out of פסילות ends the game | F | `Game over - no strikes left` | |
| 3.8 | `[out]` Rolling rock and falling coconut | none, out | | |

## 4. כוח and fruit

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 4.1 | A bar of 16, each level opening at its own amount, 11 and 9 | F | Inspector: Power Capacity on `GameInstaller`, Starting Power on `Level_1` and `Level_2`; screen: the bar at each level's start | |
| 4.2 | Drains 1 unit every 3 seconds | F, C | Inspector: Drain Seconds on `GameInstaller`; screen; code: `PowerController.Tick` | |
| 4.3 | `Fruit_1` gives 1 unit, `Fruit_2` gives 2 | F | `Fruit taken - 1 power`, `Fruit taken - 2 power` | |
| 4.4 | Fruit taken at a full bar is wasted | F | `Fruit taken - power already full` | |
| 4.5 | כוח at zero costs a פסילה | F | `Power ran out - a strike is owed`, then `Strike lost - N remaining` | |
| 4.6 | כוח does not carry into level 2 | F | screen: the bar at level 2's start | |
| 4.7 | Every twentieth fruit costs a פסילה, and the count is not cleared | F, C | `Fruit reached 20 - a strike is owed`, then `Strike lost - N remaining`; code: `GameFlow.TakeFruit` | |
| 4.8 | The fruit count survives a פסילה and a level change | F | screen: the counter after `Strike lost` and after `Level started: Level_2`; code: `SessionState` | |

## 5. Hazards

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 5.1 | An אבן costs 3 כוח | F | `Rock touched - costs 3 power` | |
| 5.2 | An אבן shoves him forward, immune for the length of the shove, and can shove him into fire | F | screen; one `Rock touched` for a shove across two rocks; `Rock touched - costs 3 power` followed by `Fire touched - a strike is owed` | |
| 5.3 | A boomerang destroys an אבן and an axe does not | F | `Sprite_Rock destroyed - Boomerang`; for the axe, no `Sprite_Rock destroyed`, and the axe flies on to `Axe hit the ground` | |
| 5.4 | Every animal's attack destroys an אבן | F | `Sprite_Rock destroyed - MountAttack`, riding each of the three | |
| 5.5 | Riding into an אבן destroys it and the animal, and costs no כוח | F | `Sprite_Rock destroyed - Riding`, `Hit absorbed - the mount and what hit it are both gone`, `Mount lost`; no `Rock touched` | |
| 5.6 | A מדורה costs a פסילה | F | `Fire touched - a strike is owed`, `Strike lost - N remaining` | |
| 5.7 | Nothing but the פייה destroys a מדורה | F | `Sprite_Fire destroyed - Fairy`; no `Sprite_Fire destroyed` after an axe, a boomerang or an animal's attack | |
| 5.8 | Riding into a מדורה destroys it and the animal, and costs no פסילה | F | `Sprite_Fire destroyed - Riding`, `Hit absorbed - the mount and what hit it are both gone`, `Mount lost`; no `Strike lost` | |
| 5.9 | A תהום costs a פסילה, and the פייה does not save him from it | F | `Spikes touched - a strike is owed`, `Strike lost - N remaining` | |

## 6. Weapons

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 6.1 | One weapon at a time; a second replaces the first | F | `Weapon taken: Projectile_Axe`, later `Weapon taken: Projectile_Boomerang`, and only `Projectile_Boomerang launched` after it | |
| 6.2 | Attacking needs a weapon or an animal | F | `Attack ignored - no weapon held` | |
| 6.3 | The weapon and the animal carry into level 2; a פסילה takes the weapon | F | no `Weapon lost` or `Mount lost` between `Level complete` and `Level started: Level_2`, then a `launched` line in level 2 with no new `Weapon taken`; `Weapon lost` after `Strike lost` | |
| 6.4 | A weapon is kept while riding and usable once the animal is gone | F | `Weapon taken` between `Mounted` and `Mount lost`; no weapon `launched` while mounted; a `launched` line after `Mount lost` with no new `Weapon taken` | |
| 6.5 | The גרזן and the פטיש are one object | none, a note on the text | | |
| 6.6 | The axe flies in an arc | F, P | screen | |
| 6.7 | At most 3 axes and 1 boomerang in flight, and no separate cooldown | F | three `Projectile_Axe launched`, then `Projectile_Axe throw ignored - every copy is already in flight`; `Projectile_Boomerang throw ignored - every copy is already in flight`; Inspector: Projectile Counts on `GameInstaller` | |
| 6.8 | Thrown axes are pooled | C | `Pooled 3 of Projectile_Axe` at the top; code: `ProjectilePool.Get` | |
| 6.9 | An axe on the ground returns to the pool; one on an enemy kills it and returns | F, C | `Axe hit the ground`; `<enemy> destroyed - Axe`; code: `ProjectileAxe.OnHit` | |
| 6.10 | An axe cannot destroy an אבן | F | as 5.3 | |
| 6.11 | The boomerang flies out to a set distance and returns to where he is now | F | `Boomerang turned`, then `Boomerang caught`; screen: he moves while it is out; code: `ProjectileBoomerang.Fly` | |
| 6.12 | The boomerang destroys everything in its path | F | more than one `destroyed - Boomerang` line inside one throw, before its `Boomerang caught` | |
| 6.13 | Caught, the boomerang disappears and goes back to its pool | F, C | `Boomerang caught`; `Pooled 1 of Projectile_Boomerang` at the top; code: `BaseProjectile.Despawn` | |
| 6.14 | No ammunition count | P | screen: nothing counts throws | |

## 7. חיות

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 7.1 | Three animals: blue, red and green | F | `Mounted: Mount_Blue`, `Mounted: Mount_Red`, `Mounted: Mount_Green` | |
| 7.2 | לב mounts blue, עלה red, כוכב green | F | `Dropped: Heart` then `Mounted: Mount_Blue`; `Dropped: Leaf` then `Mounted: Mount_Red`; `Dropped: Star` then `Mounted: Mount_Green` | |
| 7.3 | Tokens come out of eggs and drop from destroyed enemies | F | `Sprite_Egg opened - Heart`, then `Dropped: Heart`; `<enemy> destroyed - <by>`, then `Dropped: Star` | |
| 7.4 | Blue hits low with its tail; red spits fire, higher | F | screen; `Projectile_MountFire launched` for red | |
| 7.5 | Green spins in place and hits what it touches | F | screen; `<enemy> destroyed - MountAttack` while riding green | |
| 7.6 | Red's fire reaches a few tiles, not across the screen | F, C | screen; code: the mount fire's range in `ProjectileDirector` | |
| 7.7 | An animal's attack kills every enemy but the רוח רפאים | F | `<enemy> destroyed - MountAttack`; no `Sprite_Enemy_Ghost destroyed - MountAttack` | |
| 7.8 | An animal's attack destroys an אבן but not a מדורה | F | `Sprite_Rock destroyed - MountAttack`; no `Sprite_Fire destroyed - MountAttack` | |
| 7.9 | A token taken while riding swaps the animal | F | a second `Mounted:` line with no `Mount lost` before it | |
| 7.10 | An animal absorbs one hit, and it and whatever hit it both go | F | `<enemy> destroyed - Riding`, `Hit absorbed - the mount and what hit it are both gone`, `Mount lost`; no `Strike lost` | |
| 7.11 | The animal carries into level 2, with the weapon and the fruit count | F | no `Mount lost` between `Level complete` and `Level started: Level_2`; screen | |
| 7.12 | `[out]` The other two animals and the skateboard | none, out | | |

## 8. Enemies

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 8.1 | Six types: עכביש, ציפור, a jumping נחש, a shooting נחש, צפרדע, רוח רפאים | F, P | screen: level 1 alone holds all six; a `destroyed` line naming each | |
| 8.2 | An enemy's shot harms only the player and his animal, never another enemy | F, C | screen: a fireball flying through an enemy; no `destroyed` line for that enemy; code: `ProjectileSnakeFireball.OnHit` | |
| 8.3 | A drop is configured per enemy, "nothing" included, never rolled | F | Inspector: Drop on an enemy; `Dropped: <type>` after its `destroyed` line | |
| 8.4 | A destroyed enemy comes back where it was placed, after a random 10 to 20 seconds | F | `<enemy> back after N.Ns`, with N between 10 and 20; screen: it reappears where it was placed; Inspector: Respawn Delay on `GameInstaller` | |
| 8.5 | The game should not be difficult | P | screen | |
| 8.6 | An עכביש moves only up and down, or hangs still | F | screen | |
| 8.7 | Which of the two is set per instance | F | Inspector: Moves on each spider | |
| 8.8 | A ציפור flies left at a constant speed, dipping and rising | F | `Sprite_Enemy_Bird swooped - he was N away`; screen | |
| 8.9 | Each bird's speed, dip and swoop length are its own | F | Inspector: Speed, Dip and Swoop Distance on two birds | |
| 8.10 | A bird kept high signals a drop worth reaching | F | Inspector: Dip and Drop on the bird at level 1's x141; screen | |
| 8.11 | A bird waits until he is in range and to its left, swoops once, flies home, and waits again | F | a second `swooped` line from the same bird; screen | |
| 8.12 | The jumping נחש stands, hops forward, and stands again | F, P | screen | |
| 8.13 | The submission shows it jumping | F | screen | |
| 8.14 | The shooting נחש fires at him once he is in view | F | `Projectile_SnakeFireball launched` | |
| 8.15 | Its firing stops under some condition | F, C | screen: it stops once he walks out of range; Inspector: Activation Range on the snake; code: `Enemy.Update` | |
| 8.16 | Its fireball does not harm other enemies | F, C | as 8.2 | |
| 8.17 | The צפרדע leaps higher and further than the נחש, at him, once he is close | F | `Sprite_Enemy_Frog landed at x N - leapt from N, aimed at N`; screen | |
| 8.18 | Its jump timing is random | C | code: `Frog.WaitAgain` | |
| 8.19 | The רוח רפאים is the instructor's addition, modelled on Boo | none, a note on the sources | | |
| 8.20 | The רוח רפאים stays still while he faces it and chases when his back is turned | F | `Sprite_Enemy_Ghost is coming - he is N away`, `Sprite_Enemy_Ghost froze - he is N away` | |
| 8.21 | Only the פייה destroys it | F | `Sprite_Enemy_Ghost destroyed - Fairy` and no other `Sprite_Enemy_Ghost destroyed`; a mount it takes reads `Hit absorbed - the mount is gone and what hit it is still standing` | |
| 8.22 | It gives up chasing past a set distance | F, C | screen; Inspector: Activation Range on the ghost; code: `Enemy.IsPlayerNear` | |
| 8.23 | One is enough | P | screen: one in level 1 | |
| 8.24 | Its sprite is not in the instructor's sheets | none, a note on the sources | | |
| 8.25 | Level 2 has a second one, killed only by level 2's פייה | P | screen | |
| 8.26 | One drop per enemy, not a list | F | Inspector: the single Drop field | |

## 9. פייה

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 9.1 | A פייה lasts 10 seconds | F | `Fairy taken - 10s`, then `Fairy gone`; screen: the marker; Inspector: Seconds on `PlayerFairy` | |
| 9.2 | It makes him invincible and destroys what he touches, מדורה and רוח רפאים included | F | `<enemy> destroyed - Fairy`, `Sprite_Fire destroyed - Fairy`, `Sprite_Enemy_Ghost destroyed - Fairy`, `Snake fireball put out - Fairy`; no `Strike lost` while it lasts | |
| 9.3 | A תהום still kills him | F | `Spikes touched - a strike is owed`, `Strike lost - N remaining`, then `Fairy lost` | |
| 9.4 | A level has a place to take a פייה and then fall into a תהום | F | the authored spot: level 1's egg at x118 and the pit from x122; the lines of 9.3 | |
| 9.5 | The text splits this rule across two bullets | none, a note on the text | | |

## 10. ביצים and drops

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 10.1 | Eggs are placed in the level by hand | P | screen | |
| 10.2 | Touching an egg opens it | F | `Sprite_Egg opened - <type>` | |
| 10.3 | An egg holds a token, a weapon or a פייה, set per egg | F | `Sprite_Egg opened - Heart`, `- Axe`, `- Fairy`; Inspector: Drop on an egg | |
| 10.4 | Every drop type appears somewhere across the two levels | F, P | `Dropped:` for each of `Heart`, `Leaf`, `Star`, `Axe`, `Boomerang` and `Fairy` | |
| 10.5 | A drop falls out of what held it and settles on the ground below | F | screen: a high bird's drop falling | |
| 10.6 | A drop comes to rest where he can reach it | F | screen | |
| 10.7 | An egg shows its cracked frame, then what it held takes its place | F | screen; `Sprite_Egg opened` then `Dropped:` | |

## 11. The levels

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 11.1 | Level 1 runs left to right | P | screen | |
| 11.2 | Level 2 climbs from the bottom, then goes right at the top into the exit | P | screen; `Level complete`, `Game complete - every level finished` | |
| 11.3 | A fall in level 2 takes him back down toward its start | P, F | screen | |
| 11.4 | He can climb back down, the camera never leaves the level, and only a תהום or spikes kill on a fall | P, F | screen; code: `LevelCamera.ClampAxis` | |
| 11.5 | The reference videos | none, a note on the sources | | |
| 11.6 | One level gives the axe and the other the boomerang; one animal early, a different one late | P | `Weapon taken: Projectile_Axe` in level 1 and `Weapon taken: Projectile_Boomerang` in level 2; `Mounted: Mount_Blue` early in level 1 and `Mounted: Mount_Green` late | |
| 11.7 | `[out]` A third level | none, out | | |

## 12. UI

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 12.1 | The פסילות count on screen | P | screen | |
| 12.2 | The fruit count on screen | P | screen | |
| 12.3 | The כוח bar on screen | P | screen | |

## 13. Techniques

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 13.1 | DI, Pooling, Builder, Factory, MVC, Async & Tasks, Template | C | `Techniques.md`: Dependency Injection; Builder and Object Pooling; Factory; Template Method; MVC; Async & Tasks | |
| 13.2 | SOLID throughout, graded on the worst spot | C | `Techniques.md`: SOLID | |
| 13.3 | Why a Task and not a coroutine, and the reverse | C | `Techniques.md`: Async & Tasks | |
| 13.4 | `[good]` Reflection | C | `Techniques.md`: Reflection | |
| 13.5 | `[good]` Tiled, or another level pipeline | C | code: `Tools > Level`, with `LevelFile` and `LevelScene` | |
| 13.6 | `[nice]` Animation | P | screen | |
| 13.7 | Where each technique lives, the axes' pool being his | C | `Techniques.md`: each section's "Where it lives" | |

## 14. Submission

Shown in is "submission" for what has to be done rather than recorded.

| # | Requirement | Shown in | Evidence | Seen |
|---|---|---|---|---|
| 14.1 | Through Moodle, by 26.9.2026 | submission | step 9 | |
| 14.2 | A gameplay recording and a code recording | submission | the three files of 14.7 | |
| 14.3 | He watches the gameplay first and writes down its problems | none, how he grades | | |
| 14.4 | Several short recordings are allowed | submission | the features cuts | |
| 14.5 | A live download or YouTube link if Moodle refuses the size | submission | step 9 | |
| 14.6 | Defense on 28, 29 or 30 September, booked by email | submission | an email once the files are in | |
| 14.7 | `1-Gameplay-Playthrough`, `2-Gameplay-Features`, `3-Code` | submission | the three files, at their lengths | |
| 14.8 | A text file of what the features recording shows and when | submission | written from the edited video, step 4 | |
