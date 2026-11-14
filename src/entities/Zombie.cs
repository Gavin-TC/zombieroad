using Raylib_cs;

namespace zombieroad.src.entities;

public class Zombie : Entity
{
    public Zombie(String name, int health, int x, int y) : base(name, health, x, y)
    {
        glyph = 2;
    }
}