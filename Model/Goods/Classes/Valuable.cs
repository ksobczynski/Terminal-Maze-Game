namespace PROJ.Goods.Classes;

public abstract class Valuable : BoardObject
{
    public abstract int Value { get; }
    public override bool Pickupable => true;
    // public void PickUp(Player player)
    // {
    //     // TODO PickUp Implementation
    // }
    
    public override bool Blocker => false;
    public override bool Fightable => false;
}