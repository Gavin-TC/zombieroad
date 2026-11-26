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
    public bool handleInput(TileMap tileMap, List<Entity> entities)
    {
        KeyboardKey keyPressed = (KeyboardKey)Raylib.GetKeyPressed();

        // Turn skip
        if (Raylib.IsKeyPressedRepeat(KeyboardKey.Period) ||
            keyPressed == KeyboardKey.Period)
        {
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

        if (dx == 0 && dy == 0 || !moveRequested) return false;

        switch (playerMode)
        {
            case PlayerMode.NORMAL_MODE:
                if (!validMove(tileMap, entities, dx, dy))
                {
                    int tx = x + dx;
                    int ty = y + dy;
                    if (!tileMap.inBounds(tx, ty)) return false;
                    if (tileMap.getObject(tx, ty) is WindowTile) return false; // No bump interacting windows

                    bool interacted = tryInteract(tileMap, entities, dx, dy);
                    if (interacted) endPeek();

                    return interacted;
                }

                endPeek();

                x += dx;
                y += dy;

                return true;

            case PlayerMode.INTERACT_MODE:
                playerMode = PlayerMode.NORMAL_MODE;

                bool done = tryInteract(tileMap, entities, dx, dy);
                if (done) endPeek();

                return done;

            case PlayerMode.PEEK_MODE:
                playerMode = PlayerMode.NORMAL_MODE;
                return tryPeek(tileMap, dx, dy);

            default:
                return false;
        }
    }

    private bool validMove(TileMap tileMap, List<Entity> entities, int dx, int dy)
    {
        int tx = x + dx;
        int ty = y + dy;

        if (!tileMap.inBounds(tx, ty)) return false;

        foreach (Entity entity in entities)
        {
            if (entity.x == tx && entity.y == ty)
            {
                return false;
            }
        }

        return !tileMap.blocksMovement(tx, ty);
    }

    private bool tryPeek(TileMap tileMap, int dx, int dy)
    {
        if (!tileMap.getObject(x, y)?.isIndoors ?? true) return false;
        if (!tileMap.inBounds(x + dx, y + dy)) return false;

        Tile? target = tileMap.getObject(x + dx, y + dy);

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

    private bool tryInteract(TileMap tileMap, List<Entity> entities, int dx, int dy)
    {
        if (!tileMap.inBounds(x + dx, y + dy)) return false;

        Tile? targetTile = tileMap.getObject(x + dx, y + dy);
        if (targetTile is null) return false;

        return targetTile.interact(tileMap, entities, this, x + dx, y + dy);
    }

    public void setX(int x)
    {
        this.x = x;
    }

    public void setY(int y)
    {
        this.y = y;
    }
}
