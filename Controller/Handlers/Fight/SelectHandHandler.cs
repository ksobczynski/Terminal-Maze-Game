using PROJ.Communication.Results;
using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ.Fight.Handlers;

public class SelectHandHandler : Handler
{
    private FightMenu _menu;

    public SelectHandHandler(FightMenu menu)
    {
        _menu = menu;
    }

    public override HandleResult Handle(ConsoleKey key)
    {
        if (key == ConsoleKey.L)
        {
            FightHandSelectionResult result = _menu.SetHand('L');
            GameOutput.Apply(result);
            return HandleResult.Handled;
        }

        if (key == ConsoleKey.R)
        {
            FightHandSelectionResult result = _menu.SetHand('R');
            GameOutput.Apply(result);
            return HandleResult.Handled;
        }
        if (next != null)
            return next.Handle(key);
        return HandleResult.NotHandled;
    }
}
