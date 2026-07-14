namespace PROJ.Communication.Results;

public class FightLoopEndResult
{
    private readonly bool _enemyKilled;
    private readonly bool _exitedByPlayer;
    private readonly bool _playerDied;
    private readonly TileChangeResult? _tileChangeResult;

    public FightLoopEndResult(bool enemyKilled, bool exitedByPlayer, bool playerDied, TileChangeResult? tileChangeResult = null)
    {
        _enemyKilled = enemyKilled;
        _exitedByPlayer = exitedByPlayer;
        _playerDied = playerDied;
        _tileChangeResult = tileChangeResult;
    }

    public bool EnemyKilled => _enemyKilled;
    public bool ExitedByPlayer => _exitedByPlayer;
    public bool PlayerDied => _playerDied;
    public TileChangeResult? TileChangeResult => _tileChangeResult;
}
