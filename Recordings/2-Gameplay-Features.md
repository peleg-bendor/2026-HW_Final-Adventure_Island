# Features Recording Script — `2-Gameplay-Features`

Recording script for the second of the final project's three recordings: the game's features one at a
time, in eleven cuts on levels staged for them. Capped at 10 minutes. Which requirement each cut shows is
in `Coverage.md`; the numbers are never said, and the cut titles are what `0-Contents.txt` lists.

## How to read this

- Lines in `>` blocks are said out loud. Everything else is a stage direction and is never spoken.
- One sentence per line inside a `>` block. Each line is one breath.
- `### **SHOW**` is a major beat inside a cut. Plain `**SHOW**` is a small move inside that beat.
- The time in a cut's heading is its spoken words at its rate, so a take that runs much longer than it is
  either waiting for something or over-explaining.
- Every cut is its own Play session and its own take, edited together afterwards. Cut 8 is five of them.
- "Rows" under a cut title are `Coverage.md`'s numbers, for us. What each cut needs in the level is under
  Staging.
- Spoken in English. The greeting is the only Hebrew, and the game's terms are said in English.

### About the timings

Written to word counts at the rates from Exercises 1 to 3: about 90 spoken words a minute where the camera
is on the game, and 160 where it is on the Editor. A wait the speech doesn't fill is marked in its cut and
cut in editing.

| Cut | Content               | Words | Rate | Time |
| --- | --------------------- | ----- | ---- | ---- |
| 1   | Intro and controls    | 60    | 90   | 0:40 |
| 2   | Power                 | 54    | 90   | 0:36 |
| 3   | Strikes and Game Over | 76    | 90   | 0:51 |
| 4   | The fruit count       | 56    | 90   | 0:37 |
| 5   | Weapons               | 83    | 90   | 0:55 |
| 6   | Rocks                 | 52    | 90   | 0:35 |
| 7   | Mounts                | 96    | 90   | 1:04 |
| 8a  | Spider                | 38    | 90   | 0:25 |
| 8b  | Bird                  | 53    | 90   | 0:35 |
| 8c  | Snakes                | 63    | 90   | 0:42 |
| 8d  | Frog                  | 25    | 90   | 0:17 |
| 8e  | Ghost                 | 48    | 90   | 0:32 |
| 9   | Fairy                 | 41    | 90   | 0:27 |
| 10  | Level 2's start       | 33    | 90   | 0:22 |
| 11  | Inspector             | 91    | 160  | 0:34 |

**869 words, about 9:12**, measured from this file by script. The remaining forty seconds are for what the
words don't cover: the seven-second stand in cut 2, the walk in cut 8e, the two level changes in cut 4, and
the two Game Over popups. If the edited recording still comes out over 10 minutes, cut in this order:

1. **Cut 8e's walk away from the ghost** (−12 seconds, and the walk). It still hangs still, chases, and
   survives the axe and the mount; what goes is seeing it give up.
1. **Cut 4's third strike** (−14 seconds). The count still crosses into level 2 and costs a strike at 40;
   what goes is the same fruit taken again to 60.
1. **Cut 2's stand** (−7 seconds of waiting, and its first line). The bar still drains through the whole
   cut, and cut 11 states the rate as a number.

What not to cut, at any length: cut 3's "the level wasn't rebuilt" beat, cut 6's axe flying through the
rock, cut 9's fairy at the pit I built into level 1, and cut 8b's per-bird fields, and cut 8c's fireball
through the other snake. Each is a place the
instructor said he would look.

## Before you record

- **Nothing is saved.** The staging is painted into the scene and the scene is never saved, so Play runs on
  it and the project keeps the authored levels. Don't press Ctrl+S, and don't save the level file from
  `Tools > Level`. If a save happens by accident, say so, and restore the file from the last commit.
- **One station at a time.** For each cut: reopen the scene without saving, which clears the last cut's
  station and brings the authored level back, paint this cut's station, move the start marker to it, and
  film. Nothing accumulates, so no two stations ever have to coexist.
