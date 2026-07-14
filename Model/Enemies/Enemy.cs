using System.ComponentModel.DataAnnotations;
using PROJ.Communication.Results;
using PROJ.Enemies.Interfaces;
using PROJ.Enemies.MovementStrategies;
using PROJ.Enemies.Species;
using PROJ.Enemies.Species.Interfaces;
using PROJ.GameConstansts;
using PROJ.Logging.Classes;

namespace PROJ.Enemies;

public abstract class Enemy : BoardObject, ISpeciesObserver, ISoundReceiver
{
    protected int _armor;
    protected int _health;
    protected int _damage;

    protected char vis;
    protected string name;
    protected SpeciesGroup? speciesGroup;
    private SoundMessage? _lastSound;
    protected Board EnemyBoard;
    public ISoundReaction CurrentSoundReaction { get; private set; }
    public IPlayerReaction CurrentPlayerReaction { get; private set; }
    public (int X, int Y)? Destination { get; private set; }
    public int[]? Direction { get; private set; }
    public int BoardX => Y;
    public int BoardY => X;
    public SoundMessage? LastSound => _lastSound;
    public override bool Blocker => true;
    public override bool Pickupable => false;
    public override PickUpResult? PickUp(Player player) // to do zmiany TODO
    {
        return null;
    }

    public Enemy(Board b, int armor = GameConstants.BaseEnemyArmor, int health = GameConstants.BaseEnemyHealth, int damage = GameConstants.BaseEnemyDamage,
        char vis = 'X', string name = "unnamed", SpeciesGroup? speciesGroup = null)
    {
        _armor = armor;
        _health = health;
        _damage = damage;
        this.vis = vis;
        this.name = name;
        CurrentSoundReaction = new IgnoreSoundReaction();
        CurrentPlayerReaction = new IgnorePlayerReaction();
        Direction = null;
        Destination = null;
        if (speciesGroup != null)
        {
            this.speciesGroup = speciesGroup;
            this.speciesGroup.Subscribe(this);
            this.speciesGroup.AssignBehavior(this);
        }

        var messageBus = PickUpSoundBus.GetInstance;
        messageBus.Subscribe(this);

        EnemyBoard = b;
    }

    public override string Name => name;

    public override char Visual => vis;


    public virtual int Armor => _armor;

    public virtual int Health => _health;
    public virtual int Damage => _damage;
    public override bool Fightable => true;

    public virtual void Hit(int dmg)
    {
        if (_armor > 0)
        {
            _armor = int.Max(0, _armor - dmg);
        }
        else
        {
            _health = int.Max(0, _health - dmg);
        }
    }

    public virtual bool Dead()
    {
        
        return _health == 0;
    }

    public void Attack(Player p, int defense)
    {
        int attackValue = Int32.Max(Damage - defense,0);
        
        p.Health = int.Max(p.Health - attackValue,0);
    }

    public void NotifyDeath()
    {
        speciesGroup?.ReactionStrategy(this);
    }

    // public void ModifyHealth(int amount)
    // {
    //     _health += amount;
    //     if (_health <= 0)
    //         Dead();
    // }
    public void ModifyArmor(int amount)
    {
        _armor += amount;
        if (_armor <= 0)
            _armor = 0;
    }
    public void ModifyDamage(int amount)
    {
        _damage += amount;
        if (_damage <= 0)
            _damage = 0;
    }

    // public void Blink(ConsoleColor color)
    // {
    // BoardView.Blink(X, Y, color, EnemyBoard);
    // }

    public void DeleteYourself()
    {
        speciesGroup?.Unsubscribe(this);
        speciesGroup?.NotifyDeath();
        EnemyBoard.DeleteEnemy(this);

        var messageBus = PickUpSoundBus.GetInstance;
        
        messageBus.UnSubscribe(this);
        
    }

    public void SetReactions(ISoundReaction soundReaction, IPlayerReaction playerReaction)
    {
        CurrentSoundReaction = soundReaction;
        CurrentPlayerReaction = playerReaction;
    }

    public void SetDestination(int x, int y)
    {
        Destination = (x, y);
        SetDirection(Math.Sign(x - BoardX), Math.Sign(y - BoardY));
    }

    public void ClearDestination()
    {
        Destination = null;
        Direction = null;
    }

    public void SetDirection(int x, int y)
    {
        Direction = x == 0 && y == 0 ? null : new[] { x, y };
    }

    public void RefreshDestination()
    {
        Player? visiblePlayer = EnemyBoard.GetNearestVisiblePlayer(this);
        if (speciesGroup == null)
        {
            ClearDestination();
            return;
        }

        speciesGroup.RefreshDestination(this, visiblePlayer, _lastSound, EnemyBoard);
    }

    public void ClearSound()
    {
        _lastSound = null;
    }

    public void ClearSoundIfSourceReached()
    {
        if (_lastSound != null && BoardX == _lastSound.GetX && BoardY == _lastSound.GetY)
            _lastSound = null;
    }

    public void RegisterSound(SoundMessage message)
    {
        var logger = Logger.GetInstance;
        logger.Log($"{Name} Heard {message.GetMessage} From ({message.GetX}, {message.GetY}), {message.GetDist} Tiles Away");
        _lastSound = message;
        RefreshDestination();
    }
    
}
