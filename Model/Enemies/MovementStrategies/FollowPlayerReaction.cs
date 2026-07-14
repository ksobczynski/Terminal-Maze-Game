using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class FollowPlayerReaction : IPlayerReaction
{
    public bool TryReact(Enemy enemy, Player player, Board board)
    {
        enemy.SetDestination(player.Position[0], player.Position[1]);
        return true;
    }
}