- **Cut 11 is recorded on the authored scene**, which is any moment just after the scene has been reopened.
- The Game view at 16:9, the Console off screen, and a 20-second test recording first. `LogSettings` as
  saved.
- **Cut 8's five sub-cuts show the Inspector**, so those are recorded with the Editor on screen: the
  Hierarchy, the Inspector docked at the right, the Console closed, and the Game view still 16:9. Every
  other cut is the Game view alone.
- Stop Play between cuts. There are no log copies for this recording: if a cut looks wrong, say so before
  pressing Play again, so `GameLog.txt` still holds it.

## Staging

You lay each station out by hand, right before its cut, and the next reopen of the scene clears it. What
each cut needs is below: the objects, the order he meets them, and the spacing that matters.

### Rules

- **Move the start marker to the station.** Every cut starts at the marker, and every strike and restart
  returns there, so a station can sit anywhere in level 1 without a walk to it.
- **Nothing authored within about 14 cells of the station**, or erase what is: an enemy wakes when the
  player is within 12 units, so one standing nearby moves through the cut. Erasing is safe, since the scene
  is never saved.
- **Drops are staged as eggs.** The Tile Placer places no pickups, so a staged axe or star is an egg with
  its **Drop** field set in the Inspector. An egg left at its default holds a heart.
- **Keep level 1's stretch from x96 to x125 as it is.** Cut 9 uses its ghost, fire, fairy egg and pit.
- **Level 2 needs twenty fruit near its start** for cut 4, and nothing else.

### What each station needs

| Cut | In order from the marker, left to right                                                              |
| --- | ---------------------------------------------------------------------------------------------------- |
| 2   | Over the marker, at a height walking under does not take: two common fruit and three super. Then on flat floor: three rocks, each at least 6 cells from the next, with no fire within 6 cells after the last. |
| 3   | An egg with the axe; an egg with a heart; a fruit on the floor; a jumping snake; a pit, three spike cells wide; a fire; a static spider at head height. Kept short, so the walk back reaches the snake inside its 10 seconds. |
| 4   | Twenty fruit in a row on flat floor, then a door. Twenty more fruit near level 2's start.            |
| 5   | An egg with the axe; a jumping snake; open floor; an egg with the boomerang; two static spiders in a line at head height; a step up one tile, for moving while the boomerang is out. |
| 6   | An egg with the axe; rock A; an egg with the boomerang; rock B; an egg with a heart; rock C; rock D; rock E with a fire 3 or 4 cells after it. Every rock but E at least 6 cells from whatever follows it, since a shove carries him about 4. |
| 7   | An egg with the axe; an egg with a heart; a jumping snake; an egg with a leaf; a moving spider with 3 cells of floor in front of it; an egg with a star; a frog; a fire; open floor. |
| 8a  | A moving spider and a static one, 14 cells apart.                                                    |
| 8b  | A bird perched high with its Drop set to a star, over a block two tiles high that he can stand on, as under the authored bird at x141, since an axe rises less than a unit above where it is thrown and cannot reach the bird's dip from the floor; an egg with the axe before it. |
| 8c  | An egg with the axe; a jumping snake penned between two tiles 3 cells apart, so its hops stay in the fireballs' line; a shooting snake beyond it. |
| 8d  | A frog on flat floor with room either side.                                                          |
| 8e  | An egg with the axe; a ghost; an egg with a heart beside it; at least 20 cells of open floor to the right. |
| 9   | Before x96: an egg with a fairy and a shooting snake. The rest is the authored stretch to the pit at x122. |
| 10  | An egg with the axe; an egg with a heart; three fruit on the floor; a door.                          |

---

## Cut 1 — Intro and controls `0:40`

Rows: 1.1 to 1.4. Staging: none; any station's marker.

### **SHOW** — Press Play. Stand a moment, then walk right a few steps.

**SAY:**

