using Raylib_cs;
using System.Text;
using zombieroad.src.systems;
using zombieroad.src.tiles;

namespace zombieroad.src.entities;

public class Zombie : Entity
{
    enum State
    {
        IDLE,
        WANDER,
        PURSUE,
        ATTACKING,
        STATES_COUNT
    };
    private State state = State.IDLE;

    private Random rand;

    private int wanderX = -1;
    private int wanderY = -1;
    private int wanderRange = 7;
    private float wanderChance = 0.15f; // Chance to wander from idle

    private int detectionRange = 15;

    public Zombie(String name, int health, int x, int y) : base(name, health, x, y)
    {
        glyph = 2;
        fgColor = Color.Green;
        rand = new Random();
    }

    public override void update(Tile[][] tiles, Player player, List<Entity> entities)
    {
        int width = tiles[0].Length;
        int height = tiles.Length;

        int playerDist = Math.Abs(x - player.x) + Math.Abs(y - player.y);

        if (playerDist <= detectionRange)
        {
            if (state != State.PURSUE && tiles[y][x].isVisible == true)
                state = State.PURSUE;
        }

        switch (state)
        {
            case State.IDLE:
                float wanderRoll = (float)rand.NextDouble();

                if (wanderRoll <= wanderChance) state = State.WANDER;

                break;

            case State.WANDER:
                if (wanderX == -1 && wanderY == -1)
                {
                    while (!Pathfinding.inBounds(wanderX, wanderY, width, height))
                    {
                        wanderX = x + rand.Next(-wanderRange, wanderRange + 1);
                        wanderY = y + rand.Next(-wanderRange, wanderRange + 1);
                    }
                }

                if (x == wanderX && y == wanderY)
                {
                    wanderX = -1;
                    wanderY = -1;
                    state = State.IDLE;
                    return;
                }

                // Move towards wander pos
                (int x, int y)? target = Pathfinding.FindNextStepGbfs(
                    tiles,
                    entities,
                    x,
                    y,
                    wanderX,
                    wanderY
                );

                if (target == null)
                {
                    wanderX = -1;
                    wanderY = -1;
                    state = State.IDLE;
                    return;
                }

                (x, y) = target.Value;

                break;

            case State.PURSUE:

                break;

            case State.ATTACKING:
                break;
        }
    }
}