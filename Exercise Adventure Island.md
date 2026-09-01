# Exercise: Adventure Island

The final project's requirements, in one place and decided. Built by reading `Final Project.md` and
`Course/Lesson 12 Dependency Injection/תמלול_הסרטון.md` together. **Where the two disagree, the
transcript wins**, because the instructor corrected and extended the written text on camera in a
dozen places and the document was never updated to match.

Written in Stage 0, 31.8.2026. Every later stage is written against this file.

## How to read this

- Requirements are numbered `section.item`, so `5.3` is section 5, requirement 3. The videos call
  each one out in Hebrew as `דרישה 5.3`, the way Exercises 1 to 3 did.
- New requirements get appended inside their own section, so a number, once given, stays put.
- **Everything here is required unless it carries a tag.** The tags are `[good]` (not required, worth
  doing), `[nice]` (optional, only if there is time) and `[out]` (deliberately not doing it).
- A timestamp like `(00:18:44)` points into the transcript. They appear where the transcript
  overrides the written text, or where the choice is one the oral defense is likely to ask about.
- Numbers written as `3` or `11` are starting values that belong in serialized fields, not constants
  in code. Every one of them is a number the instructor may want changed in front of you.

Hebrew is kept for the words that have to survive into the video: כוח, פסילה, and the names of the
objects and enemies.

## 1. Controls

1. The player moves left and right with the arrow keys.
2. Jump on `Space`. The instructor named this key himself (00:10:44): "יש לו אפשרות לקפוץ עם רווח או
   מה שלא תרצו".
3. Attack on `Z`.
4. Jump height varies with how long the key is held, as in the original and as in the Mario projects.
5. Jumping while riding an animal jumps the animal.
6. `[out]` Crouch on the down arrow, which lowers the attack. Neither source mentions crouching at
   all; it exists in the original game, but it changes what a thrown axe can hit and there are more
   important things to spend the four weeks on.

## 2. The game

1. Two levels, `Level_1` and `Level_2`. Finishing level 1 leads straight into level 2 with nothing
   in between.
1. A level ends when the player reaches its exit, a cave door. No key and no portal.
1. Finishing level 2 shows a popup congratulating the player, with a button that starts the game
   again (00:10:57).
1. Running out of פסילות shows a Game Over popup with a button that returns to the start of the game
   (00:11:54).
1. **No `SceneManager.LoadScene` anywhere in the project.** Not on death, not on game over, not on
   restart. Starting a level resets that level's own variables instead. He said this three times
   unprompted (00:11:43, 00:12:21, 00:13:12) and named it as a thing he marks down, because a reload
   hides static state that never gets reset. The level 1 to level 2 transition is the one case he
   never addressed out loud, but his reason applies to it identically, so it holds there too.
1. `[out]` A start menu, and a pause button. Neither source asks for either.

## 3. פסילות

1. The player starts the game with `3` פסילות.
2. A פסילה is lost by touching fire, by touching an enemy, by falling into a pit, by running out of
   כוח, and by the fruit count reaching a multiple of 20 (see 4.7).
3. Losing a פסילה returns the player to the start of the **current** level, not to level 1
   (00:11:14).
4. The level is **not** rebuilt. Enemies already killed stay dead unless their own respawn timer
   brings them back (00:11:29): "אם יש דברים שהרגתי אני לא רוצה לראות אותם שוב".
5. Enemies are the only thing that persists that way. Everything collectible comes back: fruit that
   was eaten, eggs that were opened, and any item placed in the level. This is what the original
   does, and without it a death that costs the animal also costs the egg that would have given it
   back, which can leave a level unwinnable.
6. Losing a פסילה costs the player the weapon he was carrying and the animal he was riding, and
   resets כוח to the level's starting amount. The fruit count survives.
7. Running out of פסילות ends the game, per 2.4.
8. `[out]` A rolling rock and a falling coconut. He lists both as ways to die in the original
   (00:14:03), but neither appears in the written requirements.

## 4. כוח and fruit

1. The כוח bar holds `16` units. Each level starts the player with `11`.
2. כוח drains by `1` unit every `2` seconds.
3. Two kinds of fruit. `Fruit_1` adds 1 unit of כוח, `Fruit_2` adds 2.
4. כוח cannot go past the bar's capacity. Fruit collected at full כוח is wasted (00:14:44): "יש
   מקסימום שהוא יכול לתת לך".