> שלום, זה פלג בן דור, וזאת ההגשה שלי לפרויקט הסופי.
>
> This is the features recording.
> I've rearranged the levels for it, so each feature is a few steps away, and the playthrough recording plays them as I built them.

**SHOW** — Tap Space once, then hold it for a full jump.

> The arrows move, Space jumps, and Z attacks.
> A tap on Space is a short jump, and holding it jumps higher.

---

## Cut 2 — Power `0:36`

Rows: 4.1 to 4.5.

### **SHOW** — Press Play. Stand still on the marker for about seven seconds, watching the bar drop twice.

**SAY:**

> This is the power bar, which holds 16.
> This level starts at 11, and loses one every three seconds.

### **SHOW** — Walk right into the three rocks, one after another. The third empties the bar and costs a strike.

> A rock costs three power and pushes me forward.
> An empty bar costs a strike.

### **SHOW** — Back on the marker at 11. Jump through the fruit over the marker in one pass, left to right. The last one finds the bar full.

> A common fruit gives one, a super fruit two.
> The bar stops at 16, so the last one gives nothing.

---

## Cut 3 — Strikes and Game Over `0:51`

Rows: 2.4, 2.5, 3.1 to 3.7, 5.6, 5.9.

### **SHOW** — Press Play. Open the axe egg and the heart egg, and ride. Take the fruit. Kill the jumping snake with the tail, last, just before the pit. Fall into the pit.

**SAY:**

> Three strikes at the start.
> I have an axe and a mount, a fruit taken, and a snake killed.
> A pit costs a strike.

### **SHOW** — Back on the marker: no axe, no mount, the bar at 11. Walk back over the station: both eggs and the fruit are there, and the snake is not.

> I'm back at this level's start, without the axe or the mount.
> The level wasn't rebuilt: the eggs and the fruit are back, and the snake I killed stays dead.

### **SHOW** — Walk into the fire. Back on the marker, walk into the static spider. The Game Over popup.

> A fire and an enemy cost a strike too.

### **SHOW** — Click the popup's button. Level 1 again, with three strikes.

> The popup button starts the game again, and nothing here loads a scene.

---

## Cut 4 — The fruit count `0:37`

Rows: 4.7, 4.8, 3.5.

### **SHOW** — Press Play. Walk right through the row of fruit, watching the counter turn red at 15. The twentieth costs a strike.

**SAY:**

> The counter turns red five fruit before a strike.
> The twentieth costs one.

### **SHOW** — Back on the marker, the counter still at 20. Eat some more fruit, then walk into the door. Level 2: reach forty fruit. The fortieth costs a strike.

> The count is never cleared, so it comes with me into level 2.
> Twenty more make 40, and another strike.

### **SHOW** — Back at level 2's start, with the twenty fruit there again. Eat them again. The sixtieth costs the last strike, and the Game Over popup appears.

> A strike puts the fruit back, so the same twenty take me to 60.
> That was my third strike, so the game ends.

---

## Cut 5 — Weapons `0:55`

Rows: 6.1, 6.2, 6.6, 6.7, 6.9, 6.11 to 6.13, 10.2, 10.7.

### **SHOW** — Press Play. Press Z before taking anything.

**SAY:**

> Z does nothing without a weapon or a mount.

### **SHOW** — Walk into the axe egg: its cracked frame, then the axe. Throw one at open floor. Then press Z four times fast: three axes fly and the fourth press throws nothing. Throw one at the jumping snake. Throw one at a rock.

> An egg cracks, and then reveals its drop.
> The axe flies in an arc.
> Three can be in the air, so a fourth press throws nothing: that's the pooling.
> One that lands is gone; one that hits an enemy kills it. It cannot destroy a rock.

### **SHOW** — Open the boomerang egg. Throw it at the two spiders. While it's out, press Z again, then step up onto the tile and back. It comes back to him wherever he stands.

