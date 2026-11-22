using Raylib_cs;
using zombieroad.src.tiles;

namespace zombieroad.src.entities;

public class Player : Entity
{
    public enum PlayerMode
    {
        NORMAL_MODE,
        INTERACT_MODE,
        PEEK_MODE,
        PLAYER_MODE_COUNT
    }
    public PlayerMode playerMode { get; set; } = PlayerMode.NORMAL_MODE;

    private WindowTile? peekedWindow;

    public Player(String name, int health, int x, int y) : base(name, health, x, y)
    {
        glyph = 1;
        fgColor = Color.SkyBlue;
    }

    // Movement needs to be reworked later with custom delay and stuff
    public bool handleInput(Tile[][] tiles, List<Entity> entities)
    {
        KeyboardKey keyPressed = (KeyboardKey)Raylib.GetKeyPressed();

        // Turn skip
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.Period) ||
            keyPressed == KeyboardKey.Period)
        {
            endPeek();
            return true;
        }

        if (keyPressed == KeyboardKey.Escape)
        {
            if (!playerMode.Equals(PlayerMode.NORMAL_MODE))
            {
                playerMode = PlayerMode.NORMAL_MODE;
                return false;
            }
            else
            {
                // Pause menu or somethin
                return false;
            }
        }
        else if (keyPressed == KeyboardKey.Space)
        {
            playerMode = PlayerMode.INTERACT_MODE;
            return false;
        }
        else if (keyPressed == KeyboardKey.P)
        {
            playerMode = PlayerMode.PEEK_MODE;
            return false;
        }

        bool left  = Raylib.IsKeyDown(KeyboardKey.A);
        bool right = Raylib.IsKeyDown(KeyboardKey.D);
        bool up    = Raylib.IsKeyDown(KeyboardKey.W);
        bool down  = Raylib.IsKeyDown(KeyboardKey.S);

        bool moveRequested =
            Raylib.IsKeyPressed(KeyboardKey.A) || Raylib.IsKeyPressedRepeat(KeyboardKey.A) || 
            Raylib.IsKeyPressed(KeyboardKey.D) || Raylib.IsKeyPressedRepeat(KeyboardKey.D) || 
            Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressedRepeat(KeyboardKey.W) || 
            Raylib.IsKeyPressed(KeyboardKey.S) || Raylib.IsKeyPressedRepeat(KeyboardKey.S);

        int dx = (right ? 1 : 0) - (left ? 1 : 0);
        int dy = (down  ? 1 : 0) - (up   ? 1 : 0);

        if (dx == 0 && dy == 0) return false;
        if (!moveRequested) return false;

        switch (playerMode)
        {
            case PlayerMode.NORMAL_MODE:
                if (!validMove(tiles, entities, dx, dy))
                {
                    Tile t = tiles[y + dy][x + dx];
                    if (t is WindowTile wt) return false; // No bump interacting windows

                    bool interacted = tryInteract(tiles, entities, dx, dy);
                    if (interacted) endPeek();

                    return interacted;
                }

                endPeek();

                x += dx;
                y += dy;

                return true;

            case PlayerMode.INTERACT_MODE:
                playerMode = PlayerMode.NORMAL_MODE;

                bool done = tryInteract(tiles, entities, dx, dy);
                if (done) endPeek();

                return done;

            case PlayerMode.PEEK_MODE:
                playerMode = PlayerMode.NORMAL_MODE;

                return tryPeek(tiles, dx, dy);

            default:
                return false;
        }
    }

    private bool validMove(Tile[][] tiles, List<Entity> entities, int dx, int dy)
    {
        int tx = x + dx;
        int ty = y + dy;

        foreach (Entity entity in entities)
        {
            if (entity.x == tx && entity.y == ty)
            {
                return false;
            }
        }

        return !tiles[ty][tx].blocksMovement;
    }

    private bool tryPeek(Tile[][] tiles, int dx, int dy)
    {
        if (!tiles[y][x].isIndoors) return false;

        Tile target = tiles[y + dy][x + dx];

        if (target is not WindowTile window || window.blindsOpen) return false;

        endPeek();
        window.setPeeking(true);
        peekedWindow = window;

        return true;
    }

    private void endPeek()
    {
        peekedWindow?.setPeeking(false);
        peekedWindow = null;
    }

    private bool tryInteract(Tile[][] tiles, List<Entity> entities, int dx, int dy)
    {
        return tiles[y + dy][x + dx].interact(this, entities, tiles, x, y);
    }
}
