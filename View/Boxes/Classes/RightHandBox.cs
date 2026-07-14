using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class RightHandBox : Box
{
    private Player _player;
    public RightHandBox(Player player)
    {
        _player = player;
    }

    public override void DisplayFrame()
    {
        Console.SetCursorPosition(GameConstants.RightHandBoxLeft, GameConstants.RightHandBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.RightHandBoxRight, GameConstants.RightHandBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.RightHandBoxLeft, GameConstants.RightHandBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.RightHandBoxRight, GameConstants.RightHandBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.RightHandBoxBottom - GameConstants.RightHandBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.RightHandBoxLeft, GameConstants.RightHandBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.RightHandBoxRight, GameConstants.RightHandBoxTop + i);
            Console.Write('│');
        }

        for (int i = 1; i < GameConstants.RightHandBoxRight - GameConstants.RightHandBoxLeft; i++)
        {
            Console.SetCursorPosition(GameConstants.RightHandBoxLeft + i, GameConstants.RightHandBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.RightHandBoxLeft + i, GameConstants.RightHandBoxBottom);
            Console.Write('─');
        }

        string tmp = " Right ";
        Console.SetCursorPosition(
            (GameConstants.RightHandBoxRight + GameConstants.RightHandBoxLeft) / 2 - tmp.Length / 2 +
            (GameConstants.RightHandBoxRight - GameConstants.RightHandBoxLeft) / 4, GameConstants.RightHandBoxTop);
        Console.Write(tmp);
    }

    public void DisplayHand()
    {
        ClearHand();
        Console.SetCursorPosition(GameConstants.RightHandBoxWritingPointStartLeftName, GameConstants.RightHandoxWritingPointStartTopName);
        string s1, s2, s3;
        if (_player.RightHand == null)
        {
            s1 = "-------------";
            s2 = "--------------------------";
            s3 = "-----";
            
        }
        else
        {
            s1 = _player.RightHand.Name;
            s2 = "Icon " + _player.RightHand.Visual;
            s3 = "Space " + _player.RightHand.Space;
        }

        Console.Write(s1);
        
        Console.SetCursorPosition(GameConstants.RightHandBoxWritingPointStartLeftName, GameConstants.RightHandoxWritingPointStartTopDesc);

        Console.Write(s2);
        
        Console.SetCursorPosition(GameConstants.RightHandBoxWritingPointStartLeftName, GameConstants.RightHandoxWritingPointStartTopDesc + 3);
        
        Console.Write(s3);
        
        
    }
    
    private void ClearHand()
    {
        int innerWidth = GameConstants.RightHandBoxRight - GameConstants.RightHandBoxLeft - 1;
        int innerHeight = GameConstants.RightHandBoxBottom - GameConstants.RightHandBoxTop - 1;

        for (int i = 0; i < innerHeight; i++)
        {
            Console.SetCursorPosition(GameConstants.RightHandBoxLeft + 1, GameConstants.RightHandBoxTop + 1 + i);
            Console.Write(new string(' ', innerWidth));
        }
    }
}