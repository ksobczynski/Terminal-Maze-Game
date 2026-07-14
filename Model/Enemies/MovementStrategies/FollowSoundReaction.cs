using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class FollowSoundReaction : ISoundReaction
{
    public bool TryReact(Enemy enemy, SoundMessage message, Board board)
    {
        enemy.SetDestination(message.GetX, message.GetY);
        return true;
    }
}
