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
        ATTACK,
        STATES_COUNT
    };
    private State state = State.IDLE;

    private float wanderChance = 0.16f; // Chance to wander from idle
    private int defDetectionRange = 15;
    private int detectionRange = 15;

    private int attackDamage = 15;

    private int moveCost = 100;
    private int energy = 0;
    private int energyAcc = 75;

    private int ticks = 0;
    private int memoryTicks = 15;

    private int lastKnownPlayerX = -1;
    private int lastKnownPlayerY = -1;

    // Player position the zombie failed to path to, retried once the player moves
    private int blockedX = -1;
    private int blockedY = -1;

    public Zombie(String name, int health, int x, int y) : base(name, health, x, y)
    {
        glyph = 2;
        fgColor = Color.Green;
    }

    public override void update(TileMap map, List<Entity> entities, Player player)
    {
        energy += energyAcc;
        if (!(energy >= moveCost)) return;
        performAction(map, entities, player);
        energy -= moveCost;
    }

    private void performAction(TileMap map, List<Entity> entities, Player player)
    {
        Tile tile = map.getFloor(x, y);
        int playerDist = Math.Abs(x - player.x) + Math.Abs(y - player.y);
        int attackDist = Math.Max(Math.Abs(x - player.x), Math.Abs(y - player.y));
        bool targetBlocked = player.x == blockedX && player.y == blockedY;

        if (attackDist == 1)
        {
            state = State.ATTACK;
        }
        else if (playerDist <= detectionRange && tile.isVisible && !targetBlocked)
        {
            state = State.PURSUE;
            ticks = 0;
            lastKnownPlayerX = player.x;
            lastKnownPlayerY = player.y;
        }
        else if (state == State.PURSUE && ticks < memoryTicks)
        {
        }
        else if (state != State.WANDER)
        {
            state = State.IDLE;
        }

        switch (state)
        {
            case State.IDLE:
                float wanderRoll = (float)Random.Shared.NextDouble();
                float variance   = (float)Random.Shared.Next(-2, 3) / 100;

                if (wanderRoll <= wanderChance + variance) state = State.WANDER;

                ticks = 0;

                break;

            case State.WANDER: // Wanders to a surrounding tile
                (int dx, int dy) = Pathfinding.dirs[Random.Shared.Next(Pathfinding.dirs.Length)];
                int goalX = x + dx;
                int goalY = y + dy;

                (int x, int y)? wTarget = Pathfinding.FindNextStepGbfs(map, entities, x, y, goalX, goalY);

                if (wTarget == null) break;

                bool isOccupied = entities.Any(e => e.x == wTarget.Value.x && e.y == wTarget.Value.y) ||
                                  (player.x == wTarget.Value.x && player.y == wTarget.Value.y);

                if (!isOccupied) (x, y) = wTarget.Value;

                state = State.IDLE;
                break;

            case State.PURSUE:
                (int x, int y)? pTarget = Pathfinding.FindNextStepGbfs(
                    map,
                    entities,
                    x,
                    y,
                    lastKnownPlayerX,
                    lastKnownPlayerY
                );

                if (pTarget == null)
                {
                    blockedX = lastKnownPlayerX;
                    blockedY = lastKnownPlayerY;
                    state = State.WANDER;
                    break;
                }

                if ((player.x, player.y) == pTarget.Value)
                {
                    state = State.ATTACK;
                }
                else
                {
                    (x, y) = pTarget.Value;
                }

                break;

            case State.ATTACK:
                detectionRange = defDetectionRange;
                player.damage(attackDamage);
                break;
        }

        ticks++;
    }
}