using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class TileMap
{
    public Tile[][] floor;
    public Tile?[][] objects;

    public TileMap(int width, int height)
    {
        floor = new Tile[height][];
        objects = new Tile[height][];
    }

    public int height => floor.Length;
    public int width => floor.Length > 0 && floor[0] is not null ? floor[0].Length : 0;

    public bool inBounds(int x, int y) => x >= 0 && y >= 0 && y < height && x < width;
    public bool isIndoors(int x, int y) => objects[y][x]?.isIndoors ?? floor[y][x].isIndoors;

    public Tile getTop(int x, int y) => objects[y][x] ?? floor[y][x];
    public bool blocksMovement(int x, int y) => objects[y][x]?.blocksMovement ?? floor[y][x].blocksMovement;
    public bool blocksVision(int x, int y) => objects[y][x]?.blocksVision ?? floor[y][x].blocksVision;

    public void generateMapFromImage(String filepath, List<Entity> entities, Player player)
    {
        Image img = Raylib.LoadImage(filepath);
        floor = new Tile[img.Height][];
        objects = new Tile[img.Height][];

        for (int y = 0; y < img.Height; y++)
        {
            floor[y] = new Tile[img.Width];
            objects[y] = new Tile?[img.Width];

            for (int x = 0; x < img.Width; x++)
            {
                Color pixel = Raylib.GetImageColor(img, x, y);

                floor[y][x] = new FloorTile(false);

                if (pixel.Equals(Color.Black))     objects[y][x] = new WallTile();
                if (pixel.Equals(Color.Brown))     objects[y][x] = new DoorTile(false);
                if (pixel.Equals(Color.Gray))      objects[y][x] = new FloorTile(true);
                if (pixel.Equals(Color.Red))
                {
                    WindowTile windowTile = new WindowTile(false);
                    WindowTile? neighbor = null;

                    if      (x > 0 && objects[y][x - 1] is WindowTile leftWin) neighbor = leftWin;
                    else if (y > 0 && objects[y - 1][x] is WindowTile topWin)  neighbor = topWin;

                    if (neighbor != null)
                    {
                        neighbor.windowGroup ??= new List<WindowTile> { neighbor };
                        neighbor.windowGroup.Add(windowTile);
                        windowTile.windowGroup = neighbor.windowGroup;
                    }

                    objects[y][x] = windowTile;
                }
                if (pixel.Equals(Color.Blue)) 
                {
                    player.setX(x);
                    player.setY(y);
                }
                if (pixel.Equals(Color.Green)) 
                {
                    Zombie zombie = new Zombie("Zombie", 100, x, y);
                    entities.Add(zombie);
                }
            }
        }

        Raylib.UnloadImage(img);
    }
}
