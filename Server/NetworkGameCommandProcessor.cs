using PROJ.Communication.Results;
using PROJ.Handlers;
using PROJ.Handlers.Enums;

namespace PROJ.Server;

public sealed class NetworkGameCommandProcessor
{
    private readonly Board board;
    private readonly Player player;
    private readonly Handler rootHandler;
    private bool gameEnded;
    private bool endedGood;
    private string? lastActionError;
    private string? lastFightError;
    private IReadOnlyList<GameOutput.RecordedTileEffect> lastEffects = Array.Empty<GameOutput.RecordedTileEffect>();

    public NetworkGameCommandProcessor(Board board, Player player)
    {
        this.board = board;
        this.player = player;
        var fightHandler = new NetworkFightHandler(board);
        var seekHandler = new SeekHandler(board);
        var escapeHandler = new EscapeHandler(player);
        var moveHandler = new MoveHandler(board);
        var pickUpHandler = new PickUpHandler(player);
        var loggerMode = new LoggerMode(board);
        var backpackModeHandler = new BackpackModeHandler(player);

        fightHandler.SetNext(seekHandler);
        seekHandler.SetNext(escapeHandler);
        escapeHandler.SetNext(moveHandler);
        moveHandler.SetNext(pickUpHandler);
        pickUpHandler.SetNext(loggerMode);
        loggerMode.SetNext(backpackModeHandler);

        rootHandler = fightHandler;
    }

    public bool GameEnded => gameEnded;
    public bool EndedGood => endedGood;
    public string? LastActionError => lastActionError;
    public string? LastFightError => lastFightError;
    public IReadOnlyList<GameOutput.RecordedTileEffect> LastEffects => lastEffects;

    public void Handle(ConsoleKey key)
    {
        if (gameEnded)
            return;

        board.SetActivePlayer(player);
        lastActionError = null;
        lastFightError = null;
        lastEffects = Array.Empty<GameOutput.RecordedTileEffect>();

        using GameOutput.GameOutputScope output = GameOutput.SuppressRendering();
        HandleResult result = rootHandler.Handle(key);
        lastActionError = output.ActionError;
        lastFightError = output.FightError;
        lastEffects = output.Effects.ToList();

        if (result == HandleResult.NotHandled)
            lastActionError = "Command Not Recognized";

        if (result == HandleResult.ExitGame)
        {
            gameEnded = true;
            endedGood = key == ConsoleKey.Escape;
        }
    }

    private sealed class NetworkFightHandler : Handler
    {
        private readonly Board board;

        public NetworkFightHandler(Board board)
        {
            this.board = board;
        }

        public override HandleResult Handle(ConsoleKey key)
        {
            if (!board.IsFightActive)
            {
                if (key != ConsoleKey.Enter)
                    return next?.Handle(key) ?? HandleResult.NotHandled;

                FightStartResult startResult = board.FightNearestEnemy();
                GameOutput.Apply(startResult);
                return HandleResult.Handled;
            }

            switch (key)
            {
                case ConsoleKey.Escape:
                    FightExitResult? exitResult = board.ExitCurrentFight();
                    if (exitResult != null)
                        GameOutput.Apply(exitResult);
                    return HandleResult.Handled;
                case ConsoleKey.D1:
                    ApplyIfNotNull(board.SelectCurrentFightAttack(1));
                    return HandleResult.Handled;
                case ConsoleKey.D2:
                    ApplyIfNotNull(board.SelectCurrentFightAttack(2));
                    return HandleResult.Handled;
                case ConsoleKey.D3:
                    ApplyIfNotNull(board.SelectCurrentFightAttack(3));
                    return HandleResult.Handled;
                case ConsoleKey.L:
                    ApplyIfNotNull(board.SelectCurrentFightHand('L'));
                    return HandleResult.Handled;
                case ConsoleKey.R:
                    ApplyIfNotNull(board.SelectCurrentFightHand('R'));
                    return HandleResult.Handled;
                case ConsoleKey.Enter:
                    FightTurnResult? turnResult = board.SimulateCurrentFightAttack();
                    if (turnResult == null)
                        return HandleResult.Handled;

                    GameOutput.Apply(turnResult);
                    return turnResult.PlayerDead ? HandleResult.ExitGame : HandleResult.Handled;
                default:
                    return HandleResult.Handled;
            }
        }

        private static void ApplyIfNotNull(FightAttackSelectionResult? result)
        {
            if (result != null)
                GameOutput.Apply(result);
        }

        private static void ApplyIfNotNull(FightHandSelectionResult? result)
        {
            if (result != null)
                GameOutput.Apply(result);
        }
    }
}
