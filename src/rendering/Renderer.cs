
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
    }

    private void initRaylib()
    {
        Raylib.InitWindow(screenWidth, screenHeight, title);
        Raylib.SetTargetFPS(targetFPS);
    }

    public void render(Tile[][] buffer, List<Entity> entities, Camera camera)
    {
        Raylib.ClearBackground(Color.Black);
        flushBuffer(buffer, camera);
        drawEntities(entities, camera);
    }

    private void flushBuffer(Tile[][] buffer, Camera camera)
    {
        int cols = (screenWidth / camera.tileWidth) + 1;
        int rows = (screenHeight / camera.tileHeight) + 1;

        for (int sy = 0; sy < rows; sy++)
        {
            int worldY = camera.y + sy;
            if (worldY < 0 || worldY >= buffer.Length) continue;

            for (int sx = 0; sx < cols; sx++)
            {
                int worldX = camera.x + sx;
                if (worldX < 0 || worldX >= buffer[worldY].Length) continue;

                Tile tile = buffer[worldY][worldX];
                int pixelX = sx * camera.tileWidth;
                int pixelY = sy * camera.tileHeight;

                // Draw background if visible
                if (tile.bgColor.A > 0)
                {
                    Raylib.DrawRectangle(pixelX, pixelY, tileWidth, tileHeight, tile.bgColor);
                }

                if (tile.glyph == 0 || tile.glyph == 32) continue;

                Rectangle srcRec = new Rectangle(
                    (tile.glyph % AtlasColumns) * cellWidth,
                    (tile.glyph / AtlasColumns) * cellHeight,
                    cellWidth,
                    cellHeight
                );

                Rectangle destRec = new Rectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight);
                Raylib.DrawTexturePro(atlas, srcRec, destRec, Vector2.Zero, 0.0f, tile.fgColor);
            }
        } 
    }

    public void drawEntities(List<Entity> entities, Camera camera)
    {
        foreach (Entity entity in entities) {
            int sx = entity.x - camera.x;
            int sy = entity.y - camera.y;

            int cols = (screenWidth / camera.tileWidth) + 1;
            int rows = (screenHeight / camera.tileHeight) + 1;

            if (sx < 0 || sx >= cols || sy < 0 || sy >= rows) continue;

            int pixelX = sx * camera.tileWidth;
            int pixelY = sy * camera.tileHeight;

            Raylib.DrawRectangle(pixelX, pixelY, camera.tileWidth, camera.tileHeight, Color.Black);

            Rectangle srcRect = new Rectangle(
                (entity.glyph % AtlasColumns) * cellWidth,
                (entity.glyph / AtlasColumns) * cellHeight,
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
        tileSize = Math.Clamp(tileHeight, 8, 128);
        this.tileHeight = tileSize;
        tileWidth = (int)MathF.Round(tileHeight * (cellWidth / (float)cellHeight));
    }
}
