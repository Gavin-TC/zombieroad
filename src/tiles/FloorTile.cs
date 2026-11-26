using Raylib_cs;

namespace zombieroad.src.tiles;

public class FloorTile : Tile
{
    private static readonly Random random = new Random();
    private static readonly byte[] glyphs = { (byte)' ', (byte)',', (byte)'.', (byte)'`' };
    private static readonly int GRASS_CHANCE = 45;

    public FloorTile(bool isIndoors) : base((byte)' ', Color.White, Color.Black, 100, !isIndoors, isIndoors, false, false, false)
    {
        setIsIndoors(isIndoors);
    }

    public override void setIsIndoors(bool isIndoors)
    {
        base.setIsIndoors(isIndoors);

        isExplored = !isIndoors;

        int change = (int)random.NextInt64(0, 100);
        int idx = (int)random.NextInt64(0, glyphs.Length);

        if (change <= 100 - GRASS_CHANCE) idx = 0;

        glyph = isIndoors ? glyphs[0] : glyphs[idx];

        fgColor = isIndoors ? Color.Blank : Color.Green;
        bgColor = isIndoors ? Color.Black : Color.DarkGreen;
    }
}
