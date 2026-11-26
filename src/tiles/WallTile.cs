using Raylib_cs;

namespace zombieroad.src.tiles;

public class WallTile : Tile
{
    public WallTile() : base((byte)219, Color.Gray, Color.Black, 100, true, false, true, true, true)
    {
    }
}
