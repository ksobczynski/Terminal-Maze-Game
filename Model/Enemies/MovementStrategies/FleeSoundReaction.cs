using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class FleeSoundReaction : ISoundReaction
{
    public bool TryReact(Enemy enemy, SoundMessage message, Board board)
    {
        var destination = board.GetBestEnemyEscapeDestination(enemy.BoardX, enemy.BoardY, message.GetX, message.GetY);
        if (destination == null)
        {
            enemy.ClearDestination();
            return true;
        }

        enemy.SetDestination(destination.Value.X, destination.Value.Y);
        return true;
    }
}
