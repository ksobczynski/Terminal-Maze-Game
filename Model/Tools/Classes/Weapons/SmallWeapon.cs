using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Tools.Classes.Weapons;

public class SmallWeapon : LightWeapon
{
    public SmallWeapon(Player player, string name = "Shiv", char vis = '‡') : base(player, name, vis){}

    // public override char Visual => '‡';
    public override int Space => 1;
    public override int Damage => 5;
    // public override int Range => 0;
    // public override float Cooldown => 1.0f;
    // public override string Name => "Shiv";

    public override void Use()
    {
    }

    public override bool TwoHanded() => false;

    public override string Description => "It's crooked and dirty, Short ranged and quite handy.";
    
}
