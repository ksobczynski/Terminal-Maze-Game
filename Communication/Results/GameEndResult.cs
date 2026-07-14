namespace PROJ.Communication.Results;

public class GameEndResult
{
    private readonly bool _endedGood;

    public GameEndResult(bool endedGood)
    {
        _endedGood = endedGood;
    }

    public bool EndedGood => _endedGood;
}