5. כוח reaching zero costs a פסילה.
6. כוח does not carry between levels. Level 2 starts at 4.1's amount regardless of how level 1 ended.
7. **Every `20` fruit costs a פסילה.** The count is not reset when it fires, so a פסילה is lost at
   20, at 40, at 60. Reaching 23 does nothing. Revised down from the 30 in the written text, on
   camera (00:51:30). This is the literal reading of "אם לוקחים 20 פירות מקבלים פסילה", and it is
   what constrains level length and the drain rate in 4.2 -- see the plan's Decisions Log.
8. The fruit count never resets. It survives a פסילה and it carries into level 2. Neither source says
   so either way; this is what the original does, checked in the emulator.

## 5. Hazards

### אבן

1. Touching an אבן costs `3` units of כוח. The original game uses 2.
2. Contact knocks the player forward, the way the original does, with a brief immunity for the length
   of the shove (00:17:49). Immunity alone is not enough, because the player is still inside the
   collider while being pushed. The shove can throw him into fire, which kills.
3. A boomerang destroys an אבן. **An axe does not** (00:18:44): "אני לא רוצה לראות שהגרזן מוריד אותו
   ואני רוצה לראות שהבומרנג והחיות מורידות אותו".
4. Any of the three animals' attacks destroys an אבן. The written text's singular "יריה של חיה" reads
   as the red animal only, but its own bullet list says "התקפה של החיות יכולה גם להרוס אבנים" and the
   transcript is plural too (00:18:30).
5. Riding into an אבן destroys both the אבן and the animal, and costs no כוח.

### מדורה

6. Touching a מדורה costs a פסילה.
7. Nothing destroys a מדורה except the פייה. Not an axe, not a boomerang, not an animal.
8. Riding into a מדורה destroys both the מדורה and the animal, and costs no פסילה.

### תהום

9. Falling into a תהום costs a פסילה. This is the only thing the פייה cannot save the player from
   (00:46:06).

## 6. Weapons

1. The player holds at most one weapon. Picking up a second replaces the first, as in the original.
   There is no switch key and no inventory; nothing in either source suggests one.
1. The player can only attack while holding a weapon or riding an animal.
1. A weapon carries into level 2, along with the animal being ridden (7.11). Only כוח resets at the
   level boundary. A weapon is lost on a פסילה, per 3.6.
1. A weapon is kept while riding an animal, and becomes usable again when the animal dies
   (00:38:40): "אם אני על חיה ואני לוקח בומרנג, אני עדיין נשאר על חיה... אם החיה תמות אני אראה את
   הבומרנג".
1. The written text calls the first weapon both גרזן and פטיש. They are the same object. This
   document calls it the axe.

### Axe

6. Thrown in an arc, as in the original.
7. There is a delay between throws and a limit on how many are in flight at once (00:31:21). There is
   no ammunition count.
8. Thrown axes are **pooled**. He asked for this by name while watching the game (00:35:02): "ופה אני
   רוצה שיהיה פולינג".
9. An axe that hits the ground returns to the pool. One that hits an enemy kills it and returns. The
   projectile has to tell the two apart (00:35:06).
10. An axe cannot destroy an אבן, per 5.3.

### בומרנג

11. Thrown forward to a set distance, then returns to the player's **current** position rather than
    the point it was thrown from.
12. Destroys everything in its path (00:32:02).
13. Disappears when caught, and returns to a pool the same way the axe does.
14. No ammunition count.

## 7. חיות

1. Three animals, blue, red and green. The other two in the sprite sheets are not wanted (00:32:14):
   "יש שלוש חיות שאני מבקש להשמיש, לא את כל החמישה".
1. An animal is mounted by collecting its token: לב for blue, עלה for red, כוכב for green.
1. Tokens come out of eggs and drop from destroyed enemies (00:37:18).
1. Blue attacks low, with its tail. Red spits fire from its mouth, so it hits higher. The height
   difference is deliberate (00:33:14): "הוא רואה את הדברים שיותר גבוהים, הוא רואה את הדברים שיותר
   נמוכים".
