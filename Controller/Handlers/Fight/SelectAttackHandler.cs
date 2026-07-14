using PROJ.Communication.Results;
using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ.Fight.Handlers;

public class SelectAttackHandler : Handler
{
    private FightMenu _menu;
    public SelectAttackHandler(FightMenu menu)
    {
        _menu = menu;
    }

    public override HandleResult Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.D1)
        {
            FightAttackSelectionResult result = _menu.SetAttack(1);
            GameOutput.Apply(result);
            return HandleResult.Handled;
        }
        if (key == ConsoleKey.D2)
        {
            FightAttackSelectionResult result = _menu.SetAttack(2);
            GameOutput.Apply(result);
            return HandleResult.Handled;
        }
        if (key == ConsoleKey.D3)
        {
            FightAttackSelectionResult result = _menu.SetAttack(3);
            GameOutput.Apply(result);
            return HandleResult.Handled;
        }

        if (next != null)
            return next.Handle(key);
        return HandleResult.NotHandled;
    }
}
