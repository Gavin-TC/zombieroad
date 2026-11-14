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
    int tileSize = 48;

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
        while (!Raylib.WindowShouldClose())
        {
            handleInput();

            bool madeTurn = player.handleInput(tiles);
            if (madeTurn) turns++;

            camera.centerOnPosition(player.x, player.y, screenWidth, screenHeight);

            Raylib.BeginDrawing();
            renderer.render(tiles, entities, camera);
            Raylib.DrawText($"turn #{turns}", 2, 2, 32, Color.White);
            Raylib.EndDrawing();
        }
    }

    private void handleInput()
    {
        float wheel = Raylib.GetMouseWheelMove();

        if (wheel != 0)
        {
            renderer.setTileHeight(renderer.TileHeight + (wheel > 0 ? -1 : 1));

            camera.tileWidth = renderer.TileWidth;
            camera.tileHeight = renderer.TileHeight;
        }
    }

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

                if (pixel.Equals(Color.Black)) tiles[y][x] = new WallTile();
                if (pixel.Equals(Color.White)) tiles[y][x] = new FloorTile(false);
                if (pixel.Equals(Color.Brown)) tiles[y][x] = new DoorTile(false);

                if (pixel.Equals(new Color(0, 0, 255, 255))) 
                {
                    player.x = x;
                    player.y = y;
                    tiles[y][x] = new FloorTile(false);
                }
            }
        }
    }
}
