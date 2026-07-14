using PROJ.Handlers.Enums;

namespace PROJ.Handlers;

public class BackpackModeHandler : Handler
{
    private Player? _player;

    public BackpackModeHandler(Player p)
    {
        _player = p;
    }
    public override HandleResult Handle(ConsoleKey key)
    {
        if (_player == null)
        {
            if (next != null)
                return next.Handle(key);
            else
            {
                return HandleResult.NotHandled;
            }
        }
        switch (key)
        {
            case ConsoleKey.B:
            {
                var result = _player.SwitchBackpackMode();
                GameOutput.Apply(result);
                return HandleResult.Handled;
            }
            case ConsoleKey.UpArrow:
                if (_player.IsInBackpack)
                {
                    var result = _player.TryDecrementBackpackIdx();
                    if (result != null)
                        GameOutput.Apply(result);
                    
                    return HandleResult.Handled;
                }
                if(next != null)
                {
                    return next.Handle(key);
                }
                break;
            case ConsoleKey.DownArrow:
                if (_player.IsInBackpack)
                {
                    var res = _player.TryIncrementBackpackIdx();
                    if (res != null)
                        GameOutput.Apply(res);
                        
                    return HandleResult.Handled;
                }
                if(next != null)
                {
                    return next.Handle(key);
                }
                break;
            case ConsoleKey.R:
                if (_player.IsInBackpack)
                {
                    var res = _player.TrySwap('r');
                    if (res != null)
                        GameOutput.Apply(res);
                    return HandleResult.Handled;
                }
                if(next != null)
                {
                    return next.Handle(key);
                }
                break;
            case ConsoleKey.L:
                if (_player.IsInBackpack)
                {
                    var res = _player.TrySwap('l');
                    if (res != null)
                        GameOutput.Apply(res);
                    return HandleResult.Handled;
                }
                if(next != null)
                {
                    return next.Handle(key);
                }
                break;
            case ConsoleKey.Q:
                if (_player.IsInBackpack)
                {
                    var res = _player.BackpackDrop();
                    if (res != null)
                        GameOutput.Apply(res);
                        
                    return HandleResult.Handled;
                }
                break;
            default:
                if(next != null)
                    return next.Handle(key);
                break;
        }

        return HandleResult.NotHandled;

    }
}
