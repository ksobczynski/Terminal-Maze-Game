using PROJ.Handlers.Enums;
using PROJ.Logging.Classes;

namespace PROJ.Handlers;

public class LoggerMode : Handler
{
    private Board _board;
    public LoggerMode(Board b)
    {
        _board = b;
    }

    public override HandleResult Handle(ConsoleKey key)
    {
        var logger = Logger.GetInstance;
        if (logger.LoggerMode)
        {
            if (key == ConsoleKey.UpArrow)
            {
                logger.LogUp();
                return HandleResult.Handled;
            }

            if (key == ConsoleKey.DownArrow)
            {
                logger.LogDown();
                return HandleResult.Handled;
            }

            if (key == ConsoleKey.J || key == ConsoleKey.Escape)
            {
                logger.LoggerMode = false;
                return HandleResult.Handled;
            }

            return HandleResult.Handled;
        }

        if (key == ConsoleKey.J)
        {
            logger.LoggerMode = true;
            return HandleResult.Handled;
        }

        if (next != null)
            return next.Handle(key);

        return HandleResult.NotHandled;
    }
}