1. Green spins in place, and anything it touches while spinning is hit.
1. Red's fire reaches a few tiles ahead, not across the screen.
1. An animal's attack kills every enemy except the רוח רפאים.
1. An animal's attack destroys אבנים but not מדורות.
1. Collecting a token while already riding swaps to the new animal.
1. An animal absorbs one hit that would otherwise cost the player. Both the animal and whatever hit
   it disappear.
1. The animal being ridden carries into level 2, along with the weapon (6.3) and the fruit count
   (4.8) (00:16:38): "אין קשר מהשלב הראשון לשלב השני, אלא אם כן עם החיות שלקחת".
1. `[out]` The other two animals in the sheets, and the skateboard (00:22:47).

## 8. Enemies

1. Six types, all of them required: עכביש, ציפור, a jumping נחש, a shooting נחש, צפרדע and
   רוח רפאים.
1. Enemy attacks harm only the player and the animal he is riding. An enemy's projectile never kills
   another enemy. He calls this "חשוב מאוד" and specifically a code-design point (00:41:09): "שנגיד
   אתה יורה, פתאום הצפרדע קופץ על הירייה, היא לא מתה".
1. A destroyed enemy may drop something to collect. **What it drops is configured per enemy, never
   rolled at runtime**, and "drops nothing" is one of the options (00:46:39). The original game has
   no randomness in it at all (00:37:46).
1. A destroyed enemy comes back where it died after a countdown. **This delay is random**, suggested
   `10` to `20` seconds (00:44:49). It is the only randomness he asked for.
1. The game should not be difficult (00:43:21): "לא שהמשחק יהיה קשה. המשחק לא צריך להיות קשה. מה
   שמעניין אותי לראות שזה עובד".

### עכביש

6. Moves up and down on the Y axis only, or stands static in the air. Never left and right
   (00:39:20).
7. Which of the two is a field set per instance when the level is authored, not chosen at runtime.

### ציפור

8. Moves leftward at a constant speed while dipping and rising.
9. How far it dips varies per bird: some go all the way to the ground, some dip partway, some stay
   high (00:39:58). Speed, dip depth and wavelength are per-instance fields, so a bird with a dip of
   zero flies straight and stays high.
10. A bird kept high is the game's way of signalling a drop worth the effort of reaching it.
11. Leaves the level at the left edge and despawns. It does not loop back. The respawn timer in 8.4
    applies only to enemies that were destroyed.

### נחש, the jumper

12. Stands still, hops a short distance forward, stands still again, repeats.
13. It is allowed to be static instead, but the submission has to show it jumping (00:40:31).

### נחש, the shooter

14. Fires a projectile at the player from a distance, starting when the player comes into view.
15. The firing has to stop under some condition rather than run forever (00:40:58): "צריך לעצור את
    היריעה, כל אחד יעשה את זה בדרך שהוא מבין".
16. Its projectile does not harm other enemies, per 8.2.

### צפרדע

17. Jumps higher and further than the נחש, toward the player, triggered when the player comes close.
18. The timing of the jump is randomized so the player cannot read it (00:41:43): "הוא טיפה מרנדם את
    הקפיצה שלו". This is the only enemy behaviour he asked to be unpredictable.

### רוח רפאים

19. Not in the original game. It is his own addition, modelled on Mario's Boo (00:42:04).
20. Static while the player is facing it. Chases when the player's back is turned (00:42:38).
21. Only the פייה destroys it. It is the single exception to 7.7.
22. It stops chasing past a set distance, which needs a reference to the player and a distance check
    (00:44:04).
23. One is enough (00:43:21): "תעשה אחד, לא צריך כמה".
24. Its sprite is not in the sheets he provided; he could not extract one (00:21:34).

## 9. פייה

1. Lasts `10` seconds.
2. Makes the player invincible, and destroys anything he touches, including מדורה and the
   רוח רפאים.
3. Falling into a תהום still kills him. It is the only exception (00:45:51).
4. A level needs a place where a פייה can be collected and a תהום then fallen into. He said he will
   test this and ask about it if it is missing (00:46:06): "אני רוצה שיהיה מקום שאני אוכל לקחת פאה
   ואני אוכל ליפול לתהום... אני רוצה לבדוק את זה. אם זה לא יהיה, אני אשאל".
