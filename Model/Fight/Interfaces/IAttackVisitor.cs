using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Fight.Interfaces;

public interface IAttackVisitor
{
    AttackResult VisitHeavyWeapon(HeavyWeapon weapon);
    AttackResult VisitLightWeapon(LightWeapon weapon);
    AttackResult VisitMagicWeapon(MagicWeapon weapon);
    AttackResult VisitOther();
}