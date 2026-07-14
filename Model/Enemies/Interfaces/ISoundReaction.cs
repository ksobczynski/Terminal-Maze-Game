namespace PROJ.Enemies.Interfaces;

public interface ISoundReaction
{
    bool TryReact(Enemy enemy, SoundMessage message, Board board);
}
