# Cuts the five ground tiles straight out of the prepared sheets, replacing whatever is in
# Cut/ under those names. They are done here rather than by hand because a ground tile has to
# be exactly 48x48 and contain no background at all: any transparent pixel in it shows as a
# hole once the tiles are laid edge to edge, which is what the first hand-cut set did at every
# corner. Every window below was checked for that, and for tiling without a seam.
#
# Run from the folder this file is in:  python cut_tiles.py

import os
import sys

import numpy as np
from PIL import Image

SHEETS = "SheetsReady/cut_from"
OUT = "Cut"
CELL = 48

# name, sheet, x, y - in cut-from pixels, which are 3x the original art
TILES = [
    ("Sprite_Tile_Earth_1", "Adventure Island 2 - Cave Island Boss", 2421, 468),
    ("Sprite_Tile_Earth_2", "Adventure Island 2 - Fern Island Boss", 3295, 323),
    ("Sprite_Tile_Earth_3", "Adventure Island 2 - Volcano Island Boss", 145, 24),
    ("Sprite_Tile_Earth_4", "Adventure Island 3 - Area 3 Boss", 527, 32),
    ("Sprite_Tile_Earth_5", "Adventure Island 3 - Area 8 Boss", 334, 32),
]


def main():
    if not os.path.isdir(SHEETS):
        sys.exit("run prepare_sheets.py first - no %s/" % SHEETS)
    os.makedirs(OUT, exist_ok=True)

    for name, sheet, x, y in TILES:
        path = os.path.join(SHEETS, sheet + ".png")
        if not os.path.exists(path):
            sys.exit("missing sheet: " + path)
        tile = Image.open(path).convert("RGB").crop((x, y, x + CELL, y + CELL))

        a = np.array(tile)
        holes = int(((a[:, :, 0] == 255) & (a[:, :, 1] == 0) & (a[:, :, 2] == 255)).sum())
        if holes:
            sys.exit("%s would have %d transparent px - pick another window" % (name, holes))

        tile.save(os.path.join(OUT, name + ".png"))
        print("%-22s %s  x=%-5d y=%-4d  solid" % (name, sheet[-26:], x, y))

    print("\n%d tiles written to %s/ - now run finish_sprites.py" % (len(TILES), OUT))


if __name__ == "__main__":
    main()
