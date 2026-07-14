namespace PROJ.Tools.Classes.Decorators;

public class StrongWeaponDecorator : WeaponDecorator
{
    public override string Name => _inner.Name + " (Strong)";
    public override int GetStrength => 5;

    public StrongWeaponDecorator(Player player, Weapon weapon, string name="unnamed", char vis='X') : base(player, weapon, name, vis)
    {
    }

    
}