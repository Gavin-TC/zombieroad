using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class DoorTile : Tile
{
    private const byte openedGlyph = (byte)'+';
    private const byte closedGlyph = (byte)'=';
    private bool isOpen { get; set; }
    private bool isLocked { get; set; }
    private bool isBlocked { get; set; } // Is the door currently blocked, i.e. boarded

    public DoorTile(bool isOpen = false, bool isLocked = false, bool isBlocked = false)
        : base(isOpen ? openedGlyph : closedGlyph, Color.Brown, Color.Black, true, !isOpen, true, true)
    {
        setOpenState(isOpen);
        this.isLocked = isLocked;
        this.isBlocked = isBlocked;
    }

    private void setOpenState(bool open)
    {
        if (open)
        {
            glyph = openedGlyph;
            isOpen = true;
            blocksVision = false;
            blocksMovement = false;
            blocksLight = false;
        }
        else
        {
            glyph = closedGlyph;
            isOpen = false;
            blocksVision = true;
            blocksMovement = true;
            blocksLight = true;
        }
    }

    public override bool interact(Entity interactor, Tile[][] tiles, int x, int y)
    {
        if (isBlocked) return false;

        if (isLocked)
        {
            isLocked = false;

            return true;
        }

        setOpenState(!isOpen);

        return true;
    }

    public bool getIsBlocked()
    {
        return isBlocked;
    }
}

