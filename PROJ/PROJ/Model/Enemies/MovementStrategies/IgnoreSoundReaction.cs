using PROJ.Enemies.Interfaces;

namespace PROJ.Enemies.MovementStrategies;

public sealed class IgnoreSoundReaction : ISoundReaction
{
    public bool TryReact(Enemy enemy, SoundMessage message, Board board)
    {
        return false;
    }
}
