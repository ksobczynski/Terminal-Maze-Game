using PROJ.Handlers.Enums;
using PROJ.Handlers.Interfaces;

namespace PROJ.Handlers;

public abstract class Handler : IKeyHandler
{
    protected Handler? next;

    public void SetNext(Handler h)
    {
        next = h;
    }

    public abstract HandleResult Handle(ConsoleKey key);
}