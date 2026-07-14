namespace PROJ.Communication.Results;

public class FightTurnResult
{
    private readonly bool _isSuccess;
    private readonly int _attack;
    private readonly char _hand;
    private readonly EnemyViewResult _enemy;
    private readonly PlayerFightViewResult _player;
    private readonly string? _errorMessage;
    private readonly bool _updateEnemyVitals;
    private readonly bool _enemyDead;
    private readonly bool _refreshPlayerVitals;
    private readonly bool _playerDead;
    private readonly bool _exitFightMode;

    public FightTurnResult(
        bool isSuccess,
        int attack,
        char hand,
        EnemyViewResult enemy,
        PlayerFightViewResult player,
        string? errorMessage = null,
        bool updateEnemyVitals = false,
        bool enemyDead = false,
        bool refreshPlayerVitals = false,
        bool playerDead = false,
        bool exitFightMode = false)
    {
        _isSuccess = isSuccess;
        _attack = attack;
        _hand = hand;
        _enemy = enemy;
        _player = player;
        _errorMessage = errorMessage;
        _updateEnemyVitals = updateEnemyVitals;
        _enemyDead = enemyDead;
        _refreshPlayerVitals = refreshPlayerVitals;
        _playerDead = playerDead;
        _exitFightMode = exitFightMode;
    }

    public bool IsSuccess => _isSuccess;
    public int Attack => _attack;
    public char Hand => _hand;
    public EnemyViewResult Enemy => _enemy;
    public PlayerFightViewResult Player => _player;
    public string? ErrorMessage => _errorMessage;
    public bool UpdateEnemyVitals => _updateEnemyVitals;
    public bool EnemyDead => _enemyDead;
    public bool RefreshPlayerVitals => _refreshPlayerVitals;
    public bool PlayerDead => _playerDead;
    public bool ExitFightMode => _exitFightMode;
}
