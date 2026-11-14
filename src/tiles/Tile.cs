using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class Tile {
    public byte glyph;
    public Color fgColor;
    public Color bgColor;

    public bool isIndoors;
    public bool blocksMovement;
    public bool blocksVision;
    public bool blocksLight;

    public Tile(
        byte glyph,
        Color fgColor,
        Color bgColor,
        bool isIndoors,
        bool blocksMovement,
        bool blocksVision,
        bool blocksLight
    )
    {
        this.glyph = glyph;
        this.fgColor = fgColor;
        this.bgColor = bgColor;

        this.isIndoors = isIndoors;
        this.blocksMovement = blocksMovement;
        this.blocksVision = blocksVision;
        this.blocksLight = blocksLight;
    }

    public virtual bool interact(Entity interactor)
    {
        return false;
    }
};