> The boomerang replaces the axe, since I carry one weapon.
> It destroys rocks and enemies in its path, except the ghost, and goes through walls.
> It comes back to wherever I am now.

---

## Cut 6 — Rocks `0:35`

Rows: 5.1 to 5.5, 6.10.

### **SHOW** — Press Play. Open the axe egg and throw two axes at rock A. They fly through it. Then walk into rock A: the bar drops by three and he is pushed forward.

**SAY:**

> An axe flies straight through a rock.
> Touching one costs three power and pushes me forward.

### **SHOW** — Open the boomerang egg and throw it at rock B, which breaks. Open the heart egg, ride, and attack rock C, which breaks. Ride into rock D: the rock and the mount are gone, and the bar has not moved.

> The boomerang breaks it, and so does a mount's attack.
> Riding into one costs the mount and no power, and the rock goes too.

### **SHOW** — Walk into rock E. The push carries him into the fire. A strike.

> The push can carry me into a fire, which costs a strike.

---

## Cut 7 — Mounts `1:04`

Rows: 5.8, 6.4, 7.1 to 7.10.

### **SHOW** — Press Play. Open the axe egg, then the heart egg. Press Z beside the jumping snake: the tail kills it.

**SAY:**

> Three mounts, each from its own token.
> The heart gives the blue one, which hits with its tail.
> While I ride, Z is the mount's attack.

### **SHOW** — Open the leaf egg while riding. From 3 cells away, press Z at the moving spider. The flame travels a few tiles and kills it.

> Another token swaps it, and the leaf gives the red one, who shoots fireballs.

### **SHOW** — Open the star egg. Press Z beside the frog: the spin kills it.

> The star gives the green one, which spins and hits whatever it touches.

### **SHOW** — Press Z at the fire: it stays. Ride into it: the fire and the mount are gone, and no strike is lost. Press Z: an axe flies. Open the heart egg, ride into a rock.

> It kills every enemy except the ghost, and does not destroy a fire.
> Riding into one, the mount takes the hit and the fire goes with it.
> With the mount gone, the axe is in use again. Riding with a mount into a rock has the same effect. 

---

## Cut 8 — Enemies `2:31`

Six sub-cuts, one per enemy, each its own take. The Editor is on screen for all six: the enemy is selected
in the Hierarchy and its fields are in the Inspector while it behaves in the Game view.

### Cut 8a — Spider `0:25`

Rows: 8.1, 8.6, 8.7.

### **SHOW** — The moving spider selected: Activation Range, and Moves ticked. Press Play and walk toward it; it drops and rises. Then the static one, with Moves unticked.

**SAY:**

> Every enemy has a range, and does nothing at all until I'm inside it.
> This spider's is twelve units.
> A spider only moves up and down, and this box is what decides whether it moves or is static.

### Cut 8b — Bird `0:35`

Rows: 7.3, 8.3, 8.8, 8.9, 8.10, 8.11, 10.5, 10.6.

### **SHOW** — The bird selected: Speed, Dip, Swoop Distance, and Drop set to a star. Press Play, take the axe, and walk in from its left. It swoops, flies back, and waits. Step out and in for a second swoop.

**SAY:**

> A bird waits on its perch until I'm close and on its left.
> Then it swoops once, dipping and rising, flies home, and waits again.
> Its speed, dip and swoop distance are set on this bird.

### **SHOW** — Stand on the block and throw axes at it as it dips. It falls, and the star drops to the floor and settles. Take it.

> Every enemy can carry a drop, set the same way, with nothing as one of the choices.

### Cut 8c — Snakes `0:42`

Rows: 8.2, 8.4, 8.12 to 8.16. The shooting snake fills the other one's respawn, so nothing here waits.

### **SHOW** — The jumping snake selected: Hop Distance and Stand Seconds. Press Play and walk in from the left: it stands, hops, and turns at its wall. Come closer, and the shooting snake beyond it turns to face him and fires; the fireballs pass through the jumping snake.

**SAY:**

