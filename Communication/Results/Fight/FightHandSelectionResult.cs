namespace PROJ.Communication.Results;

public class FightHandSelectionResult
{
    private readonly char _hand;
    private readonly PlayerFightViewResult _player;

    public FightHandSelectionResult(char hand, PlayerFightViewResult player)
    {
        _hand = hand;
        _player = player;
    }

    public char Hand => _hand;
    public PlayerFightViewResult Player => _player;
}
