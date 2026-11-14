namespace zombieroad.src.rendering;

public class Camera
{
    public int x { get; set; }
    public int y { get; set; }

    public int tileWidth { get; set; }
    public int tileHeight { get; set; }

    public int zoom { get; set; } = 1;

    public Camera(int x, int y, int tileWidth, int tileHeight)
    {
        this.x = x;
        this.y = y;
        this.tileWidth = tileWidth;
        this.tileHeight = tileHeight;
    }

    public void centerOnPosition(int x, int y, int screenWidth, int screenHeight)
    {
        int cols = screenWidth / tileWidth;
        int rows = screenHeight / tileHeight;

        this.x = x - cols / 2;
        this.y = y - rows / 2;
    }
}
