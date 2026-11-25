using zombieroad.src.tiles;

namespace zombieroad.src.rendering;

public static class FovSystem {
    public static void computeFov(TileMap map, int px, int py, int radius)
    {
        if (map is null || map.height == 0) return;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                map.floor[y][x].isVisible = false;

                Tile? obj = map.objects[y][x];
                if (obj is not null) obj.isVisible = false;
            }
        }

        int minX = Math.Max(0, px - radius);
        int maxX = Math.Min(map.width - 1, px + radius);
        int minY = Math.Max(0, py - radius);
        int maxY = Math.Min(map.height - 1, py + radius);
        int radiusSq = radius * radius + radius;

        for (int x = minX; x <= maxX; x++)
        {
            CastRay(map, px, py, x, minY, radiusSq);
            CastRay(map, px, py, x, maxY, radiusSq);
        }

        for (int y = minY + 1; y < maxY; y++)
        {
            CastRay(map, px, py, minX, y, radiusSq);
            CastRay(map, px, py, maxX, y, radiusSq);
        }

        markSeen(map, px, py);
    }

    private static void markSeen(TileMap map, int x, int y)
    {
        Tile floorTile = map.floor[y][x];
        floorTile.isVisible = true;
        floorTile.isExplored = true;

        Tile? obj = map.objects[y][x];
        if (obj is not null)
        {
            obj.isVisible = true;
            obj.isExplored = true;
        }
    }

    // Bresenham's line rays
    private static void CastRay(TileMap map, int x0, int y0, int x1, int y1, int radiusSq)
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
            if (!map.inBounds(curX, curY))
                break;

            int ddx = curX - x0;
            int ddy = curY - y0;
            if (ddx * ddx + ddy * ddy > radiusSq) break;

            markSeen(map, curX, curY);

            // If this tile blocks vision, stop the ray
            if (map.blocksVision(curX, curY) && (curX != x0 || curY != y0)) break;

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