> The jumping snake stands, hops forward, and stands again, turning at a wall.
> The shooting snake turns to face me, and fires while I'm in its range.
> Its fireballs fly through the other snake, because an enemy's shot never harms an enemy.

### **SHOW** — Take the axe and kill the jumping snake. Dodge the fireballs, then walk back out of the shooter's range: the firing stops. Step in and out once more while the countdown runs.

> True to all enemies, once killed it comes back at its origin point, after ten to twenty seconds, rolled each time.

**SHOW** — The jumping snake is standing in its pen again.

### Cut 8d — Frog `0:17`

Rows: 8.17, 8.18.

### **SHOW** — The frog selected: Min and Max Wait Seconds, Jump Height, Min and Max Jump Distance. Press Play and stand near it. It faces him and leaps two or three times; jump clear.

**SAY:**

> The frog turns to face me and leaps at me, further than the snake hops.
> The wait between leaps is rolled between these two numbers, Min and Max Wait Seconds.

### Cut 8e — Ghost `0:32`

Rows: 8.20, 8.21, 8.22.

### **SHOW** — The ghost selected: Activation Range and Chase Speed. Press Play, take the axe, and face it: it hangs still. Turn away: it comes. Turn back: it stops. Throw two axes through it. Take the heart and ride into it: the mount is gone and the ghost stays. Walk off at once.

**SAY:**

> The ghost watches which way I face.
> It hangs still while I look at it, and comes after me when my back is turned.
> A weapon goes through it, and a mount can't kill it either.

### **SHOW** — Turn your back and walk right. It follows, then stops.

> Outside its range it gives up. Only a fairy can destroy it.

---

## Cut 9 — Fairy `0:27`

Rows: 5.7, 9.1 to 9.4.

### **SHOW** — Press Play. Open the fairy egg. The fairy appears beside him. Walk into the snake's fireballs, the shooting snake, the fire at x113, and the ghost from x101, turning your back to bring it in.

**SAY:**

> A fairy lasts ten seconds, shown flying beside the player.
> While it lasts nothing hurts me, and whatever I touch is destroyed, even a fire and the ghost.

**SHOW** — Stand still until the fairy disappears.

> Ten seconds, and it's gone.

### **SHOW** — The authored egg at x118. Open it, and walk into the pit at x122. A strike. Cut the walk in editing if the first fairy ran out early.

> A pit still costs a strike, even with a fairy.

---

## Cut 10 — Level 2's start `0:22`

Rows: 4.6, 4.8, 6.3, 7.11.

### **SHOW** — Press Play. Open the axe egg and the heart egg, take the three fruit, and ride into the door. Level 2: he arrives riding, the counter at 03, the bar at 9.

**SAY:**

> I'm riding, holding an axe, with three fruit taken.
> Level 1 leads straight into level 2.
> The mount, the axe and the fruit count come with me, and the power restarts at 9.

---

## Cut 11 — Inspector `0:34`

Rows: 8.26, 10.3, 10.4, and the numbers of 3.1, 4.1, 4.2, 4.7, 6.7, 8.4, 9.1. Recorded on the authored
scene, in Edit mode, with the Hierarchy and the Inspector on screen.

### **SHOW** — Select `SceneContext > GameInstaller`. Then `Level_1` and `Level_2`: Starting Power 11 and 9. Then `Player`, with the Inspector scrolled to its last component, **Player Fairy**: Seconds 10, and Marker pointing at its child `Fairy`.

**SAY:**

> The game's rules are numbers on the installer, so any of them can be changed here.
> Three strikes, a strike every twenty fruit, a bar of 16 that loses one every three seconds, three axes and one boomerang in the air, and an enemy back in ten to twenty seconds.
> Each level sets its own starting power, and the fairy's ten seconds are on the player.

### **SHOW** — Select the egg at x14 and open its Drop list, then the egg at x118.

> An egg or an enemy holds one drop, chosen from these six: the heart, the leaf, the star, the axe, the boomerang and the fairy.