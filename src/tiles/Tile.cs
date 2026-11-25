using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class Tile {
    public byte glyph;
    public Color fgColor;
    public Color bgColor;

    public bool isIndoors { get; set; }
    public bool isVisible { get; set; } = false;
    public bool isExplored { get; set; } = true; // Has the player seen this tile before
    public bool blocksMovement { get; set; }
    public bool blocksVision { get; set; }
    public bool blocksLight { get; set; }

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

    public virtual bool interact(TileMap map, List<Entity> entities, Entity interactor, int x, int y)
    {
        return false;
    }

    public virtual void setIsIndoors(bool isIndoors)
    {
        this.isIndoors = isIndoors;
    }

    public virtual void setBlocksMovement(bool blocksMovement)
    {
        this.blocksMovement = blocksMovement;
    }

    public virtual void setBlocksVision(bool blocksVision)
    {
        this.blocksVision = blocksVision;
    }

    public virtual void setBlocksLight(bool blocksLight)
    {
        this.blocksLight = blocksLight;
    }
};
