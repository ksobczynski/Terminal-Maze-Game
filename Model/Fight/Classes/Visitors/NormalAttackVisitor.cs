using PROJ.Fight.Visitors;
using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Fight.Classes.Visitors;

public class NormalAttackVisitor : AttackVisitor
{
    public NormalAttackVisitor(Player p) : base(p)
    {
    }

    public override AttackResult VisitLightWeapon(LightWeapon weapon)
    {
        int damage = weapon.Damage;
        int defense = P.Dexterity + P.Luck;
        int leftHandDef = 0;
        int rightHandDef = 0;
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetDexterity + P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetDexterity + P.RightHand.GetLuck;
        defense += leftHandDef;
        defense += rightHandDef;
        return new AttackResult(damage, defense);
    }
    
    public override AttackResult VisitHeavyWeapon(HeavyWeapon weapon)
    {
        int damage = weapon.Damage;
        int defense = P.Strength + P.Luck;
        int leftHandDef = 0;
        int rightHandDef = 0;
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetStrength + P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetStrength + P.RightHand.GetLuck;
        defense += leftHandDef;
        defense += rightHandDef;
        return new AttackResult(damage, defense);
    }
    
    public override AttackResult VisitMagicWeapon(MagicWeapon weapon)
    {
        int damage = 1;
        int defense = P.Dexterity + P.Luck;
        int leftHandDef = 0;
        int rightHandDef = 0;
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetDexterity + P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetDexterity + P.RightHand.GetLuck;
        defense += leftHandDef;
        defense += rightHandDef;
        return new AttackResult(damage, defense);
    }
    public override AttackResult VisitOther()
    {
        int damage = 0;
        int defense = P.Dexterity;
        
        int leftHandDef = 0;
        int rightHandDef = 0;
        
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetDexterity;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetDexterity;
        
        defense += leftHandDef;
        defense += rightHandDef;
        
        return new AttackResult(damage, defense);
    }
}