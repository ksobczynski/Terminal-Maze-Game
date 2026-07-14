using PROJ.Communication.Results;
using PROJ.Handlers.Enums;

namespace PROJ.Handlers;

public class FightHandler: Handler
{
    private Board _board;

    public FightHandler(Board board)
    {
        _board = board;
    }
    public override HandleResult Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.Enter)
        {
            FightStartResult startResult = _board.FightNearestEnemy();
            //TODO cos w stylu: if local to to co nizej a if online to biore paczke i wysyłam do odpowiedniego klienta
            GameOutput.Apply(startResult);
            if (startResult.IsSuccess)
            {
                FightLoopEndResult endResult = _board.RunCurrentFight();
                GameOutput.Apply(endResult);
                if (endResult.PlayerDied)
                    return HandleResult.ExitGame;
            }
            return HandleResult.Handled;
        }
        else
        {
            if(next != null)
                return next.Handle(key);
            return HandleResult.NotHandled;
        }
    }
}
