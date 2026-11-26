using Raylib_cs;
using zombieroad.src.entities;

namespace zombieroad.src.tiles;

public class TileMap
{
    private Tile[][] floor = [];
    private Tile?[][] objects = [];

    public int height => floor.Length;
    public int width => floor.Length > 0 && floor[0] is not null ? floor[0].Length : 0;

    // Is this cell part of the generated/loaded world?
    public bool inBounds(int x, int y) => x >= 0 && y >= 0 && y < height && x < width;

    public Tile getFloor(int x, int y) => floor[y][x];
    public Tile? getObject(int x, int y) => objects[y][x];
    public void setObject(int x, int y, Tile? tile) => objects[y][x] = tile;

    public Tile getTop(int x, int y) => getObject(x, y) ?? getFloor(x, y);
    public bool isIndoors(int x, int y) => getTop(x, y).isIndoors;
    public bool blocksMovement(int x, int y) => getTop(x, y).blocksMovement;
    public bool blocksVision(int x, int y) => getTop(x, y).blocksVision;

    // Stamps the image onto the map with its top-left corner at (offsetX, offsetY)
    public void generateMapFromImage(String filepath, List<Entity> entities, Player player, int offsetX = 0, int offsetY = 0)
    {
        Image img = Raylib.LoadImage(filepath);
        floor = new Tile[offsetY + img.Height][];
        objects = new Tile?[offsetY + img.Height][];

        for (int y = 0; y < floor.Length; y++)
        {
            floor[y] = new Tile[offsetX + img.Width];
            objects[y] = new Tile?[offsetX + img.Width];

            for (int x = 0; x < floor[y].Length; x++) floor[y][x] = new FloorTile(false);
        }

        for (int iy = 0; iy < img.Height; iy++)
        {
            for (int ix = 0; ix < img.Width; ix++)
            {
                int x = ix + offsetX;
                int y = iy + offsetY;
                Color pixel = Raylib.GetImageColor(img, ix, iy);

                if (pixel.Equals(Color.Black))     setObject(x, y, new WallTile());
                if (pixel.Equals(Color.Brown))     setObject(x, y, new DoorTile(false));
                if (pixel.Equals(Color.Gray))      setObject(x, y, new FloorTile(true));
                if (pixel.Equals(Color.Red))
                {
                    WindowTile windowTile = new WindowTile(false);
                    WindowTile? neighbor = null;

                    if      (ix > 0 && getObject(x - 1, y) is WindowTile leftWin) neighbor = leftWin;
                    else if (iy > 0 && getObject(x, y - 1) is WindowTile topWin)  neighbor = topWin;

                    if (neighbor != null)
                    {
                        neighbor.windowGroup ??= new List<WindowTile> { neighbor };
                        neighbor.windowGroup.Add(windowTile);
                        windowTile.windowGroup = neighbor.windowGroup;
                    }

                    setObject(x, y, windowTile);
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

        int stampWidth = img.Width;
        int stampHeight = img.Height;
        Raylib.UnloadImage(img);

        indoorizeSpawnCells(entities);
        revealBuildingExteriors(offsetX, offsetY, stampWidth, stampHeight);
    }

    private static readonly (int dx, int dy)[] dirs = { (0, -1), (0, 1), (-1, 0), (1, 0) };

    // Spawn pixels carry no floor info, so adopt the indoor floor of an adjacent room tile
    private void indoorizeSpawnCells(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            foreach (var (dx, dy) in dirs)
            {
                int nx = e.x + dx;
                int ny = e.y + dy;

                if (inBounds(nx, ny) && getObject(nx, ny) is FloorTile { isIndoors: true })
                {
                    setObject(e.x, e.y, new FloorTile(true));
                    break;
                }
            }
        }
    }

    private void revealBuildingExteriors(int startX, int startY, int w, int h)
    {
        for (int y = startY; y < startY + h; y++)
        {
            for (int x = startX; x < startX + w; x++)
            {
                Tile? obj = getObject(x, y);
                if (obj is null || obj is FloorTile) continue;

                foreach (var (dx, dy) in dirs)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (!inBounds(nx, ny) || getObject(nx, ny) is not null) continue;

                    if (!getFloor(nx, ny).isIndoors)
                    {
                        obj.isExplored = true;
                        break;
                    }
                }
            }
        }
    }
}
