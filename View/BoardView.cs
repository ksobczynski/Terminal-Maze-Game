using PROJ.Communication.Results;
using PROJ.GameConstansts;

namespace PROJ;

public static class BoardView
{

    public static void RenderTile(TileChangeResult res)
    {
        DrawAt(res.Column, res.Row, res.Visual);
    }
    public static void MoveRender(MoveResult result)
    {
        if (result.FromTile == null || result.ToTile == null)
            return;

        DrawAt(result.FromTile.Column, result.FromTile.Row, result.FromTile.Visual);
        DrawAt(result.ToTile.Column, result.ToTile.Row, result.ToTile.Visual);
    }
    
    private static void DrawAt(int x, int y, char symbol, ConsoleColor? color = null)
    {
        ConsoleRender.WriteAt(GameConstants.BoardLeft + x, GameConstants.BoardTop + y, symbol, color);
    }
    
    public static void SendWave(MessageBusResult res)
    {
        var mb = PickUpSoundBus.GetInstance;
        mb.Send(res.Y, res.X, res.Range);
    }
    
    public static void SoundBlink(TileBlinkResult result)
    {
        _ = Task.Run(async () =>
        {
            const int durationMs = 500;
            const int blinkIntervalMs = 125;
            int iterations = durationMs / blinkIntervalMs;

            for (int i = 0; i < iterations; i++)
            {
                if (i % 2 == 0)
                {
                    if (result.Tile.IsEmpty)
                        ConsoleRender.WriteAt(GameConstants.BoardLeft + result.Tile.Column, GameConstants.BoardTop + result.Tile.Row, ' ', backgroundColor: result.Color);
                    else
                        DrawAt(result.Tile.Column, result.Tile.Row, result.Tile.Visual, result.Color);
                }
                else
                {
                    DrawAt(result.Tile.Column, result.Tile.Row, result.Tile.Visual);
                }

                await Task.Delay(blinkIntervalMs);
            }

            DrawAt(result.Tile.Column, result.Tile.Row, result.Tile.Visual);
        });
    }

    public static void Blink(TileBlinkResult result)
    {
        BlinkTile(result,3000, 250);
    }
    
    private static void BlinkTile(TileBlinkResult result, int durationMs, int blinkIntervalMs)
    {
        _ = Task.Run(async () =>
        {
            int iterations = durationMs / blinkIntervalMs;

            for (int i = 0; i < iterations; i++)
            {
                DrawTile(result.Tile, i % 2 == 0 ? result.Color : null);
                await Task.Delay(blinkIntervalMs);
            }

            DrawTile(result.Tile);
        });
    }
    
    private static void DrawTile(TileViewResult tile, ConsoleColor? color = null)
    {
        DrawAt(tile.Column, tile.Row, tile.Visual, color);
    }
    
    public static void Display(BoardSnapshotResult snapshot)
    {
        string[] signLines = GameConstants.AboveBoardSign.Split('\n');
        for (int i = 0; i < signLines.Length; i++)
        {
            Console.SetCursorPosition(GameConstants.SignStartLeft, GameConstants.SignStartTop + i);
            Console.Write(signLines[i]);
        }

        for (int y = 0; y < GameConstants.Height; y++)
        {
            Console.SetCursorPosition(GameConstants.BoardLeft, GameConstants.BoardTop + y);
            for (int x = 0; x < GameConstants.Width; x++)
            {
                TileViewResult? tile = snapshot.Tiles.FirstOrDefault(t => t.Row == y && t.Column == x);
                Console.Write(tile?.Visual ?? ' ');
            }
        }

        string[] sign2Lines = GameConstants.BelowBoardSign.Split('\n');
        for (int i = 0; i < sign2Lines.Length; i++)
        {
            Console.SetCursorPosition(GameConstants.Sign2StartLeft, GameConstants.Sign2StartTop + i);
            Console.Write(sign2Lines[i]);
        }
    }

    public static void SpecificBlink(TileBlinkResult? result)
    {
        if (result != null)
        {
            Blink(result);
        }
    }
}
