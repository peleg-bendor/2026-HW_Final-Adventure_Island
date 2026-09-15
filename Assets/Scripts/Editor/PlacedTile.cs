// One object in a level root, as the level file stores it: a cell and a tile id, with the object's
// name kept for the warnings. Gathered before anything is written, so the grid fits what was found.
public struct PlacedTile
{
    public int x;
    public int y;
    public int tileId;
    public string name;
}
