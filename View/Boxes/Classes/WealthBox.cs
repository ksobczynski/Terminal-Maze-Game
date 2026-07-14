using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class WealthBox : Box
{
    private Player _player;

    public WealthBox(Player player)
    {
        _player = player;
    }

    public override void DisplayFrame()
    {
        Console.SetCursorPosition(GameConstants.WealthBoxLeft, GameConstants.WealthBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.WealthBoxRight, GameConstants.WealthBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.WealthBoxLeft, GameConstants.WealthBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.WealthBoxRight, GameConstants.WealthBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.WealthBoxBottom - GameConstants.WealthBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.WealthBoxLeft, GameConstants.WealthBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.WealthBoxRight, GameConstants.WealthBoxTop + i);
            Console.Write('│');
        }
        for (int i = 1; i < GameConstants.WealthBoxRight - GameConstants.WealthBoxLeft ; i++)
        {
            Console.SetCursorPosition(GameConstants.WealthBoxLeft + i, GameConstants.WealthBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.WealthBoxLeft + i, GameConstants.WealthBoxBottom);
            Console.Write('─');
        }

        string tmp = " Wealth ";
        Console.SetCursorPosition((GameConstants.WealthBoxRight + GameConstants.WealthBoxLeft) / 2 - tmp.Length/2 -  (GameConstants.WealthBoxRight - GameConstants.WealthBoxLeft) / 4, GameConstants.WealthBoxTop);
        Console.Write(tmp);
    }
    
    public void DisplayGoods()
    {
        List<string> goods = new List<string>();
        goods.Add($"Gold: {_player.Gold}");
        goods.Add($"Coins: {_player.Coins}");
        Console.SetCursorPosition(GameConstants.WealthBoxWritingPointStartLeft,
            GameConstants.WealthBoxWritingPointStartTop);
        for (int i = 0; i < goods.Count; i++)
        {
            Console.Write(goods[i]);
            Console.SetCursorPosition(GameConstants.WealthBoxWritingPointStartLeft,
                GameConstants.WealthBoxWritingPointStartTop + i + 1);
        }
        
    }
}