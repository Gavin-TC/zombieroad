using Raylib_cs;

namespace zombieroad.src.tiles;

public class FloorTile : Tile
{
    public FloorTile(bool isIndoors) : base((byte)' ', Color.White, Color.Black, isIndoors, false, false, false)
    {
    }
}
