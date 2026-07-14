using PROJ.Fight.Interfaces;
using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Fight.Visitors;

public abstract class AttackVisitor : IAttackVisitor
{
    protected Player P;
    public AttackVisitor(Player p)
    {
        P = p;
    }

    public abstract AttackResult VisitLightWeapon(LightWeapon weapon);
    public abstract AttackResult VisitHeavyWeapon(HeavyWeapon weapon);

    public abstract AttackResult VisitMagicWeapon(MagicWeapon weapon);
    public abstract AttackResult VisitOther();

}