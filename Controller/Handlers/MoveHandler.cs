using PROJ.Communication.Results;
using PROJ.Handlers.Enums;

namespace PROJ.Handlers;

public class MoveHandler : Handler
{
    private Board? _board;

    public MoveHandler(Board b)
    {
        _board = b;
    }

    public override HandleResult Handle(ConsoleKey key)
    {
        if (_board == null)
        {
            if(next != null)
                return next.Handle(key);
            else
            {
                return HandleResult.NotHandled;
            }
        }
            
        switch (key)
        {
            case ConsoleKey.W:
                var res = _board.MoveUp();
                GameOutput.Apply(res);
                return HandleResult.Handled;
            case ConsoleKey.A:
                GameOutput.Apply(_board.MoveLeft());
                return HandleResult.Handled;
            case ConsoleKey.S:
                GameOutput.Apply(_board.MoveDown());
                return HandleResult.Handled;
            case ConsoleKey.D:
                GameOutput.Apply(_board.MoveRight());
                return HandleResult.Handled;
            default:
                if(next != null)
                    return next.Handle(key);
                return HandleResult.NotHandled;
        }
    }
}
