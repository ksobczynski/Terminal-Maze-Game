using PROJ.Enemies.Species;

namespace PROJ.Enemies;

public class BigEnemy : Enemy
{
    public BigEnemy(Board b, int health = 200, int armor = 30, int damage = 20, string name = "Guard", char vis = '⛨', SpeciesGroup? group = null) : base(b, health, armor, damage, vis, name, group)
    {
    }

    public override string Description =>
        "One of the oligarch's fighters whose sole purpose is to make sure you don't get the medicine";
    
}