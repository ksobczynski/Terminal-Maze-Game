using PROJ.Communication.Results;
using PROJ.Handlers.Enums;

namespace PROJ.Handlers;

public class SeekHandler : Handler
{
    private Board? _board;
    public SeekHandler(Board b)
    {
        _board = b;
    }
    public override HandleResult Handle(ConsoleKey key)
    {
        if (_board == null)
        {
            if (next != null)
            {
                return next.Handle(key);
            }
            else
            {
                return HandleResult.NotHandled;
            }
        }

        if (key == ConsoleKey.LeftArrow)
        {
            int curSeek = _board.TryDecreaseSeek();
            GameOutput.Apply(new SeekResult(curSeek, _board.CreatePlayerActionBoxResult(curSeek)));
            return HandleResult.Handled;
        }
        if (key == ConsoleKey.RightArrow)
        {
            int curSeek = _board.TryIncreaseSeek();
            GameOutput.Apply(new SeekResult(curSeek, _board.CreatePlayerActionBoxResult(curSeek)));
            return HandleResult.Handled;
        }
        
        _board.ResetSeek();
        if(next != null)
            return next.Handle(key);
        return HandleResult.NotHandled;
    }
}
