using PROJ.Communication.Results;
using PROJ.Tools;

namespace PROJ;

public abstract class BoardObject : IPickupable
{
    public int X;
    public int Y;
    public Board? ObjBoard;
    
    public abstract char Visual { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract bool Pickupable { get; }
    public abstract PickUpResult? PickUp(Player player);
    
    public abstract bool Blocker { get; }
    public abstract bool Fightable { get; }
}