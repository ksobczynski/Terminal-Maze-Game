namespace PROJ.Tools.Classes.Items;

public class MediumItem : Item
{
    public MediumItem(Player player, string name = "Physician's Ledger", char vis = '⎙') : base(player, name, vis){}

    // public override char Visual => '⎙';
    public override int Space => 1;
    // public override string Name => "Physician's Ledger";
    public override void Use()
    {
    }
    public override bool Pickupable => Owner.Level > 10.0;
    public override string Description => "A doctor’s record book from the plague wards";
}
