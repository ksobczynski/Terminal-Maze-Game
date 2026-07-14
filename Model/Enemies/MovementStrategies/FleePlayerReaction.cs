using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class FleePlayerReaction : IPlayerReaction
{
    public bool TryReact(Enemy enemy, Player player, Board board)
    {
        var destination = board.GetBestEnemyEscapeDestination(enemy.BoardX, enemy.BoardY, player.Position[0], player.Position[1]);
        if (destination == null)
        {
            enemy.ClearDestination();
            return true;
        }

        enemy.SetDestination(destination.Value.X, destination.Value.Y);
        return true;
    }
}
