using PROJ.Enemies;
using PROJ.Communication.Results;
using PROJ.Logging.Classes;

namespace PROJ;

public sealed class PickUpSoundBus
{
    
    

    private static readonly PickUpSoundBus _instance = new PickUpSoundBus();
    private Board? _board;
    private List<Enemy> enemies = new List<Enemy>();
    public static PickUpSoundBus GetInstance => _instance;

    private PickUpSoundBus() { }

    public void Init(Board board)
    {
        _board = board;
    }

    public void Subscribe(Enemy e)
    {
        enemies.Add(e);
    }

    public void UnSubscribe(Enemy e)
    {
        enemies.Remove(e);
    }

    public void Send(int x, int y, int range)
    {
        if (_board == null)
            throw new InvalidOperationException("PickUpSoundBus has not been initialized.");

        var logger = Logger.GetInstance;
        // logger.Log($"Pickup sound from ({x}, {y}) with range {range}");

        var tiles = _board.Tiles;
        int height = tiles.GetLength(0);
        int width = tiles.GetLength(1);

        if (x < 0 || x >= width || y < 0 || y >= height || range < 0)
            return;

        bool[,] visited = new bool[height, width];
        Queue<(int x, int y, int depth)> queue = new();

        visited[y, x] = true;
        queue.Enqueue((x, y, 0));

        while (queue.Count > 0)
        {
            var (currentX, currentY, depth) = queue.Dequeue();
            var tile = tiles[currentY, currentX];
            TileBlinkResult? blink = _board.CreateTileBlinkResult(currentY, currentX, GetSoundColor(depth, range));
            GameOutput.SoundBlink(blink, depth * 100);
            // logger.Log($"Sound wave at ({currentX}, {currentY}) depth {depth}");

            var enemy = tile.TryGetEnemy();
            if (enemy != null)
            {
                enemy.RegisterSound(new SoundMessage(x, y, depth, "Pickup"));
            }

            bool blocksSound = tile.Content != null &&
                               tile.Content.Exists(obj => obj is Wall || obj is FrameObject);
            if (blocksSound || depth >= range)
                continue;

            TryEnqueue(currentX + 1, currentY, depth + 1);
            TryEnqueue(currentX - 1, currentY, depth + 1);
            TryEnqueue(currentX, currentY + 1, depth + 1);
            TryEnqueue(currentX, currentY - 1, depth + 1);
        }

        void TryEnqueue(int nextX, int nextY, int nextDepth)
        {
            if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)
                return;
            if (visited[nextY, nextX])
                return;

            visited[nextY, nextX] = true;
            queue.Enqueue((nextX, nextY, nextDepth));
        }
    }

    private static ConsoleColor GetSoundColor(int depth, int range)
    {
        if (range <= 0)
            return ConsoleColor.Yellow;

        double ratio = (double)depth / range;
        if (ratio <= 0.25)
            return ConsoleColor.Yellow;
        if (ratio <= 0.5)
            return ConsoleColor.DarkYellow;
        if (ratio <= 0.75)
            return ConsoleColor.Gray;
        return ConsoleColor.DarkGray;
    }

}
