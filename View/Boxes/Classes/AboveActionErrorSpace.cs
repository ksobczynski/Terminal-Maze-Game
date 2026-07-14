using PROJ.Boxes;
using PROJ.GameConstansts;

namespace PROJ;

public class AboveActionErrorSpace : Box
{
    public override void DisplayFrame()
    {
        return;
    }

    private void Reset()
    {
        string s = new string(' ', GameConstants.ActionBoxRight - GameConstants.ActionBoxLeft);
        Console.SetCursorPosition(GameConstants.ActionBoxLeft, GameConstants.ErrorSpaceTop);
        Console.Write(s);
        
    }

    public void DisplayErr(string s)
    {
        Console.SetCursorPosition((GameConstants.ActionBoxLeft + GameConstants.ActionBoxRight - s.Length)/2, GameConstants.ErrorSpaceTop);
        Console.Write("\u001b[31m");
        Console.Write(s);
        Console.Write("\u001b[0m");
        Task.Run(async () =>
        {
            await Task.Delay(3000);
            Reset();
        });
    }
    
}