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

    public int viewWidth { get; private set; }
    public int viewHeight { get; private set; }
    private int viewOffsetX;
    private int viewOffsetY;
    private int statusHeight;
    private int panelWidth;

    public int tileSize { get; private set; }
    private int targetFPS = 240;

    private int cellWidth;
    private int cellHeight;

    public int tileWidth { get; private set; }
    public int tileHeight { get; private set; }

    private Texture2D atlas;

    // Sets up Renderer object and initializes Raylib
    public Renderer(String title,
                    int screenWidth,
                    int screenHeight,
                    int panelWidth,
                    int statusHeight,
                    int tileSize,
                    int targetFPS)
    {
        this.title = title;
        this.screenWidth = screenWidth;
        this.screenHeight = screenHeight;
        this.panelWidth = panelWidth;
        this.statusHeight = statusHeight;

        viewWidth = screenWidth - panelWidth;
        viewHeight = screenHeight - statusHeight;
        viewOffsetX = 0;
        viewOffsetY = statusHeight;

        this.tileSize = tileSize;
        this.targetFPS = targetFPS;

        initRaylib();
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

        Raylib.BeginScissorMode(viewOffsetX, viewOffsetY, viewWidth, viewHeight);
        drawFloor(tileMap, camera);
        drawObjects(tileMap, entities, camera);
        Raylib.EndScissorMode();

        drawUI(tileMap, entities);
    }

    private void drawUI(TileMap tileMap, List<Entity> entities)
    {
        Raylib.DrawLine(0, statusHeight, screenWidth, statusHeight, Color.White);
        Raylib.DrawLine(viewWidth, statusHeight, viewWidth, screenHeight, Color.White);
    }

    private void drawFloor(TileMap tileMap, Camera camera)
    {
        int cols = (viewWidth / camera.tileWidth) + 1;
        int rows = (viewHeight / camera.tileHeight) + 1;

        for (int sy = 0; sy < rows; sy++)
        {
            int worldY = camera.y + sy;

            for (int sx = 0; sx < cols; sx++)
            {
                int worldX = camera.x + sx;

                if (!tileMap.inBounds(worldX, worldY)) continue;

                Tile tile = tileMap.getFloor(worldX, worldY);
                Tile? obj = tileMap.getObject(worldX, worldY);

                if (obj is not null && !obj.isExplored) continue;

                if (!tile.isExplored) continue;
                if (tile.glyph == 0 || tile.glyph == 32) continue;

                int pixelX = sx * camera.tileWidth + viewOffsetX;
                int pixelY = sy * camera.tileHeight + viewOffsetY;

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
        int cols = (viewWidth / camera.tileWidth) + 1;
        int rows = (viewHeight / camera.tileHeight) + 1;

        for (int sy = 0; sy < rows; sy++)
        {
            int worldY = camera.y + sy;

            for (int sx = 0; sx < cols; sx++)
            {
                int worldX = camera.x + sx;
                if (!tileMap.inBounds(worldX, worldY)) continue;

                Tile? obj = tileMap.getObject(worldX, worldY);
                if (obj is null || !obj.isExplored) continue;

                int pixelX = sx * camera.tileWidth + viewOffsetX;
                int pixelY = sy * camera.tileHeight + viewOffsetY;

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

            int pixelX = sx * camera.tileWidth + viewOffsetX;
            int pixelY = sy * camera.tileHeight + viewOffsetY;

            if (!tileMap.getFloor(entity.x, entity.y).isVisible) continue;

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

        setTileHeight(tileSize);

        tileWidth = (int)MathF.Round(tileHeight * (cellWidth / (float)cellHeight));
    }

    public void setTileHeight(int tileHeight)
    {
        tileHeight = Math.Clamp(tileHeight, 16, 64);
        this.tileHeight = tileHeight;
        tileSize = tileHeight;
        tileWidth = (int)MathF.Round(tileHeight * (cellWidth / (float)cellHeight));
    }
}
