using PROJ.Communication.Results;
using PROJ.Fight;
using PROJ.Fight.Interfaces;

namespace PROJ.Tools.Classes.Decorators;

public abstract class WeaponDecorator : Weapon
{
    protected Weapon _inner;

    public WeaponDecorator(Player player, Weapon inner, string name, char vis) : base(player, name, vis)
    {
        _inner = inner;
    }
    
    public virtual bool CanBeHeld => _inner.Pickupable;
    public virtual bool IsTwoHanded => _inner.TwoHanded();

    // public virtual int StrengthBonus => 0;
    // public virtual int DexterityBonus => 0;
    // public virtual int WisdomBonus => 0;
    // public virtual int LuckBonus => 0;

    public override int Space => _inner.Space;
    public override char Visual => _inner.Visual;
    public override void Use()
    {
        
    }

    public override bool Pickupable => _inner.Pickupable;

    public override bool TwoHanded()
    {
        return _inner.TwoHanded();
    }

    public override string Description => _inner.Description;

    public override AttackResult Accept(IAttackVisitor visitor)
    {
        return _inner.Accept(visitor);
    }
    public override MessageBusResult GetPickupMessage()
    {
        _inner.X = X;
        _inner.Y = Y;
        _inner.ObjBoard = ObjBoard;
        return _inner.GetPickupMessage();
    }
}
