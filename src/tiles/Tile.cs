using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class Tile {
    public byte glyph;
    public Color fgColor;
    public Color bgColor;

    public int health = 100;
    public Func<Tile?>? debris { get; set; }   // Optional tile that replaces the current one after destruction
    public Func<Entity?>? drop { get; set; }   // Optional entity/tile that replaces the current tile after destruction

    public bool isDestructible { get; set; } = true;
    public bool isIndoors { get; set; }
    public bool isVisible { get; set; } = false;
    public bool isExplored { get; set; } = false; // Has the player seen this tile before
    public bool blocksMovement { get; set; }
    public bool blocksVision { get; set; }
    public bool blocksLight { get; set; }

    public Tile(
        byte glyph,
        Color fgColor,
        Color bgColor,
        int health,
        bool isDestructible,
        bool isIndoors,
        bool blocksMovement,
        bool blocksVision,
        bool blocksLight
    )
    {
        this.glyph = glyph;
        this.fgColor = fgColor;
        this.bgColor = bgColor;

        this.health = health;

        this.isDestructible = isDestructible;
        this.isIndoors = isIndoors;
        this.blocksMovement = blocksMovement;
        this.blocksVision = blocksVision;
        this.blocksLight = blocksLight;
    }

    public virtual bool interact(TileMap map, List<Entity> entities, Entity interactor, int x, int y)
    {
        return false;
    }

    public virtual bool damage(TileMap map, int x, int y, int damage)
    {
        if (!isDestructible) return false;

        health = Math.Max(health - damage, 0);

        if (health == 0) destroy(map, x, y);
        else             onDamaged();

        return true;
    }

    protected virtual void onDamaged() {}
    
    protected virtual void destroy(TileMap map, int x, int y)
    {
        map.setObject(x, y, debris?.Invoke());
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