5. In `Final Project.md` this rule is split across two bullets and reads as broken: the sentence
   beginning "כל דבר שהוא יתקל בו" ends at the bottom of the ביצים bullet with "יהרס ולא יפגע בשחקן
   בכלל (חוץ מתהום)". Points 9.2 and 9.3 are what it says once rejoined.

## 10. ביצים and drops

1. Eggs are placed in the level by hand.
2. Stepping on an egg opens it (00:37:18).
3. An egg yields an animal token, a weapon or a פייה. Which one is set per egg, never rolled.
4. The first few enemies and eggs in level 1 should between them yield every drop type, so that all
   of them can be seen in the opening half-minute instead of after an hour of play (00:47:03): "אז
   תשימו את כל הדברים על החיה הראשונה, תלחמו איתה ותראו שהיא מפילה לכם הכל". This is the payoff of
   8.3 and it is how he intends to check the drop code.

## 11. The levels

1. Level 1 runs left to right, start to exit, like a classic Mario level.
2. Level 2 climbs bottom to top by jumping between platforms, then goes right at the top into the
   exit (00:51:13).
3. Falling in level 2 returns the player to the start of the level (00:49:33).
4. The player can climb back down to parts of level 2 he has already passed. The camera does not
   leave the playable area (00:50:30). Falling only kills into a תהום or onto spikes.
5. Level 1's reference is the gameplay video from 2:04. Level 2's is the second video from 7:15.
   Copying his level 1 outright is explicitly allowed (00:48:22).
6. One level gives the axe and the other the boomerang; one animal appears early and a different one
   near the end (00:38:01). This is so both weapons and more than one animal can be shown without
   replaying.
7. `[out]` A third level.

## 12. UI

1. The פסילות count is on screen. He asked for this specifically (00:51:47): "אני רוצה לראות גם את
   היואיי של הפסילות".
1. The fruit count is on screen.
1. The כוח bar is on screen.

## 13. Techniques

1. All seven, named by him: **DI, Pooling, Builder, Factory, MVC, Async & Tasks, Template**.
2. SOLID throughout. He runs an automated check over the submitted code and grades on the single
   worst place it finds, not the average (00:52:01): "אני הולך למקום היחידי שלא עשיתם אותו סוליד ועל
   זה אני שופט אתכם". Extra content is extra surface for that one bad spot to appear in.
3. He will ask why Tasks were used instead of a coroutine, and the reverse (00:57:58). Both
   directions, so the answer has to be a real one.
4. `[good]` Reflection. Not on the list and not required. He said he meant to include it, decided
   against adding it after the fact, and would be glad to see it (00:54:18).
5. `[good]` Tiled, or any other level pipeline. A student asked outright (00:57:40): "חייב יהיה
   להשתמש בתאילנד? לא, תשתמש איך שבא לך". The choice is free.
6. `[nice]` Animation. Optional, and a two-frame flip is acceptable as long as it reads clearly
   (00:24:01).
7. Where each of the seven lives is Stage 1's decision. The one exception is pooling the thrown axes,
   which he placed himself, per 6.8.

## 14. Submission

1. Due 26.9.2026, through Moodle.
2. **Two separate recordings**: one of gameplay showing every feature working, one walking through
   the code and explaining each part (00:54:46).
3. He watches the gameplay recording first and writes down the problems he sees, then watches the
   code one, and builds the defense questions out of both (00:54:58).
4. Several short recordings instead of one long take is allowed (00:55:11).
5. If the files are too large for Moodle, a download link or a YouTube link, and it has to stay live
   (00:55:25): "תדאגו שהלינק יהיה חי... לינק אבל שהוא לא ימות".
6. Oral defense on 28, 29 or 30 September, between 18:00 and 21:00, booked by email.

## 15. Deliberately not doing

Gathered from the `[out]` tags above, in one place, because four weeks of scope pressure makes this
list worth as much as the requirements. None of these earn marks, and each one adds code the SOLID
check in 13.2 can find its worst spot in.

- A third level, a third weapon, a seventh enemy, a fourth animal.
- The other two animals from the sheets, and the skateboard.
- More than one רוח רפאים.
- A rolling rock and a falling coconut.
- A start menu and a pause button.
- Crouching.
- Making the game hard.
