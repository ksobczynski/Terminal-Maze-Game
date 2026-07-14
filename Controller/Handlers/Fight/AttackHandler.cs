using PROJ.Communication.Results;
using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ.Fight.Handlers;

public class AttackHandler : Handler
{
    
    private FightMenu _menu;
    public AttackHandler(FightMenu menu)
    {
        _menu = menu;
    }
    
    public override HandleResult Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.Enter)
        {
            FightTurnResult result = _menu.SimulateAttack();
            GameOutput.Apply(result);
            if (result.ExitFightMode)
                return HandleResult.ExitGame;
            return HandleResult.Handled;
        }
        if (next != null)
            return next.Handle(key);
        return HandleResult.NotHandled;
    }
}
