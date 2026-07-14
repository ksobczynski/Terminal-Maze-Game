namespace PROJ.Communication.Results;

public class BackpackPointerMoveResult
{
    private readonly bool _isSuccess;
    private readonly int _backpackIdx;
    private readonly bool _moveUp;

    public BackpackPointerMoveResult(bool isSuccess, int backpackIdx, bool moveUp)
    {
        _isSuccess = isSuccess;
        _backpackIdx = backpackIdx;
        _moveUp = moveUp;
    }
    public bool IsSuccess => _isSuccess;
    public int BackpackIdx => _backpackIdx;
    public bool MoveUp => _moveUp;
}