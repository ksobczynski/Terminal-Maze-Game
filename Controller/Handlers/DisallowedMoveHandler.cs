using PROJ.Handlers.Enums;
using PROJ.Handlers.Interfaces;
using PROJ.Logging.Classes;

namespace PROJ.Handlers;

public class DisallowedMoveHandler : Handler
{
    private AboveActionErrorSpace _errSpace;

    public DisallowedMoveHandler(AboveActionErrorSpace errorSpace)
    {
        _errSpace = errorSpace;
    }
    
    public override HandleResult Handle(ConsoleKey key)
    {
        _errSpace.DisplayErr("Command Not Recognized");
        var logger = Logger.GetInstance;
        
        logger.Log($"- Command not recognized: {key}");
        return HandleResult.Handled;
    }
    
}