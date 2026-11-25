using Raylib_cs;
using zombieroad.src.tiles;

namespace zombieroad.src.entities;

public class Entity
{
    public String name { get; set; }
    public byte glyph { get; set; }
    public Color fgColor { get; set; }
    public int health { get; set; }

    public int x { get; set; }
    public int y { get; set; }

    public Entity(String name, byte glyph, Color fgColor, int health, int x, int y)
    {
        this.name = name;
        this.health = health;
        this.glyph = glyph;
        this.fgColor = fgColor;

        this.x = x;
        this.y = y;
    }

    public Entity(String name, int health, int x, int y)
    {
        this.name = name;
        this.health = health;
        fgColor = Color.White;

        this.x = x;
        this.y = y;
    }

    public virtual void update(TileMap map, List<Entity> entities, Player player)
    {
    }

    public virtual void damage(int damage)
    {
        health = Math.Max(health - damage, 0);

        if (health == 0) Console.WriteLine($"{name} died");
    }
}
