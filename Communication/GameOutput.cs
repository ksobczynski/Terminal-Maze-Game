using System.Threading;
using PROJ.Communication.Results;

namespace PROJ;

public static class GameOutput
{
    private static readonly AsyncLocal<bool> RenderingSuppressed = new();
    private static readonly AsyncLocal<GameOutputScope?> CurrentScope = new();

    public static GameOutputScope SuppressRendering() 
    {
        bool previous = RenderingSuppressed.Value;
        GameOutputScope? previousScope = CurrentScope.Value;
        RenderingSuppressed.Value = true;
        GameOutputScope scope = new GameOutputScope(previous, previousScope);
        CurrentScope.Value = scope;
        return scope;
    }

    public static void Apply(BackpackModeSwitchResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(BackpackPointerMoveResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(BackpackHandChangeResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(BackpackDropResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(List<MoveResult> results)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(results);
    }

    public static void Apply(FightStartResult result)
    {
        if (!result.IsSuccess && result.ErrorMessage != null)
            CurrentScope.Value?.SetActionError(result.ErrorMessage);

        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(FightAttackSelectionResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(FightHandSelectionResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(FightExitResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(FightTurnResult result)
    {
        if (result.ErrorMessage != null)
            CurrentScope.Value?.SetFightError(result.ErrorMessage);

        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(FightLoopEndResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(SeekResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(PickUpResult result)
    {
        if (!result.IsSuccess && result.Errormsg != null)
            CurrentScope.Value?.SetActionError(result.Errormsg);

        if (RenderingSuppressed.Value && result.IsSuccess && result.Result != null)
            BoardView.SendWave(result.Result);

        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void Apply(GameEndResult result)
    {
        if (!RenderingSuppressed.Value)
            GameView.GetInstance().Apply(result);
    }

    public static void SoundBlink(TileBlinkResult? result, int delayMs = 0)
    {
        if (result == null)
            return;

        if (RenderingSuppressed.Value)
        {
            CurrentScope.Value?.AddEffect("sound", result, delayMs);
            return;
        }

        RunDelayed(delayMs, () => BoardView.SoundBlink(result));
    }

    public static void SpecificBlink(TileBlinkResult? result, int delayMs = 0)
    {
        if (result == null)
            return;

        if (RenderingSuppressed.Value)
        {
            CurrentScope.Value?.AddEffect("blink", result, delayMs);
            return;
        }

        RunDelayed(delayMs, () => BoardView.SpecificBlink(result));
    }

    private static void RunDelayed(int delayMs, Action action)
    {
        if (delayMs <= 0)
        {
            action();
            return;
        }

        _ = Task.Run(async () =>
        {
            await Task.Delay(delayMs);
            action();
        });
    }

    public sealed class GameOutputScope : IDisposable
    {
        private readonly bool previous;
        private readonly GameOutputScope? previousScope;
        private bool disposed;
        private readonly List<RecordedTileEffect> effects = new();

        public GameOutputScope(bool previous, GameOutputScope? previousScope)
        {
            this.previous = previous;
            this.previousScope = previousScope;
        }

        public string? ActionError { get; private set; }
        public string? FightError { get; private set; }
        public IReadOnlyList<RecordedTileEffect> Effects => effects;

        public void SetActionError(string errorMessage)
        {
            ActionError = errorMessage;
        }

        public void SetFightError(string errorMessage)
        {
            FightError = errorMessage;
        }

        public void AddEffect(string kind, TileBlinkResult result, int delayMs)
        {
            effects.Add(new RecordedTileEffect(kind, result, delayMs));
        }

        public void Dispose()
        {
            if (disposed)
                return;

            RenderingSuppressed.Value = previous;
            CurrentScope.Value = previousScope;
            disposed = true;
        }
    }

    public sealed class RecordedTileEffect
    {
        public RecordedTileEffect(string kind, TileBlinkResult result, int delayMs)
        {
            Kind = kind;
            Result = result;
            DelayMs = delayMs;
        }

        public string Kind { get; }
        public TileBlinkResult Result { get; }
        public int DelayMs { get; }
    }
}
