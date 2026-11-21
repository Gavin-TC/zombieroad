using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class WindowTile : Tile
{
    private const byte openedGlyph = 176;
    private const byte closedGlyph = 178;

    public bool blindsOpen { get; set; }
    public List<WindowTile>? windowGroup { get; set; }

    public WindowTile(bool blindsOpen) : base(178, Color.White, Color.Black, true, true, true, true)
    {
        setBlindsState(blindsOpen);
    }

    private void setBlindsState(bool open)
    {
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

    public override bool interact(Entity interactor, Tile[][] tiles, int x, int y)
    {
        // Only allow window interaction if player is on an indoor tile
        if (!tiles[(int)y][(int)x].isIndoors) return false;

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
