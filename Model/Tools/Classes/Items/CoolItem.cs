namespace PROJ.Tools.Classes.Items;

public class CoolItem : Item
{
    public CoolItem(Player player, string name = "Plague Mask", char vis = 'Ѫ') : base(player, name, vis){}
    // public override char Visual => 'Ѫ';
    public override int Space => 1;
    // public override string Name => "Plague Mask";
        
    
    public override void Use()
    {
    }
    public override bool Pickupable => Owner.Level > 5.0;
    public override string Description => "A leather beak mask meant to ward off sickness";
}
