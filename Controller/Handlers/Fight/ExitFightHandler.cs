using PROJ.Communication.Results;
using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ.Fight.Handlers;

public class ExitFightHandler : Handler
{
    private FightMenu _menu;
    public ExitFightHandler(FightMenu menu)
    {
        _menu = menu;
    }
    public override HandleResult Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
        {
            FightExitResult result = _menu.ExitFight();
            GameOutput.Apply(result);
            return HandleResult.ExitGame;
        }
        if(next!=null)
            return next.Handle(key);
        return HandleResult.NotHandled;
    }
}
