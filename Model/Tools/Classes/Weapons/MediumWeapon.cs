using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Tools.Classes.Weapons;

public class MediumWeapon : HeavyWeapon
{
    public MediumWeapon(Player player, string name = "Sailor's Cutlass", char vis = '☽') : base(player, name, vis){}
    // public override char Visual => '☽';
    public override int Space => 1;
    public override int Damage => 10;
    // public override int Range => 0;
    // public override float Cooldown => 1.5f;
    // public override string Name => "Sailor's Cutlass";


    public override void Use()
    {
    }

    public override string Description =>  "A sturdy cutlass once carried by a Marseille sailor.";
    public override bool TwoHanded() => false;
}