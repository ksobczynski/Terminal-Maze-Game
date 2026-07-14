namespace PROJ.Communication.Results;

public class FightStartResult
{
    private readonly bool _isSuccess;
    private readonly EnemyViewResult? _enemy;
    private readonly PlayerFightViewResult? _player;
    private readonly string? _errorMessage;

    public FightStartResult(bool isSuccess, EnemyViewResult? enemy = null, PlayerFightViewResult? player = null, string? errorMessage = null)
    {
        _isSuccess = isSuccess;
        _enemy = enemy;
        _player = player;
        _errorMessage = errorMessage;
    }

    public bool IsSuccess => _isSuccess;
    public EnemyViewResult? Enemy => _enemy;
    public PlayerFightViewResult? Player => _player;
    public string? ErrorMessage => _errorMessage;
}
