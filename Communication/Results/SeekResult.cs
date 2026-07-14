namespace PROJ.Communication.Results;

public class SeekResult
{
    private readonly int _seek;
    private readonly ActionBoxResult? _actionBoxResult;

    public SeekResult(int seek, ActionBoxResult? actionBoxResult = null)
    {
        _seek = seek;
        _actionBoxResult = actionBoxResult;
    }

    public int Seek => _seek;
    public ActionBoxResult? ActionBoxResult => _actionBoxResult;
}
