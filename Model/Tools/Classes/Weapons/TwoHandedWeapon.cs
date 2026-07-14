using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Tools.Classes.Weapons;

public class TwoHandedWeapon : HeavyWeapon
{
    public TwoHandedWeapon(Player player, string name = "Boat Hook", char vis = '⟆') : base(player, name, vis){}

    // public override char Visual => '⟆';
    public override int Space => 2;
    public override int Damage => 20;
    // public override int Range => 1;
    // public override float Cooldown => 2.5f;
    // public override string Name => "Boat Hook";


    public override string Description => "A long harbor hook turned into a brutal weapon.";
    public override void Use()
    {
    }
    public override bool TwoHanded() => true;
}