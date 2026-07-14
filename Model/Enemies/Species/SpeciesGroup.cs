using PROJ.Enemies.Species.Interfaces;
using PROJ.Enemies.Death_Reaction_Strategies;
using PROJ.Enemies.Interfaces;
using PROJ.Enemies.MovementStrategies;

namespace PROJ.Enemies.Species;

public enum EnemyBehaviorTrigger
{
    Player,
    Sound
}

public class SpeciesGroup
{
    private string _name;

    private List<ISpeciesObserver> _members = new();
    private IDeathReactionStrategy _strategy;
    private readonly ISoundReaction _soundReaction;
    private readonly IPlayerReaction _playerReaction;
    private readonly EnemyBehaviorTrigger[] _priority;
    public virtual string Name => _name;

    public SpeciesGroup(
        string name,
        IDeathReactionStrategy? strategy = null,
        ISoundReaction? soundReaction = null,
        IPlayerReaction? playerReaction = null,
        EnemyBehaviorTrigger[]? priority = null)
    {
        _name = name;
        if (strategy == null)
            _strategy = new DefaultStrategy();
        else
            _strategy = strategy;

        _soundReaction = soundReaction ?? new IgnoreSoundReaction();
        _playerReaction = playerReaction ?? new IgnorePlayerReaction();
        _priority = priority ?? new[] { EnemyBehaviorTrigger.Player, EnemyBehaviorTrigger.Sound };
    }

    public void Subscribe(ISpeciesObserver observer) => _members.Add(observer);
    public void Unsubscribe(ISpeciesObserver observer) => _members.Remove(observer);

    public void NotifyDeath()
    {
        foreach (var member in _members)
        {
            member.NotifyDeath();
        }
    }
    
    public void ReactionStrategy(Enemy e)
    {
        _strategy.ReactToDeath(e);
    }

    public void AssignBehavior(Enemy enemy)
    {
        enemy.SetReactions(_soundReaction, _playerReaction);
    }

    public void RefreshDestination(Enemy enemy, Player? visiblePlayer, SoundMessage? soundMessage, Board board)
    {
        foreach (EnemyBehaviorTrigger trigger in _priority)
        {
            if (trigger == EnemyBehaviorTrigger.Player &&
                visiblePlayer != null &&
                enemy.CurrentPlayerReaction.TryReact(enemy, visiblePlayer, board))
            {
                return;
            }

            if (trigger == EnemyBehaviorTrigger.Sound &&
                soundMessage != null &&
                enemy.CurrentSoundReaction.TryReact(enemy, soundMessage, board))
            {
                return;
            }
        }

        enemy.ClearDestination();
    }
}
