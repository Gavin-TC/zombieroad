using zombieroad.src.tiles;

namespace zombieroad.src.rendering;

public static class FovSystem {
    // Cells marked visible by the previous computeFov call, cleared at the start of the next one
    private static readonly List<(int x, int y)> visibleCells = new();

    public static void computeFov(TileMap map, int px, int py, int radius)
    {
        if (map is null || map.height == 0) return;

        foreach (var (x, y) in visibleCells)
        {
            map.getFloor(x, y).isVisible = false;

            Tile? obj = map.getObject(x, y);
            if (obj is not null) obj.isVisible = false;
        }
        visibleCells.Clear();

        int minX = Math.Max(0, px - radius);
        int maxX = Math.Min(map.width - 1, px + radius);
        int minY = Math.Max(0, py - radius);
        int maxY = Math.Min(map.height - 1, py + radius);
        int radiusSq = radius * radius + radius;

        for (int x = minX; x <= maxX; x++)
        {
            castRay(map, px, py, x, minY, radiusSq);
            castRay(map, px, py, x, maxY, radiusSq);
        }

        for (int y = minY + 1; y < maxY; y++)
        {
            castRay(map, px, py, minX, y, radiusSq);
            castRay(map, px, py, maxX, y, radiusSq);
        }

        markSeen(map, px, py);
    }

    private static void markSeen(TileMap map, int x, int y)
    {
        Tile floorTile = map.getFloor(x, y);
        if (!floorTile.isVisible) visibleCells.Add((x, y));

        floorTile.isVisible = true;
        floorTile.isExplored = true;

        Tile? obj = map.getObject(x, y);
        if (obj is not null)
        {
            obj.isVisible = true;
            obj.isExplored = true;
        }
    }

    // Bresenham's line rays
    private static void castRay(TileMap map, int x0, int y0, int x1, int y1, int radiusSq)
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
