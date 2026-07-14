namespace PROJ;

public static class ConsoleRender
{
    private static readonly object Sync = new();

    public static void Run(Action action, bool preserveCursor = true, bool preserveColor = true)
    {
        lock (Sync)
        {
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            ConsoleColor backgroundColor = Console.BackgroundColor;

            action();

            if (preserveColor)
            {
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
            }

            if (preserveCursor)
                Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }

    public static void WriteAt(int left, int top, char symbol, ConsoleColor? color = null, ConsoleColor? backgroundColor = null)
    {
        Run(() =>
        {
            Console.SetCursorPosition(left, top);
            if (color.HasValue)
                Console.ForegroundColor = color.Value;
            if (backgroundColor.HasValue)
                Console.BackgroundColor = backgroundColor.Value;
            Console.Write(symbol);
        });
    }
}
