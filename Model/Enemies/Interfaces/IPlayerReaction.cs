namespace PROJ.Enemies.Interfaces;

public interface IPlayerReaction
{
    bool TryReact(Enemy enemy, Player player, Board board);
}
