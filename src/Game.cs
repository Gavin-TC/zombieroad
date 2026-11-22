using Raylib_cs;
using System.Collections;
using System.Numerics;
using zombieroad.src.entities;
using zombieroad.src.rendering;
using zombieroad.src.tiles;

namespace zombieroad.src;

public class Game
{
    const String TITLE = "Zombie Road";

    int screenWidth = 1440;
    int screenHeight = 1080;
    int targetFPS = 240;
    int tileSize = 32;

    int turns = 0;

    Renderer renderer;
    Camera camera;
    Player player;

    Tile[][] tiles;
    List<Entity> entities;

    public Game()
    {
        renderer = new Renderer(TITLE, screenWidth, screenHeight, tileSize, targetFPS);
        renderer.loadAtlas("assets/CP437.png");

        int px = 5;
        int py = 5;

        player = new Player("Player", 100, px, py);
        camera = new Camera(0, 0, renderer.TileWidth, renderer.TileHeight);

        entities = new List<Entity>();
        entities.Add(player);

        loadMapFromImage("maps/map_01.png");
    }

    public void run()
    {
        FovSystem.computeFov(tiles, player.x, player.y, 15);

        while (!Raylib.WindowShouldClose())
        {
            handleInput();

            int px = player.x;
            int py = player.y;

            bool madeTurn = player.handleInput(tiles, entities);
            if (madeTurn)
            {
                turns++;

                Console.WriteLine("Made turn");

                foreach (Entity entity in entities) {
                    entity.update(tiles, entities, player);
                }

                FovSystem.computeFov(tiles, player.x, player.y, 15);
            }

            String playerModeText = player.playerMode switch
            {
                Player.PlayerMode.INTERACT_MODE => "INTERACT MODE",
                Player.PlayerMode.PEEK_MODE => "PEEK MODE",
                _ => "NORMAL MODE"
            };

            camera.centerOnPosition(player.x, player.y, screenWidth, screenHeight);

            Raylib.BeginDrawing();
            renderer.render(tiles, entities, camera);

            Raylib.DrawText($"turn #{turns}", 5, 5, 32, Color.White);
            Raylib.DrawText(
                playerModeText,
                screenWidth / 2 - Raylib.MeasureText(playerModeText, 32) / 2,
                5,
                32,
                Color.White
            );

            Raylib.EndDrawing();
        }
    }

    private void handleInput()
    {
        float wheel = Raylib.GetMouseWheelMove();

        if (wheel != 0)
        {
            renderer.setTileHeight(renderer.TileHeight + (wheel > 0 ? 1 : -1));

            camera.tileWidth = renderer.TileWidth;
            camera.tileHeight = renderer.TileHeight;
        }
    }

    // Relatively temporary
    public void loadMapFromImage(String filepath)
    {
        Image img = Raylib.LoadImage(filepath);
        tiles = new Tile[img.Height][];

        for (int y = 0; y < img.Height; y++)
        {
            tiles[y] = new Tile[img.Width];

            for (int x = 0; x < img.Width; x++)
            {
                Color pixel = Raylib.GetImageColor(img, x, y);

                tiles[y][x] = new FloorTile(false);

                if (pixel.Equals(Color.Black))     tiles[y][x] = new WallTile();
                if (pixel.Equals(Color.DarkGreen)) tiles[y][x] = new FloorTile(false);
                if (pixel.Equals(Color.Gray))      tiles[y][x] = new FloorTile(true);
                if (pixel.Equals(Color.Brown))     tiles[y][x] = new DoorTile(false);
                if (pixel.Equals(Color.Red))
                {
                    WindowTile windowTile = new WindowTile(false);
                    WindowTile? neighbor = null;

                    if (x > 0 && tiles[y][x - 1] is WindowTile leftWin) neighbor = leftWin;
                    else if (y > 0 && tiles[y - 1][x] is WindowTile topWin) neighbor = topWin;

                    if (neighbor != null)
                    {
                        neighbor.windowGroup ??= new List<WindowTile> { neighbor };
                        neighbor.windowGroup.Add(windowTile);
                        windowTile.windowGroup = neighbor.windowGroup;
                    }

                    tiles[y][x] = windowTile;
                }
                if (pixel.Equals(Color.Blue)) 
                {
                    player.x = x;
                    player.y = y;
                    tiles[y][x] = new FloorTile(false);
                }
                if (pixel.Equals(Color.Green)) 
                {
                    tiles[y][x] = new FloorTile(tiles[y][x - 1].isIndoors);
                    Zombie zombie = new Zombie("Zombie", 100, x, y);
                    entities.Add(zombie);
                }
            }
        }
    }
}
