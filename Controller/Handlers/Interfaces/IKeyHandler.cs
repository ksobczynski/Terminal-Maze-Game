using PROJ.Handlers.Enums;

namespace PROJ.Handlers.Interfaces;

public interface IKeyHandler
{
    HandleResult Handle(ConsoleKey key);
}