using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class DoorTile : Tile
{
    private const byte openedGlyph = (byte)'+';
    private const byte closedGlyph = (byte)'=';
    private bool isOpen { get; set; } = false;
    private bool isBlocked { get; set; } = false; // Is the door currently blocked, i.e. boarded/object behind it

    public DoorTile(bool isOpen = true)
        : base(isOpen ? openedGlyph : closedGlyph, Color.Brown, Color.Black, true, true, true, true)
    {
    }

    public override bool interact(Entity interactor)
    {
        if (isBlocked) return false;

        if (isOpen)
        {
            glyph = closedGlyph;
            blocksVision = true;
            blocksMovement = true;
            blocksLight = true;
        }
        else
        {
            glyph = openedGlyph;
            blocksVision = false;
            blocksMovement = false;
            blocksLight = false;
        }

        return true;
    }
}

