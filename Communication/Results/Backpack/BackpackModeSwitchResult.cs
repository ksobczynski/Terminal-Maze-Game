namespace PROJ.Communication.Results;

public class BackpackModeSwitchResult
{
    private readonly bool _inBp;
    private int _backpackIdx;
    public BackpackModeSwitchResult(bool inBp, int idx = 0)
    {
        _inBp = inBp;
        _backpackIdx = idx;
    }
    public bool InBp => _inBp;
    public int BackpackIdx => _backpackIdx;
}