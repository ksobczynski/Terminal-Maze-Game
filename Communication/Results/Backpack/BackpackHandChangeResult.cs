namespace PROJ.Communication.Results;

public class BackpackHandChangeResult
{
    private readonly bool _isSuccess;
    private readonly bool _refreshEquipment;
    private readonly bool _leavePointer;
    private readonly int _pointerIdx;
    private readonly bool _resetPointerToTop;
    private readonly bool _refreshLeftHand;
    private readonly bool _refreshRightHand;

    public BackpackHandChangeResult(
        bool isSuccess,
        bool refreshEquipment,
        bool leavePointer,
        int pointerIdx,
        bool resetPointerToTop,
        bool refreshLeftHand,
        bool refreshRightHand)
    {
        _isSuccess = isSuccess;
        _refreshEquipment = refreshEquipment;
        _leavePointer = leavePointer;
        _pointerIdx = pointerIdx;
        _resetPointerToTop = resetPointerToTop;
        _refreshLeftHand = refreshLeftHand;
        _refreshRightHand = refreshRightHand;
    }

    public bool IsSuccess => _isSuccess;
    public bool RefreshEquipment => _refreshEquipment;
    public bool LeavePointer => _leavePointer;
    public int PointerIdx => _pointerIdx;
    public bool ResetPointerToTop => _resetPointerToTop;
    public bool RefreshLeftHand => _refreshLeftHand;
    public bool RefreshRightHand => _refreshRightHand;
}
