namespace PROJ.Communication.Results;

public class PickUpResult
{
    private MessageBusResult? _result;
    private bool _isSuccess;
    private TileChangeResult? _tileChangeResult;
    private string? _errormsg;
    private ActionBoxResult? _actionBoxResult;
    
    public PickUpResult(bool isSuccess, MessageBusResult? result, TileChangeResult? tileChangeResult, string? errormsg = null, ActionBoxResult? actionBoxResult = null)
    {
        _isSuccess = isSuccess;
        _result = result;
        _tileChangeResult = tileChangeResult;
        _errormsg = errormsg;
        _actionBoxResult = actionBoxResult;
    }
    
    public bool IsSuccess => _isSuccess;
    public MessageBusResult? Result => _result;
    public TileChangeResult? TileChangeResult => _tileChangeResult;
    public string? Errormsg => _errormsg;
    public ActionBoxResult? ActionBoxResult => _actionBoxResult;

    public void SetActionBoxResult(ActionBoxResult? actionBoxResult)
    {
        _actionBoxResult = actionBoxResult;
    }
}
