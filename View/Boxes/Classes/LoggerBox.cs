using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class LoggerBox : Box
{
    private readonly List<string> _logs = new();

    private int _firstVisibleIndex;

    private int FirstLine => GameConstants.LoggerBoxWritingPointStartTop;

    private int LastLine => GameConstants.LoggerBoxBottom - 1;

    private int VisibleLines => LastLine - FirstLine + 1;

    private int MaxTextLength => GameConstants.LoggerBoxRight - GameConstants.LoggerBoxWritingPointStartLeft;

    private int MaxFirstVisibleIndex => Math.Max(0, _logs.Count - VisibleLines);

    public override void DisplayFrame()
    {
        ConsoleRender.Run(() =>
        {
            // ┌ ┐ └ ┘ ─ │
            Console.SetCursorPosition(GameConstants.LoggerBoxLeft, GameConstants.LoggerBoxTop);
            Console.Write('┌');
            Console.SetCursorPosition(GameConstants.LoggerBoxRight, GameConstants.LoggerBoxTop);
            Console.Write('┐');
            Console.SetCursorPosition(GameConstants.LoggerBoxLeft, GameConstants.LoggerBoxBottom);
            Console.Write('└');
            Console.SetCursorPosition(GameConstants.LoggerBoxRight, GameConstants.LoggerBoxBottom);
            Console.Write('┘');

            for (int i = 1; i < GameConstants.LoggerBoxBottom - GameConstants.LoggerBoxTop; i++)
            {
                Console.SetCursorPosition(GameConstants.LoggerBoxLeft, GameConstants.LoggerBoxTop + i);
                Console.Write('│');
                Console.SetCursorPosition(GameConstants.LoggerBoxRight, GameConstants.LoggerBoxTop + i);
                Console.Write('│');
            }
            for (int i = 1; i < GameConstants.LoggerBoxRight - GameConstants.LoggerBoxLeft ; i++)
            {
                Console.SetCursorPosition(GameConstants.LoggerBoxLeft + i, GameConstants.LoggerBoxTop);
                Console.Write('─');
                Console.SetCursorPosition(GameConstants.LoggerBoxLeft + i, GameConstants.LoggerBoxBottom);
                Console.Write('─');
            }

            string tmp = " Log ";
            Console.SetCursorPosition((GameConstants.LoggerBoxRight + GameConstants.LoggerBoxLeft) / 2 - tmp.Length/2, GameConstants.LoggerBoxTop);
            Console.Write(tmp);
        });
    }

    public void AddLog(string move)
    {
        bool wasAtBottom = _firstVisibleIndex >= MaxFirstVisibleIndex;

        _logs.Add(move);

        if (wasAtBottom)
            _firstVisibleIndex = MaxFirstVisibleIndex;

        Render();
    }

    public void LogUp()
    {
        if (_firstVisibleIndex == 0)
            return;

        _firstVisibleIndex--;
        Render();
    }

    public void LogDown()
    {
        if (_firstVisibleIndex >= MaxFirstVisibleIndex)
            return;

        _firstVisibleIndex++;
        Render();
    }

    private void Render()
    {
        ConsoleRender.Run(() =>
        {
            ClearVisibleArea();

            int visibleCount = Math.Min(VisibleLines, _logs.Count - _firstVisibleIndex);
            for (int i = 0; i < visibleCount; i++)
            {
                Console.SetCursorPosition(GameConstants.LoggerBoxWritingPointStartLeft, FirstLine + i);
                Console.Write(TrimToVisibleWidth(_logs[_firstVisibleIndex + i]));
            }
        });
    }

    private void ClearVisibleArea()
    {
        for (int row = FirstLine; row <= LastLine; row++)
        {
            Console.SetCursorPosition(GameConstants.LoggerBoxWritingPointStartLeft, row);
            Console.Write(new string(' ', MaxTextLength));
        }
    }

    private string TrimToVisibleWidth(string text)
    {
        if (text.Length <= MaxTextLength)
            return text;

        return text[..MaxTextLength];
    }
}
