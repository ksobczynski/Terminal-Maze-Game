using PROJ.Fight;
using PROJ.Fight.Interfaces;
using PROJ.GameConstansts;

namespace PROJ.Tools.Classes;

public abstract class Weapon : Tool
{
    public virtual int Damage => GameConstants.BaseDamage;
    // public abstract float Cooldown { get; }
    // public abstract int Range { get; }


    public Weapon(Player player, string name = "unnamed", char vis = 'X') : base(player, name, vis){}

    
    public abstract bool TwoHanded(); // TODO decyzja - size i twohandedness to to samo czy nie
    
    
}
