# Turns the raw Adventure Island sprite sheets into cut-ready copies: background removed,
# upscaled 3x with no smoothing. Each sheet is written twice - once with a real transparent
# background, once with that background filled with a colour the sheet's own art never uses,
# which is the copy to cut from by hand. Cutting is not done here, and nothing is written
# into Assets/.
#
# Run from the folder this file is in:  python prepare_sheets.py

import os
import sys
import zipfile
from collections import deque

import numpy as np
from PIL import Image

ZIP = r"c:\Users\peleg\OneDrive\Desktop\Unity\2026\Course\Exercises\Copy of NES - Adventure Island.zip"
OUT = "SheetsReady"
SCALE = 3

# How each sheet's background has to come off. "flood" clears only background reachable from
# the border, which is what keeps the white stars on 29903 and every black outline. "global"
# clears the colour everywhere, used only where it is provably absent from the art. An empty
# list means the file already carries a real alpha channel and is upscaled untouched.
SHEETS = {
    "123.png": ([], None, "player, and the three animals with the rider drawn on"),
    "208311.png": ([(0, 128, 0), (116, 116, 116)], "flood", "24 enemies, labelled - green sheet, grey cell boxes"),
    "29903.png": ([(255, 255, 255)], "flood", "AI2 animals without rider, card-suit tokens"),
    "32880.png": ([(0, 136, 255)], "flood", "snail and bat"),
    "33298.png": ([(255, 0, 255)], "global", "AI1 player and items - axe, egg, fairy, fruit, digits"),
    "33299.png": ([(255, 0, 255)], "global", "AI1 enemies - the fire and the boulder are here"),
    "88427.gif": ([], None, "AI3 player, throw animations"),
    "88429.gif": ([], None, "a few AI3 enemies, mirrored"),
    "NES - Tinas Adventure Island 3 Hack - Characters & Items.png": ([], None, "fruit, fairy, clover"),
    "NES - Adventure Island 3 - Bonus Rooms & Friendly NPCs.png": ([(0, 0, 0), (51, 102, 255)], "flood", "cave wall tiles, egg pedestals"),
    "NES - Adventure Island 2 - Cave Island Boss.png": ([(255, 0, 255)], "global", "yellow ground strip, white spikes"),
    "NES - Adventure Island 2 - Cloud Island Boss.png": ([(255, 0, 255)], "global", "cloud platforms"),
    "NES - Adventure Island 2 - Fern Island Boss.png": ([(255, 0, 255)], "global", "green platforms"),
    "NES - Adventure Island 2 - Volcano Island Boss.png": ([(255, 0, 255)], "global", "lava ground, spikes"),
    "NES - Adventure Island 3 - Area 3 Boss.png": ([(255, 0, 255)], "global", "purple crystal walls, orange floor"),
    "NES - Adventure Island 3 - Area 8 Boss.png": ([(255, 0, 255)], "global", "purple stone, platforms, pink spikes"),
}

# 33382.png is left out of the list: it is byte-for-byte the Fern Island Boss sheet.

# Tried in order for the cut-from copy's background. The first one a sheet's art does not
# already contain wins, so a hand cut can never lose a pixel that belonged to a sprite.
KEY_CANDIDATES = [(255, 0, 255), (0, 255, 255), (255, 0, 128), (0, 255, 0), (128, 0, 255), (255, 255, 0)]


def matches_any(rgb, colours):
    hit = np.zeros(rgb.shape[:2], bool)
    for c in colours:
        hit |= (rgb[:, :, 0] == c[0]) & (rgb[:, :, 1] == c[1]) & (rgb[:, :, 2] == c[2])
    return hit


def flood_from_border(mask):
    h, w = mask.shape
    out = np.zeros_like(mask)
    q = deque()
    for x in range(w):
        for y in (0, h - 1):
            if mask[y, x] and not out[y, x]:
                out[y, x] = True
                q.append((y, x))
    for y in range(h):
        for x in (0, w - 1):
            if mask[y, x] and not out[y, x]:
                out[y, x] = True
                q.append((y, x))
    while q:
        y, x = q.popleft()
        for ny, nx in ((y + 1, x), (y - 1, x), (y, x + 1), (y, x - 1)):
            if 0 <= ny < h and 0 <= nx < w and mask[ny, nx] and not out[ny, nx]:
                out[ny, nx] = True
                q.append((ny, nx))
    return out


def pick_key_colour(rgba):
    art = rgba[rgba[:, :, 3] > 0][:, :3]
    if len(art) == 0:
        return KEY_CANDIDATES[0]
    used = set(map(tuple, np.unique(art, axis=0)))
    for c in KEY_CANDIDATES:
        if c not in used:
            return c
    sys.exit("no free key colour - add another to KEY_CANDIDATES")


def main():
    if not os.path.exists(ZIP):
        sys.exit("sheet zip not found: " + ZIP)
    os.makedirs(os.path.join(OUT, "transparent"), exist_ok=True)
    os.makedirs(os.path.join(OUT, "cut_from"), exist_ok=True)

    zf = zipfile.ZipFile(ZIP)
    for name, (bg, mode, note) in SHEETS.items():
        with zf.open(name) as fh:
            im = Image.open(fh).convert("RGBA")
        a = np.array(im)

        if bg:
            hit = matches_any(a[:, :, :3], bg)
            if mode == "flood":
                hit = flood_from_border(hit)
            a[hit] = (0, 0, 0, 0)

        big = Image.fromarray(a).resize((im.width * SCALE, im.height * SCALE), Image.NEAREST)
        stem = os.path.splitext(name)[0].replace("NES - ", "")
        big.save(os.path.join(OUT, "transparent", stem + ".png"))

        key = pick_key_colour(np.array(big))
        flat = Image.new("RGBA", big.size, key + (255,))
        flat.alpha_composite(big)
        flat.convert("RGB").save(os.path.join(OUT, "cut_from", stem + ".png"))

        clear = round(100 * float((np.array(big)[:, :, 3] == 0).mean()))
        print("%-52s %4dx%-4d %3d%% clear  key=%-14s %s"
              % (stem, big.width, big.height, clear, "%d,%d,%d" % key, note))

    print("\n%s/transparent/  the real sprites" % OUT)
    print("%s/cut_from/     open these in Paint - background is a colour the art never uses" % OUT)


if __name__ == "__main__":
    main()
