using PROJ.Communication.Results;

namespace PROJ;

public class FrameObject : BoardObject
{
    private char _vis;
    public FrameObject(char c)
    {
        _vis = c;
    }

    public override char Visual => _vis;
    public override bool Blocker => true;
    public override string Description => " ";

    public override string Name => "";

    public override PickUpResult? PickUp(Player player)
    {
        return null;
    }
    
    public override bool Pickupable => false;
    public override bool Fightable => false;
}