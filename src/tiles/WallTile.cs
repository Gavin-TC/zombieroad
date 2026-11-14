using Raylib_cs;

namespace zombieroad.src.tiles;

public class WallTile : Tile
{
    public WallTile() : base((byte)219, Color.Gray, Color.Black, false, true, true, true)
    {
    }
}
