namespace PROJ.Communication.Results;

public class BackpackDropResult
{
    private readonly bool _isSuccess;
    private readonly int _backpackIdx;
    private readonly int _X;
    private readonly int _Y;
    private readonly TileChangeResult? _tileChangeResult;

    public BackpackDropResult(bool isSuccess, int backpackIdx, int x, int y, TileChangeResult? tileChangeResult = null)
    {
        _isSuccess = isSuccess;
        _backpackIdx = backpackIdx;
        _X = x;
        _Y = y;
        _tileChangeResult = tileChangeResult;
    }
    
    public bool IsSuccess
    {
        get { return _isSuccess; }
    }
    public int BackpackIdx
    {
        get { return _backpackIdx; }
    }
    public int X => _X;
    public int Y => _Y;
    public TileChangeResult? TileChangeResult => _tileChangeResult;
}
