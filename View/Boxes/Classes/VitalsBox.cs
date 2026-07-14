using PROJ;
using PROJ.Boxes;
using PROJ.GameConstansts;

public class VitalsBox : Box
{
    
    private Player _player;

    public VitalsBox(Player player)
    {
        _player = player;
    }
    
    public override void DisplayFrame()
    {
        Console.SetCursorPosition(GameConstants.VitalsBoxLeft, GameConstants.VitalsBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.VitalsBoxRight, GameConstants.VitalsBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.VitalsBoxLeft, GameConstants.VitalsBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.VitalsBoxRight, GameConstants.VitalsBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.VitalsBoxBottom - GameConstants.VitalsBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.VitalsBoxLeft, GameConstants.VitalsBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.VitalsBoxRight, GameConstants.VitalsBoxTop + i);
            Console.Write('│');
        }
        for (int i = 1; i < GameConstants.VitalsBoxRight - GameConstants.VitalsBoxLeft ; i++)
        {
            Console.SetCursorPosition(GameConstants.VitalsBoxLeft + i, GameConstants.VitalsBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.VitalsBoxLeft + i, GameConstants.VitalsBoxBottom);
            Console.Write('─');
        }

        string tmp = " Vitals ";
        Console.SetCursorPosition((GameConstants.VitalsBoxRight + GameConstants.VitalsBoxLeft) / 2 - tmp.Length/2 -  (GameConstants.VitalsBoxRight - GameConstants.VitalsBoxLeft) / 4, GameConstants.VitalsBoxTop);
        Console.Write(tmp);
    }

    public void DisplayVitals()
    {
        ClearInside();
        List<string> vitals = new List<string>();
        vitals.Add($"Health: {_player.Health}/100");
        vitals.Add($"Level: {Math.Floor(_player.Level)}");
        vitals.Add($"Strength: {_player.Strength}");
        vitals.Add($"Dexterity: {_player.Dexterity}");
        vitals.Add($"Luck: {_player.Luck}");
        vitals.Add($"Wisdom: {_player.Wisdom}");
        Console.SetCursorPosition(GameConstants.VitalsBoxWritingPointStartLeft,
            GameConstants.VitalsBoxWritingPointStartTop);
        for (int i = 0; i < vitals.Count; i++)
        {
            Console.Write(vitals[i]);
            Console.SetCursorPosition(GameConstants.VitalsBoxWritingPointStartLeft,
                GameConstants.VitalsBoxWritingPointStartTop + i + 1);
        }
        
    }

    private void ClearInside()
    {
        int width = GameConstants.VitalsBoxRight - GameConstants.VitalsBoxWritingPointStartLeft;
        string emptyLine = new string(' ', width);

        for (int y = GameConstants.VitalsBoxWritingPointStartTop; 
             y < GameConstants.VitalsBoxBottom; 
             y++)
        {
            Console.SetCursorPosition(GameConstants.VitalsBoxWritingPointStartLeft, y);
            Console.Write(emptyLine);
        }
    }
}