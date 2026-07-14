using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class IgnorePlayerReaction : IPlayerReaction
{
    public bool TryReact(Enemy enemy, Player player, Board board)
    {
        return false;
    }
}
