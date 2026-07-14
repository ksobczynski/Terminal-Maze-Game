using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ;

public class GameController
{
    private readonly Board _board;
    private readonly Player _player;
    private readonly AboveActionErrorSpace _errSpace;

    public GameController(Board board, Player player, AboveActionErrorSpace errSpace)
    {
        _board = board;
        _player = player;
        _errSpace = errSpace;
    }
    public void Run()
    {
        var sh = new SeekHandler(_board);
        var eh = new EscapeHandler(_player);
        var mh = new MoveHandler(_board);
        var puh = new PickUpHandler(_player);
        var bmh = new BackpackModeHandler(_player);
        var fh = new FightHandler(_board);
        var lm = new LoggerMode(_board);
        var dmh = new DisallowedMoveHandler(_errSpace);

        fh.SetNext(sh);
        sh.SetNext(eh);
        eh.SetNext(mh);
        mh.SetNext(puh);
        puh.SetNext(lm);
        lm.SetNext(bmh);
        bmh.SetNext(dmh);

        while (true)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            var res = fh.Handle(key);
            if (res == HandleResult.ExitGame)
                return;
        }
    }
}
