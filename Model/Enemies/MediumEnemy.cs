using PROJ.Enemies.Species;

namespace PROJ.Enemies;

public class MediumEnemy : Enemy
{
    public MediumEnemy(Board b,int health = 100, int armor = 5, int damage = 15,string name = "Infected", char vis = '☠', SpeciesGroup? group = null) : base(b,health, armor, damage, vis, name, group)
    {
    }
    public override string Description => "Once a regular man turned zombie-like creature";
}