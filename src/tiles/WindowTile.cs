using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class WindowTile : Tile
{
    private const byte openedGlyph = 176;
    private const byte closedGlyph = 178;

    public bool blindsOpen { get; set; }
    public bool isPeeked { get; private set; }
    public List<WindowTile>? windowGroup { get; set; }

    public WindowTile(bool blindsOpen) : base(178, Color.White, Color.Black, true, true, true, true)
    {
        setBlindsState(blindsOpen);
    }

    private void setBlindsState(bool open)
    {
        isPeeked = false;

        if (open)
        {
            glyph = openedGlyph;
            blindsOpen = true;
            blocksVision = false;
            blocksLight = false;
        }
        else
        {
            glyph = closedGlyph;
            blindsOpen = false;
            blocksVision = true;
            blocksLight = true;
        }
    }

    public void setPeeking(bool peeking)
    {
        if (blindsOpen) return; // Nothing to peek through

        isPeeked = peeking;
        blocksVision = !peeking; // blocksLight stays true, the blinds are still closed
    }

    public override bool interact(Entity interactor, List<Entity> entities, Tile[][] tiles, int x, int y)
    {
        // Only allow window interaction if player is on an indoor tile
        // (stops player from opening windows from outside)
        if (!tiles[y][x].isIndoors) return false;

        if (windowGroup != null)
        {
            bool targetState = !blindsOpen;

            foreach (WindowTile w in windowGroup)
            {
                w.setBlindsState(targetState);
            }
        }
        else
        {
            setBlindsState(!blindsOpen);
        }

        return true;
    }
}
