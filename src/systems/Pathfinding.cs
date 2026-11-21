using zombieroad.src.entities;
using zombieroad.src.tiles;

namespace zombieroad.src.systems;

public static class Pathfinding
{
    private static readonly (int dx, int dy)[] dirs =
    {
        (0, -1), (0, 1), (-1, 0), (1, 0),   // Cardinal
        (-1, -1), (1, -1), (-1, 1), (1, 1)  // Diagonal
    };

    public static (int x, int y)? FindNextStepGbfs(
        Tile[][] tiles,
        List<Entity> entities,
        int startX,
        int startY,
        int goalX,
        int goalY,
        int maxNodes = 256)
    {
        if (startX == goalX && startY == goalY) return null;

        int width = tiles[0].Length;
        int height = tiles.Length;

        if (!inBounds(goalX, goalY, width, height)) return null;

        var frontier = new PriorityQueue<(int x, int y), int>();
        var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();
        var visited = new HashSet<(int x, int y)>();

        frontier.Enqueue((startX, startY), Heuristic(startX, startY, goalX, goalY));

        int expanded = 0;

        while (frontier.Count > 0 && expanded < maxNodes)
        {
            var current = frontier.Dequeue();

            if (!visited.Add(current)) continue;

            expanded++;

            if (current.x == goalX && current.y == goalY)
                return reconstructFirstStep(cameFrom, startX, startY, current);

            foreach (var (dx, dy) in dirs)
            {
                int nx = current.x + dx;
                int ny = current.y + dy;
                var next = (nx, ny);

                if (visited.Contains(next)) continue;

                if (!isWalkable(tiles, entities, nx, ny, goalX, goalY)) continue;

                if (!cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                }

                frontier.Enqueue(next, Heuristic(nx, ny, goalX, goalY));
            }
        }

        return null;
    }

    private static bool isWalkable(
        Tile[][] tiles,
        List<Entity> entities,
        int x,
        int y,
        int goalX,
        int goalY)
    {
        if (!inBounds(x, y, tiles[0].Length, tiles.Length)) return false;

        if (tiles[y][x].blocksMovement) return false;

        if (x != goalX || y != goalY)
        {
            foreach(Entity entity in entities)
            {
                if (entity.x == x && entity.y == y) return false;
            }
        }

        return true;
    }

    private static (int x, int y)? reconstructFirstStep(
        Dictionary<(int x, int y), (int x, int y)> cameFrom,
        int startX,
        int startY,
        (int x, int y) goal
    )
    {
        var step = goal;

        while (cameFrom.TryGetValue(step, out var prev))
        {
            if (prev.x == startX && prev.y == startY) return step;

            step = prev;
        }

        return null;
    }

    private static int Heuristic(int x0, int y0, int x1, int y1)
    {
        return Math.Max(Math.Abs(x0 - x1), Math.Abs(y0 - y1));
    }

    public static bool inBounds(int x, int y, int width, int height)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
