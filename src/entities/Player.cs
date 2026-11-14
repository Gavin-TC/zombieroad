using Raylib_cs;
using zombieroad.src.tiles;

namespace zombieroad.src.entities;

public class Player : Entity
{
    private enum PlayerMode
    {
        NORMAL_MODE,
        INTERACT_MODE,
        PLAYER_MODE_COUNT
    }
    private PlayerMode playerMode = PlayerMode.NORMAL_MODE;

    public Player(String name, int health, int x, int y) : base(name, health, x, y)
    {
        glyph = 1;
        fgColor = Color.SkyBlue;
    }

    // Movement needs to be reworked later with custom delay and stuff
    public bool handleInput(Tile[][] tiles)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
        {
            if (playerMode.Equals(PlayerMode.INTERACT_MODE))
            {
                playerMode = PlayerMode.NORMAL_MODE;
            }
            else
            {
                // Pause menu or somethin
            }
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            playerMode = PlayerMode.INTERACT_MODE;
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

        switch (playerMode)
        {
            case PlayerMode.NORMAL_MODE:
                if (!moveRequested || (dx == 0 && dy == 0)) return false;

                if (!validMove(tiles, dx, dy))
                {
                    return tryInteract(tiles, dx, dy);
                }

                x += dx;
                y += dy;

                return true;

            case PlayerMode.INTERACT_MODE:
                bool interaction = tryInteract(tiles, dx, dy);

                if (interaction)
                {
                    playerMode = PlayerMode.NORMAL_MODE;
                    return true;
                }

                return false;

            default:
                return false;
        }
    }

    private bool validMove(Tile[][] tiles, int dx, int dy)
    {
        return !tiles[y + dy][x + dx].blocksMovement;
    }

    private bool tryInteract(Tile[][] tiles, int dx, int dy)
    {
        return tiles[y + dy][x + dx].interact(this);
    }
}
