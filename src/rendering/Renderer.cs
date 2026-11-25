using Raylib_cs;
using System.Numerics;
using zombieroad.src.entities;
using zombieroad.src.tiles;

namespace zombieroad.src.rendering;
public class Renderer
{
    private const int AtlasColumns = 32;
    private const int AtlasRows = 8;

    private String title;
    private int screenWidth;
    private int screenHeight;
    private int tileSize;
    private int targetFPS = 240;

    public int TileSize => tileSize;

    private int cellWidth;
    private int cellHeight;

    private int tileWidth;
    private int tileHeight;

    public int TileWidth => tileWidth;
    public int TileHeight => tileHeight;

    private Texture2D atlas;

    // Sets up Renderer object and initializes Raylib
    public Renderer(String title, int screenWidth, int screenHeight, int tileSize, int targetFPS)
    {
        this.title = title;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.tileSize = tileSize;

        this.targetFPS = targetFPS;

        initRaylib();
        setTileHeight(tileHeight);
    }

    private void initRaylib()
    {
        Raylib.InitWindow(screenWidth, screenHeight, title);
        Raylib.SetTargetFPS(targetFPS);
        Raylib.SetExitKey(KeyboardKey.Null);
    }

    public void render(TileMap tileMap, List<Entity> entities, Camera camera)
    {
        Raylib.ClearBackground(Color.Black);
        flushBuffer(tileMap, camera);
        drawObjects(tileMap, entities, camera);
    }

    private void flushBuffer(TileMap tileMap, Camera camera)
    {
        Tile[][] floor = tileMap.floor;
        int cols = (screenWidth / camera.tileWidth) + 1;
        int rows = (screenHeight / camera.tileHeight) + 1;

        for (int sy = 0; sy < rows; sy++)
        {
            int worldY = camera.y + sy;
            if (worldY < 0 || worldY >= floor.Length) continue;

            for (int sx = 0; sx < cols; sx++)
            {
                int worldX = camera.x + sx;

                if (worldX < 0 || worldX >= floor[worldY].Length) continue;

                Tile tile = floor[worldY][worldX];

                if (!tile.isExplored) continue;
                if (tile.glyph == 0 || tile.glyph == 32) continue;

                int pixelX = sx * camera.tileWidth;
                int pixelY = sy * camera.tileHeight;

                Rectangle srcRec = new Rectangle(
                    tile.glyph % AtlasColumns * cellWidth,
                    tile.glyph / AtlasColumns * cellHeight,
                    cellWidth,
                    cellHeight
                );
                Color color = (tile.isExplored && !tile.isVisible) ? new Color(40, 40, 40, 255) : tile.fgColor;
                Rectangle destRec = new Rectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight);

                Raylib.DrawTexturePro(atlas, srcRec, destRec, Vector2.Zero, 0.0f, color);
            }
        } 
    }

    public void drawObjects(TileMap tileMap, List<Entity> entities, Camera camera)
    {
        Tile[][] floor = tileMap.floor;
        Tile?[][] objects = tileMap.objects;
        int cols = (screenWidth / camera.tileWidth) + 1;
        int rows = (screenHeight / camera.tileHeight) + 1;

        for (int sy = 0; sy < rows; sy++)
        {
            int worldY = camera.y + sy;
            if (worldY < 0 || worldY >= objects.Length) continue;

            for (int sx = 0; sx < cols; sx++)
            {
                int worldX = camera.x + sx;
                if (worldX < 0 || worldX >= objects[worldY].Length) continue;

                Tile? obj = objects[worldY][worldX];
                if (obj is null || !obj.isExplored) continue;

                int pixelX = sx * camera.tileWidth;
                int pixelY = sy * camera.tileHeight;

                Raylib.DrawRectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight, Color.Black);

                Rectangle srcRec = new Rectangle(
                    obj.glyph % AtlasColumns * cellWidth,
                    obj.glyph / AtlasColumns * cellHeight,
                    cellWidth,
                    cellHeight
                );
                Rectangle destRec = new Rectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight);
                Color color = obj.isVisible ? obj.fgColor : new Color(40, 40, 40, 255);

                Raylib.DrawTexturePro(atlas, srcRec, destRec, Vector2.Zero, 0.0f, color);
            }
        }

        foreach (Entity entity in entities) {
            int sx = entity.x - camera.x;
            int sy = entity.y - camera.y;

            if (sx < 0 || sx >= cols || sy < 0 || sy >= rows) continue;

            int pixelX = sx * camera.tileWidth;
            int pixelY = sy * camera.tileHeight;

            if (!floor[entity.y][entity.x].isVisible) continue;

            Raylib.DrawRectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight, Color.Black);

            Rectangle srcRect = new Rectangle(
                entity.glyph % AtlasColumns * cellWidth,
                entity.glyph / AtlasColumns * cellHeight,
                cellWidth,
                cellHeight
            );

            Rectangle destRec = new Rectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight);
            Raylib.DrawTexturePro(atlas, srcRect, destRec, Vector2.Zero, 0.0f, entity.fgColor);
        }
    }

    public void loadAtlas(String filePath)
    {
        Image img = Raylib.LoadImage(filePath);
        Raylib.ImageColorReplace(ref img, Color.Black, Color.Blank);

        atlas = Raylib.LoadTextureFromImage(img);
        Raylib.SetTextureFilter(atlas, TextureFilter.Point);
        Raylib.UnloadImage(img);

        cellWidth = atlas.Width / AtlasColumns;
        cellHeight = atlas.Height / AtlasRows;

        tileHeight = tileSize;
        tileWidth = (int)MathF.Round(tileHeight * (cellWidth / (float)cellHeight));
    }

    public void setTileHeight(int tileHeight)
    {
        tileHeight = Math.Clamp(tileHeight, 32, 64);
        this.tileHeight = tileHeight;
        tileSize = tileHeight;
        tileWidth = (int)MathF.Round(tileHeight * (cellWidth / (float)cellHeight));
    }
}
