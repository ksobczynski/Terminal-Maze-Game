namespace PROJ.Fight;

public class AttackResult
{
    public int DamageToEnemy { get; }
    public int PlayerDefense { get; }

    public AttackResult(int damageToEnemy, int playerDefense)
    {
        DamageToEnemy = damageToEnemy;
        PlayerDefense = playerDefense;
    }
}