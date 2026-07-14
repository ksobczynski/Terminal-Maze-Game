using PROJ.Boxes;
using PROJ.Communication.Results;
using PROJ.GameConstansts;
using PROJ.Tools.Classes;
using PROJ.Tools.Classes.Items;

namespace PROJ;

public class EquipmentBox : Box
{
    private Player _player;

    public EquipmentBox(Player player)
    {
        _player = player;
    }
    
    public override void DisplayFrame()
    {
        Console.SetCursorPosition(GameConstants.EqBoxLeft, GameConstants.EqBoxTop);
        Console.Write('┌');
        Console.SetCursorPosition(GameConstants.EqBoxRight, GameConstants.EqBoxTop);
        Console.Write('┐');
        Console.SetCursorPosition(GameConstants.EqBoxLeft, GameConstants.EqBoxBottom);
        Console.Write('└');
        Console.SetCursorPosition(GameConstants.EqBoxRight, GameConstants.EqBoxBottom);
        Console.Write('┘');

        for (int i = 1; i < GameConstants.EqBoxBottom - GameConstants.EqBoxTop; i++)
        {
            Console.SetCursorPosition(GameConstants.EqBoxLeft, GameConstants.EqBoxTop + i);
            Console.Write('│');
            Console.SetCursorPosition(GameConstants.EqBoxRight, GameConstants.EqBoxTop + i);
            Console.Write('│');
        }
        for (int i = 1; i < GameConstants.EqBoxRight - GameConstants.EqBoxLeft ; i++)
        {
            Console.SetCursorPosition(GameConstants.EqBoxLeft + i, GameConstants.EqBoxTop);
            Console.Write('─');
            Console.SetCursorPosition(GameConstants.EqBoxLeft + i, GameConstants.EqBoxBottom);
            Console.Write('─');
        }

        string tmp = " Equipment ";
        Console.SetCursorPosition((GameConstants.EqBoxRight + GameConstants.EqBoxLeft) / 2 - tmp.Length/2 -  (GameConstants.EqBoxRight - GameConstants.EqBoxLeft) / 4, GameConstants.EqBoxTop);
        Console.Write(tmp);
    }

    public void DisplayItemsLeavePointer()
    {
        ClearInsideLeavePointer();
        Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop);
        for (int i = 0; i < GameConstants.BackpackCapacity; i++)
        {
            if (_player.PlayerBackpack.IsItemAt(i))
            {
                Console.Write($"{i + 1}. {_player.PlayerBackpack.TryGetItem(i)?.Name} ({_player.PlayerBackpack.TryGetItem(i)?.Space})");
            }
            else
            {
                Console.Write($"{i + 1}. --------------");
            }
            Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop + i + 1);
        }
    }
    public void DisplayItems()
    {
        ClearInside();
        Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop);
        for (int i = 0; i < GameConstants.BackpackCapacity; i++)
        {
            if (_player.PlayerBackpack.IsItemAt(i))
            {
                Console.Write($"{i + 1}. {_player.PlayerBackpack.TryGetItem(i)?.Name} ({_player.PlayerBackpack.TryGetItem(i)?.Space})");
            }
            else
            {
                Console.Write($"{i + 1}. --------------");
            }
            Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop + i + 1);
        }
    }

    private void ClearInsideLeavePointer()
    {
        Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop);
        string temp = new string(' ', GameConstants.EqPointer - GameConstants.EqBoxLeft - 3);
        for (int i = 1; i < GameConstants.EqBoxBottom - GameConstants.EqBoxWritingPointStartTop; i++)
        {
            Console.Write(temp);
            Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop + i);
        }
    }
    private void ClearInside()
    {
        Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop);
        string temp = new string(' ', GameConstants.EqBoxRight - GameConstants.EqBoxLeft - 3);
        for (int i = 1; i < GameConstants.EqBoxBottom - GameConstants.EqBoxWritingPointStartTop; i++)
        {
            Console.Write(temp);
            Console.SetCursorPosition(GameConstants.EqBoxWritingPointStartLeft, GameConstants.EqBoxWritingPointStartTop + i);
        }
    }

    public void PointerInit()
    {
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop);
        Console.Write('<');
    }
    public void PointerUp(int from)
    {
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop + from);
        Console.Write(' ');
        
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop - 1 + from);
        Console.Write('<');
    }

    public void PointerDown(int from)
    {
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop + from);
        Console.Write(' ');
        
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop + 1 + from);
        Console.Write('<');
        
    }
    public void ClearPointer(int pos)
    {
        Console.SetCursorPosition(GameConstants.EqPointer, GameConstants.EqBoxWritingPointStartTop + pos);
        Console.Write(' ');
    }

    public void UpdatePointer(BackpackModeSwitchResult modeSwitch)
    {
        if(modeSwitch.InBp)
            PointerInit();
        else
            ClearPointer(modeSwitch.BackpackIdx);
    }

    public void RenderPointer(BackpackPointerMoveResult modeSwitch)
    {
        if(modeSwitch.MoveUp)
            PointerUp(modeSwitch.BackpackIdx);
        else
            PointerDown(modeSwitch.BackpackIdx);
    }
}