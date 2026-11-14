using Raylib_cs;

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
}
