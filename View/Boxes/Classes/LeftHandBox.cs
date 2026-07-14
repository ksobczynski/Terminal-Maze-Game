using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class LeftHandBox : Box
{
    private Player _player;
    public LeftHandBox(Player player)
    {
        _player = player;
    }

    public override void DisplayFrame()
    {
        Console.SetCursorPosition(GameConstants.LeftHandBoxLeft, GameConstants.LeftHandBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.LeftHandBoxRight, GameConstants.LeftHandBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.LeftHandBoxLeft, GameConstants.LeftHandBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.LeftHandBoxRight, GameConstants.LeftHandBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.LeftHandBoxBottom - GameConstants.LeftHandBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.LeftHandBoxLeft, GameConstants.LeftHandBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.LeftHandBoxRight, GameConstants.LeftHandBoxTop + i);
            Console.Write('│');
        }

        for (int i = 1; i < GameConstants.LeftHandBoxRight - GameConstants.LeftHandBoxLeft; i++)
        {
            Console.SetCursorPosition(GameConstants.LeftHandBoxLeft + i, GameConstants.LeftHandBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.LeftHandBoxLeft + i, GameConstants.LeftHandBoxBottom);
            Console.Write('─');
        }

        string tmp = " Left ";
        Console.SetCursorPosition(
            (GameConstants.LeftHandBoxRight + GameConstants.LeftHandBoxLeft) / 2 - tmp.Length / 2 -
            (GameConstants.LeftHandBoxRight - GameConstants.LeftHandBoxLeft) / 4, GameConstants.LeftHandBoxTop);
        Console.Write(tmp);
    }

    public void DisplayHand()
    {
        ClearHand();
        Console.SetCursorPosition(GameConstants.LeftHandBoxWritingPointStartLeftName, GameConstants.LeftHandoxWritingPointStartTopName);
        string s1, s2, s3;
        if (_player.LeftHand == null)
        {
            s1 = "-------------";
            s2 = "--------------------------";
            s3 = "-----";
            
        }
        else
        {
            s1 = _player.LeftHand.Name;
            s2 = "Icon " + _player.LeftHand.Visual;
            s3 = "Space " + _player.LeftHand.Space;
        }

        Console.Write(s1);
        
        Console.SetCursorPosition(GameConstants.LeftHandBoxWritingPointStartLeftName, GameConstants.LeftHandoxWritingPointStartTopDesc);

        Console.Write(s2);
        
        Console.SetCursorPosition(GameConstants.LeftHandBoxWritingPointStartLeftName, GameConstants.LeftHandoxWritingPointStartTopDesc + 3);
        
        Console.Write(s3);
        
        
    }

    private void ClearHand()
    {
        int innerWidth = GameConstants.LeftHandBoxRight - GameConstants.LeftHandBoxLeft - 1;
        int innerHeight = GameConstants.LeftHandBoxBottom - GameConstants.LeftHandBoxTop - 1;

        for (int i = 0; i < innerHeight; i++)
        {
            Console.SetCursorPosition(GameConstants.LeftHandBoxLeft + 1, GameConstants.LeftHandBoxTop + 1 + i);
            Console.Write(new string(' ', innerWidth));
        }
    }
}