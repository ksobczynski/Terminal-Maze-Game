using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class PlayerMovesBox : Box
{
    private int _lastCursorPosTop = GameConstants.PlayerMovesBoxWritingPointStartTop;
    public override void DisplayFrame()
    {
        // ┌ ┐ └ ┘ ─ │
        Console.SetCursorPosition(GameConstants.PlayerMovesBoxLeft, GameConstants.PlayerMovesBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.PlayerMovesBoxRight, GameConstants.PlayerMovesBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.PlayerMovesBoxLeft, GameConstants.PlayerMovesBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.PlayerMovesBoxRight, GameConstants.PlayerMovesBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.PlayerMovesBoxBottom - GameConstants.PlayerMovesBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.PlayerMovesBoxLeft, GameConstants.PlayerMovesBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.PlayerMovesBoxRight, GameConstants.PlayerMovesBoxTop + i);
            Console.Write('│');
        }
        for (int i = 1; i < GameConstants.PlayerMovesBoxRight - GameConstants.PlayerMovesBoxLeft ; i++)
        {
            Console.SetCursorPosition(GameConstants.PlayerMovesBoxLeft + i, GameConstants.PlayerMovesBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.PlayerMovesBoxLeft + i, GameConstants.PlayerMovesBoxBottom);
            Console.Write('─');
        }

        string tmp = " Player Moves ";
        Console.SetCursorPosition((GameConstants.PlayerMovesBoxRight + GameConstants.PlayerMovesBoxLeft) / 2 - tmp.Length/2, GameConstants.PlayerMovesBoxTop);
        Console.Write(tmp);
    }

    public void AddMove(string move)
    {
        Console.SetCursorPosition(GameConstants.PlayerMovesBoxWritingPointStartLeft, _lastCursorPosTop++);
        Console.Write(move);
    }
}