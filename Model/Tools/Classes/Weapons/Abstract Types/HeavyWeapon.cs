using PROJ.Communication.Results;
using PROJ.Fight;
using PROJ.Fight.Interfaces;

namespace PROJ.Tools.Classes.Weapons.Abstract_Types;

public abstract class HeavyWeapon : Weapon
{
    public HeavyWeapon(Player player, string name = "unnamed", char vis = 'X') : base(player, name, vis)
    {
    }

    public override AttackResult Accept(IAttackVisitor visitor)
    {
        return visitor.VisitHeavyWeapon(this);
    }

    public override MessageBusResult? GetPickupMessage()
    {
        // var messageBus = PickUpSoundBus.GetInstance;
        // messageBus.Send(Y, X, 10);
        return new MessageBusResult(X, Y, 10);
    }
}
