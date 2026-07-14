using PROJ.Fight.Visitors;
using PROJ.Tools.Classes.Weapons.Abstract_Types;

namespace PROJ.Fight.Classes.Visitors;

public class MagicAttackVisitor : AttackVisitor
{
    public MagicAttackVisitor(Player p) : base(p)
    {
    }

    public override AttackResult VisitLightWeapon(LightWeapon weapon)
    {
        int damage = 1;
        int defense = P.Luck;
        
        int leftHandDef = 0;
        int rightHandDef = 0;
        
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetLuck;
        
        defense += leftHandDef;
        defense += rightHandDef;
        
        return new AttackResult(damage, defense);
    }
    
    public override AttackResult VisitHeavyWeapon(HeavyWeapon weapon)
    {
        int damage = 1;
        int defense = P.Luck;
        
        int leftHandDef = 0;
        int rightHandDef = 0;
        
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetLuck;
        
        defense += leftHandDef;
        defense += rightHandDef;
        
        return new AttackResult(damage, defense);
    }
    
    public override AttackResult VisitMagicWeapon(MagicWeapon weapon)
    {
        int damage = weapon.Damage;
        int defense = P.Wisdom * 2;
        
        int leftHandDef = 0;
        int rightHandDef = 0;
        
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetWisdom;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetWisdom;
        
        defense += leftHandDef;
        defense += rightHandDef;
        return new AttackResult(damage, defense);
    }

    public override AttackResult VisitOther()
    {
        int damage = 0;
        int defense = P.Luck;
        
        int leftHandDef = 0;
        int rightHandDef = 0;
        
        if (P.LeftHand != null)
            leftHandDef = P.LeftHand.GetLuck;
        if (P.RightHand != null)
            rightHandDef = P.RightHand.GetLuck;
        
        defense += leftHandDef;
        defense += rightHandDef;
        
        return new AttackResult(damage, defense);
    }
}