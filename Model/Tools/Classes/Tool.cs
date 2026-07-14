using PROJ.Communication.Results;
using PROJ.Fight;
using PROJ.Fight.Interfaces;
using PROJ.GameConstansts;
using PROJ.Logging.Classes;

namespace PROJ.Tools.Classes;

public abstract class Tool :  BoardObject, ITool, IUsable
{
    public abstract int Space { get; }
    protected Player Owner;

    protected string name;
    protected char vis;

    public override bool Fightable => false;
    
    public override string Name =>  name;
    public override char Visual => vis;


    public Tool(Player player, string name = "unnamed", char vis = 'X')
    {
        Owner = player;
        this.name = name;
        this.vis = vis;
    }

    public abstract void Use();
    public override bool Pickupable => true; // TODO musze sprawdzic czy ta logika zadziała

    public override PickUpResult? PickUp(Player player)
    {
        var logger = Logger.GetInstance;
        if (!Pickupable)
        {
            // Owner.ErrSpace.DisplayErr("Can't Pick up that object yet!");
            logger.Log($"{player.Name} Tried to Pick up {Name} with no success");
            return new PickUpResult(false, null, null, errormsg:"Can't Pick up that object yet!");
        }
        if (ObjBoard != null)
        {
            if (player.PlayerBackpack.TryAddItem(this))
            {
                logger.Log($" {player.Name} Picked up {Name}");
                var res = ObjBoard.RemoveFromMap(X,Y);
                // player.EqBox.DisplayItems();
                // ObjBoard.RefreshActionBox(X,Y);

                var s = GetPickupMessage();
                return new PickUpResult(true, s, res);
            }
            else
            {
                logger.Log($"{player.Name} Tried to Pick up {Name} with no success");
                // Owner.ErrSpace.DisplayErr("Item too large!");
                return new PickUpResult(false, null, null, errormsg:"Item too large!");
            }
        }
        return null;

    }

    public virtual MessageBusResult? GetPickupMessage()
    {
        return null;
    }
    public virtual int GetStrength => 0;

    public virtual int GetLuck => 0;
    public virtual int GetDexterity => 0;
    public virtual int GetWisdom => 0;

    public override bool Blocker => false;
    
    public virtual AttackResult Accept(IAttackVisitor visitor)
    {
        return visitor.VisitOther();
    }
}