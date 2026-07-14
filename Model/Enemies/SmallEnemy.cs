using PROJ.Enemies.Species;

namespace PROJ.Enemies;

public class SmallEnemy : Enemy
{
    public SmallEnemy(Board b,int health = 30, int armor = 5, int damage = 15,string name = "Rat", char vis = '~', SpeciesGroup? group = null) : base(b,health, armor, damage, vis, name, group)
    {
    }

    public override string Description =>
        "Small creature, but one bite might get you infected.";
}