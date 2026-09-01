# Cleans up sprites cropped by hand out of SheetsReady/cut_from/. It clears the background,
# trims to the art, and pads the result out to a whole number of 48px cells so one cell stays
# one world unit. How loosely the crop was drawn makes no difference to what comes out - only
# the trim decides the sprite's extent - so the one thing a crop must not do is take in part
# of the sprite next to it, which on these sheets can sit 3px away. The reported size is how
# that gets caught: a spider measuring 2 x 1 units took in its neighbour.
#
# Run from the folder this file is in:  python finish_sprites.py

import os
import re
import sys

import numpy as np
from PIL import Image

IN = "Cut"
OUT = "Ready"
CELL = 48

# Backgrounds to clear from a cut wherever they appear in it. Magenta is what prepare_sheets.py
# fills its cut-from sheets with, and it checks that colour against every opaque pixel before
# using it. The rest are patches the original rippers left behind, sealed inside a sprite's
# outline where their own flood fill could not reach: two shades of khaki and a purple box on
# 123.png, and the green boxes holding the effects on 29903.png. Every one was checked against
# all sixteen sheets' art first - the pale pink that looks equally out of place is the pink
# animal's belly, and the olive that looks like background is the door's frame.
CLEAR = [(255, 0, 255), (196, 197, 107), (222, 223, 129), (146, 39, 143), (0, 255, 0)]

# Sprites drawn on the Canvas rather than in the world. The 48px cell means nothing to them, and
# padding the 9px-wide power bar out to a 48px box would leave a UI Image mostly empty.
NOPAD = [
    "Sprite_Strength_Line",
    "Sprite_Strike",
]

# Frames that keep their own tight box despite their name starting with a character's. An effect
# or a projectile is spawned at its own position, so padding a 24px fireball out to the rider's
# 2x4 box would leave its pivot sitting in empty space.
ALONE = [
    "Sprite_Player_Blue_Mount_Hit",
    "Sprite_Player_Red_Mount_FireBall",
    "Sprite_Enemy_Snake_FireBall",
]

# Names that share one box, longest prefix first. Every frame a character can be in has to come
# out the same size, or it slides sideways the moment the Animator changes state: a frog's idle
# frame one cell wide beside its jumping frame two cells wide moves it half a unit.
GROUPS = [
    "Sprite_Enemy_Snake",
    "Sprite_Enemy_Frog",
    "Sprite_Enemy_Ghost",
    "Sprite_Enemy_Bird",
    "Sprite_Enemy_Spider",
    "Sprite_Egg",
    "Sprite_Player_Blue_Mount",
    "Sprite_Player_Red_Mount",
    "Sprite_Player_Green_Mount",
    "Sprite_Player",
]


def trim(a):
    ys, xs = np.nonzero(a[:, :, 3] > 0)
    if len(ys) == 0:
        return None
    return a[ys.min():ys.max() + 1, xs.min():xs.max() + 1]


def key_and_trim(path):
    a = np.array(Image.open(path).convert("RGBA"))
    # Cleared everywhere rather than flooded in from the border. Flooding cannot reach a pocket
    # sealed inside an outline, and cannot tell a solid-coloured sprite from a background at
    # all - it ate the whole power bar and stripped the frame off the door.
    for c in CLEAR:
        a[(a[:, :, 0] == c[0]) & (a[:, :, 1] == c[1]) & (a[:, :, 2] == c[2])] = (0, 0, 0, 0)
    return trim(a)


def group_of(stem):
    for a in ALONE:
        # matched before GROUPS, since these names start with a character's prefix
        if stem.startswith(a):
            return stem
    for g in GROUPS:
        if stem.startswith(g):
            return g
    # Anything not listed shares a box only with its own numbered frames.
    return re.sub(r"_\d+$", "", stem)


def main():
    if not os.path.isdir(IN):
        sys.exit("no %s/ folder - put the hand-cut PNGs there" % IN)
    os.makedirs(OUT, exist_ok=True)

    trimmed = {}
    for f in sorted(os.listdir(IN)):
        if f.lower().endswith(".png"):
            art = key_and_trim(os.path.join(IN, f))
            if art is None:
                print("%-38s SKIPPED - nothing left after keying" % f)
            else:
                trimmed[os.path.splitext(f)[0]] = art

    groups = {}
    for stem, art in trimmed.items():
        groups.setdefault(group_of(stem), []).append(art)

    sizes = {}
    for g, arts in groups.items():
        mw = max(a.shape[1] for a in arts)
        mh = max(a.shape[0] for a in arts)
        sizes[g] = (-(-mw // CELL) * CELL, -(-mh // CELL) * CELL)

    suspect = 0
    for stem in sorted(trimmed):
        art = trimmed[stem]
        ui = any(stem.startswith(u) for u in NOPAD)
        bw, bh = (art.shape[1], art.shape[0]) if ui else sizes[group_of(stem)]
        canvas = np.zeros((bh, bw, 4), np.uint8)
        # Centred across, sitting on the floor of the box, so a creature's feet land on the
        # cell boundary and the slack ends up over its head.
        x0 = (bw - art.shape[1]) // 2
        y0 = bh - art.shape[0]
        canvas[y0:y0 + art.shape[0], x0:x0 + art.shape[1]] = art
        Image.fromarray(canvas).save(os.path.join(OUT, stem + ".png"))

        # Most sheets draw on a 16px grid and everything here is 3x, so art spilling a little
        # past a cell boundary has usually taken in the sprite beside it. Art falling short is
        # just a sprite not filling its cell, and is left alone. 123.png warns freely and can be
        # ignored: its animals are 27 to 29px wide, so nothing on it lands on a multiple of 16.
        note = ""
        if not ui:
            for n, side in ((art.shape[1], "wide"), (art.shape[0], "tall")):
                over = n % CELL
                if 1 <= over <= 20:
                    note = "  <-- check: %dpx %s overshoots a cell by %d" % (n, side, over)
        suspect += bool(note)
        size = "on the Canvas, unpadded" if ui else "%g x %g units" % (bw / CELL, bh / CELL)
        print("%-38s art %3dx%-3d  ->  %3dx%-3d = %s%s"
              % (stem, art.shape[1], art.shape[0], bw, bh, size, note))

    print("\n%d sprites in %s/ - copy them into Assets/Sprites/" % (len(trimmed), OUT))
    if suspect:
        print("%d marked 'check' - all of them are from 123.png this time, and are fine" % suspect)


if __name__ == "__main__":
    main()
