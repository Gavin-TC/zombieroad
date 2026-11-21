using zombieroad.src.tiles;

namespace zombieroad.src.rendering;

public static class FovSystem {
    public static void computeFov(Tile[][] tiles, int px, int py, int radius)
    {
        for (int y = 0; y < tiles.Length; y++)
        {
            for (int x = 0; x < tiles[y].Length; x++)
            {
                tiles[y][x].isVisible = false;
            }
        }

        int minX = Math.Max(0, px - radius);
        int maxX = Math.Min(tiles[0].Length - 1, px + radius);
        int minY = Math.Max(0, py - radius);
        int maxY = Math.Min(tiles.Length - 1, py + radius);

        for (int x = minX; x <= maxX; x++)
        {
            CastRay(tiles, px, py, x, minY);
            CastRay(tiles, px, py, x, maxY);
        }

        for (int y = minY + 1; y < maxY; y++)
        {
            CastRay(tiles, px, py, minX, y);
            CastRay(tiles, px, py, maxX, y);
        }

        tiles[py][px].isVisible = true;
        tiles[py][px].isExplored = true;
    }

    // Bresenham's line rays
    private static void CastRay(Tile[][] tiles, int x0, int y0, int x1, int y1)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        int curX = x0;
        int curY = y0;

        while (true)
        {
            if (curY < 0 || curY >= tiles.Length || curX < 0 || curX >= tiles[curY].Length)
                break;

            Tile tile = tiles[curY][curX];

            tile.isVisible = true;
            tile.isExplored = true;

            // If this tile blocks vision, stop the ray
            if (tile.blocksVision && (curX != x0 || curY != y0)) break;

            if (curX == x1 && curY == y1) break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                curX += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                curY += sy;
            }
        }
    }
}