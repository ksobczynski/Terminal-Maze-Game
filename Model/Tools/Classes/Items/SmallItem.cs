namespace PROJ.Tools.Classes.Items;

public class SmallItem : Item
{
    public SmallItem(Player player, string name = "Rosary", char vis = '✟') : base(player, name, vis){}
    
    // public override char Visual => '✟';
    public override int Space => 1;
    // public override string Name => "Rosary";
    
    public override void Use()
    {
    }
    public override bool Pickupable => Owner.Level > 4.0;
    public override string Description => "A prayer rope carried through the plague years";
    
}
