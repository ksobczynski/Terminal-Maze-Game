using PROJ.Communication.Results;
using PROJ.GameConstansts;

namespace PROJ;

public class Wall : BoardObject
{
    public override string Name => "Wall";
    public override bool Pickupable => false;

    public override char Visual => GameConstants.WallSymbol;

    public override string Description => " ";

    public override PickUpResult? PickUp(Player player)
    {
        throw new Exception("Can't Pick up a wall");
    }
    
    public override bool Blocker => true;
    public override bool Fightable => false;
}