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

    Renderer renderer;
    Camera camera;
    Player player;

    static TileMap tileMap;
    List<Entity> entities;

    int turns = 0;

    public Game()
    {
        renderer = new Renderer(TITLE, screenWidth, screenHeight, tileSize, targetFPS);
        renderer.loadAtlas("assets/CP437.png");

        player = new Player("Player", 100, 0, 0);
        camera = new Camera(0, 0, renderer.TileWidth, renderer.TileHeight);

        entities = [player];

        tileMap = new TileMap(screenWidth, screenHeight);
        tileMap.generateMapFromImage("maps/map_01.png", entities, player);
    }

    public void run()
    {
        FovSystem.computeFov(tileMap, player.x, player.y, 25);

        while (!Raylib.WindowShouldClose())
        {
            handleInput();

            int px = player.x;
            int py = player.y;

            bool madeTurn = player.handleInput(tileMap, entities);
            if (madeTurn)
            {
                turns++;

                foreach (Entity entity in entities) {
                    entity.update(tileMap, entities, player);
                }

                FovSystem.computeFov(tileMap, player.x, player.y, 15);
            }

            String playerModeText = player.playerMode switch
            {
                Player.PlayerMode.INTERACT_MODE => "INTERACT MODE",
                Player.PlayerMode.PEEK_MODE     => "PEEK MODE",
                _                               => "NORMAL MODE"
            };

            camera.centerOnPosition(player.x, player.y, screenWidth, screenHeight);

            Raylib.BeginDrawing();
            renderer.render(tileMap, entities, camera);

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
}
