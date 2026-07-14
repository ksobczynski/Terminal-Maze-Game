namespace PROJ.Communication.Results;

public class FightAttackSelectionResult
{
    private readonly int _attack;

    public FightAttackSelectionResult(int attack)
    {
        _attack = attack;
    }

    public int Attack => _attack;
}
