using PROJ.Builder.Classes;
using PROJ.Communication.Results;
using PROJ.Enemies;
using PROJ.Fight;
using PROJ.GameConstansts;
using PROJ.Logging.Classes;
using PROJ.Themes;
using PROJ.Tools.Classes;

namespace PROJ;
using System;

public class Board
{

    public Tile[,] Tiles;
    
    private Player _player;
    
    private int _currentlySeeked;
    private readonly List<Player> players = new();
    private readonly Dictionary<Player, int> seekByPlayer = new();
    private readonly Dictionary<Player, FightState> fightsByPlayer = new();


    private List<Enemy> enemies = new();
    private readonly Random enemyRandom = new();
    private const int EnemySightRange = 6;

    private sealed class FightState
    {
        public FightState(FightMenu menu, int enemyRow, int enemyColumn)
        {
            Menu = menu;
            EnemyRow = enemyRow;
            EnemyColumn = enemyColumn;
        }

        public FightMenu Menu { get; }
        public int EnemyRow { get; }
        public int EnemyColumn { get; }
    }
    


    private static readonly (int X, int Y)[] MovementDirections =
    {
        (0, -1),
        (1, 0),
        (0, 1),
        (-1, 0)
    };

    private static bool IsInsideWalkableArea(int x, int y)
    {
        return x > 0 && x < GameConstants.Width - 1 && y > 0 && y < GameConstants.Height - 1;
    }

    private bool CanMove(int x, int y)
    {
        if (!IsInsideWalkableArea(x, y))
            return false;
        return !Tiles[y, x].BlocksMovement && !IsOccupiedByPlayer(x, y, _player);
    }

    public (int X, int Y)? GetNextStepOnShortestPath(int startX, int startY, int endX, int endY)
    {
        if (Tiles == null)
            return null;
        if (!IsInsideWalkableArea(startX, startY) || !CanMove(endX, endY))
            return null;
        if (startX == endX && startY == endY)
            return null;

        var start = (X: startX, Y: startY);
        var end = (X: endX, Y: endY);
        return GetNextStepToAnyTarget(start, new HashSet<(int X, int Y)> { end }, CanMove);
    }

    private bool CanEnemyMoveTo(int x, int y)
    {
        if (!IsInsideWalkableArea(x, y))
            return false;

        return Tiles[y, x].IsEmpty && !IsOccupiedByPlayer(x, y);
    }

    public (int X, int Y)? GetBestEnemyEscapeDestination(int startX, int startY, int threatX, int threatY)
    {
        int currentDistance = ManhattanDistance(startX, startY, threatX, threatY);
        int bestDistance = currentDistance;
        (int X, int Y)? bestDestination = null;

        foreach (var direction in MovementDirections)
        {
            int nextX = startX + direction.X;
            int nextY = startY + direction.Y;

            if (!CanEnemyMoveTo(nextX, nextY))
                continue;

            int distance = ManhattanDistance(nextX, nextY, threatX, threatY);
            if (distance <= bestDistance)
                continue;

            bestDistance = distance;
            bestDestination = (nextX, nextY);
        }

        return bestDestination;
    }

    public Player? GetNearestVisiblePlayer(Enemy enemy)
    {
        Player? nearest = null;
        int nearestDistance = int.MaxValue;

        foreach (var direction in MovementDirections)
        {
            for (int distance = 1; distance <= EnemySightRange; distance++)
            {
                int x = enemy.BoardX + direction.X * distance;
                int y = enemy.BoardY + direction.Y * distance;

                if (!IsInsideWalkableArea(x, y))
                    break;
                if (Tiles[y, x].BlocksMovement)
                    break;

                Player? player = GetPlayerAt(x, y);
                if (player == null || distance >= nearestDistance)
                    continue;

                nearest = player;
                nearestDistance = distance;
                break;
            }
        }

        return nearest;
    }

    private (int X, int Y)? GetNextEnemyStepToDestination(Enemy enemy)
    {
        if (enemy.Destination == null)
            return null;

        var start = (X: enemy.BoardX, Y: enemy.BoardY);
        var destination = enemy.Destination.Value;

        if (!IsInsideWalkableArea(destination.X, destination.Y))
            return null;
        if (start == destination)
            return null;

        HashSet<(int X, int Y)> targets = new();
        if (CanEnemyMoveTo(destination.X, destination.Y))
        {
            targets.Add(destination);
        }
        else
        {
            foreach (var direction in MovementDirections)
            {
                var candidate = (X: destination.X + direction.X, Y: destination.Y + direction.Y);

                if (candidate == start)
                    return null;
                if (CanEnemyMoveTo(candidate.X, candidate.Y))
                    targets.Add(candidate);
            }
        }

        if (targets.Count == 0)
            return null;

        return GetNextStepToAnyTarget(start, targets, CanEnemyMoveTo);
    }

    private static int ManhattanDistance(int firstX, int firstY, int secondX, int secondY)
    {
        return Math.Abs(firstX - secondX) + Math.Abs(firstY - secondY);
    }

    private (int X, int Y)? GetNextStepToAnyTarget(
        (int X, int Y) start,
        HashSet<(int X, int Y)> targets,
        Func<int, int, bool> canEnter)
    {
        Queue<(int X, int Y)> queue = new();
        Dictionary<(int X, int Y), (int X, int Y)> previous = new();

        queue.Enqueue(start);
        previous[start] = start;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (targets.Contains(current))
                return GetFirstStep(start, current, previous);

            foreach (var direction in MovementDirections)
            {
                var next = (X: current.X + direction.X, Y: current.Y + direction.Y);

                if (previous.ContainsKey(next) || !canEnter(next.X, next.Y))
                    continue;

                previous[next] = current;
                queue.Enqueue(next);
            }
        }

        return null;
    }
    
    private static (int X, int Y)? GetFirstStep(
        (int X, int Y) start,
        (int X, int Y) end,
        Dictionary<(int X, int Y), (int X, int Y)> previous)
    {
        if (start == end)
            return null;

        var step = end;
        while (previous[step] != start)
        {
            step = previous[step];
        }

        return step;
    }

    private int CurrentSeek
    {
        get
        {
            if (!seekByPlayer.TryGetValue(_player, out int seek))
            {
                seekByPlayer[_player] = 0;
                return 0;
            }

            return seek;
        }
        set
        {
            int seek = Math.Max(0, value);
            seekByPlayer[_player] = seek;
            _currentlySeeked = seek;
        }
    }

    private FightState? CurrentFightState
    {
        get
        {
            return fightsByPlayer.TryGetValue(_player, out FightState? state) ? state : null;
        }
    }

    private Player? GetPlayerAt(int x, int y)
    {
        return players.FirstOrDefault(player =>
            !player.Dead() &&
            player.Position[0] == x &&
            player.Position[1] == y);
    }

    private bool IsOccupiedByPlayer(int x, int y, Player? except = null)
    {
        return players.Any(player =>
            !ReferenceEquals(player, except) &&
            !player.Dead() &&
            player.Position[0] == x &&
            player.Position[1] == y);
    }

    private string GetBlockedByName(int x, int y)
    {
        Player? player = GetPlayerAt(x, y);
        if (player != null && !ReferenceEquals(player, _player))
            return player.Name;

        Tile tile = Tiles[y, x];
        if (tile.Content != null && tile.Content.Count > 0)
            return tile.Content[0].Name;

        return "blocked tile";
    }

    private (int X, int Y) FindSpawnPosition()
    {
        if (Tiles == null)
            return (1, 1);

        for (int y = 1; y < GameConstants.Height - 1; y++)
        {
            for (int x = 1; x < GameConstants.Width - 1; x++)
            {
                if (!Tiles[y, x].BlocksMovement && !IsOccupiedByPlayer(x, y))
                    return (x, y);
            }
        }

        return (1, 1);
    }

    public char GetVisualAt(int x, int y)
    {
        Player? player = GetPlayerAt(x, y);
        if (player != null)
            return player.MapSymbol;

        if (Tiles[y, x].IsEmpty)
            return ' ';
        if (Tiles[y, x].Content != null &&
            Tiles[y, x].Objects > 0) // TODO pusibul nul - teorytycznie miedzy linijkami może się zmienić
            return Tiles[y, x].GetVisual();

        return ' ';
    }

    public TileViewResult CreateTileViewResult(int row, int column)
    {
        return new TileViewResult(row, column, GetVisualAt(column, row), Tiles[row, column].IsEmpty && !IsOccupiedByPlayer(column, row));
    }

    public TileChangeResult CreateTileChangeResult(int row, int column)
    {
        TileViewResult tile = CreateTileViewResult(row, column);
        return new TileChangeResult(tile.Row, tile.Column, tile.Visual, tile.IsEmpty);
    }

    public BoardSnapshotResult CreateSnapshotResult()
    {
        List<TileViewResult> tiles = new();

        for (int row = 0; row < GameConstants.Height; row++)
        {
            for (int column = 0; column < GameConstants.Width; column++)
            {
                tiles.Add(CreateTileViewResult(row, column));
            }
        }

        return new BoardSnapshotResult(tiles);
    }

    public ActionBoxResult CreateActionBoxResult(int row, int column, int seek = 0)
    {
        Tile tile = Tiles[row, column];
        List<BoardObjectViewResult> objects = new();

        if (tile.Content != null)
        {
            foreach (BoardObject obj in tile.Content)
            {
                objects.Add(new BoardObjectViewResult(obj.Name, obj.Description, obj.Pickupable));
            }
        }

        int selected = objects.Count == 0 ? 0 : Math.Clamp(seek, 0, objects.Count - 1);
        return new ActionBoxResult(objects, selected);
    }

    public ActionBoxResult CreatePlayerActionBoxResult(int seek = -1)
    {
        int column = _player.Position[0];
        int row = _player.Position[1];
        return CreateActionBoxResult(row, column, seek >= 0 ? seek : CurrentSeek);
    }

    public EnemyViewResult? CreateNearbyEnemyResult()
    {
        Enemy? enemy = GetNearestEnemy();
        return enemy != null ? CreateEnemyViewResult(enemy) : null;
    }

    public PlayerFightViewResult CreatePlayerFightResult()
    {
        return new PlayerFightViewResult(
            _player.LeftHand != null ? _player.LeftHand.Visual : GameConstants.EmptyHandSymbol,
            _player.RightHand != null ? _player.RightHand.Visual : GameConstants.EmptyHandSymbol);
    }

    public bool IsFightActive => CurrentFightState != null;

    private static EnemyViewResult CreateEnemyViewResult(Enemy enemy)
    {
        return new EnemyViewResult(enemy.Name, enemy.Description, enemy.Fightable, enemy.Visual, enemy.Health, enemy.Armor);
    }

    public TileBlinkResult? CreateTileBlinkResult(int row, int column, ConsoleColor color)
    {
        if (row < 0 || row >= Tiles.GetLength(0) || column < 0 || column >= Tiles.GetLength(1))
            return null;

        return new TileBlinkResult(CreateTileViewResult(row, column), color);
    }

    private List<MoveResult> PrepareMoveResults(List<MoveResult> results)
    {
        foreach (MoveResult result in results)
        {
            if (!result._success)
                continue;

            result.SetRenderedTiles(
                CreateTileViewResult(result.from_y, result.from_x),
                CreateTileViewResult(result.to_y, result.to_x));
        }

        if (results.Count > 0)
        {
            results[0].SetViewContext(
                results[0]._success ? CreatePlayerActionBoxResult() : null,
                CreateNearbyEnemyResult());
        }

        return results;
    }

    public void InsertEnemies(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }
    public void Init(Tile[,] tiles)
    {
        Tiles = tiles;
    }
    

    public TileChangeResult RemoveFromMap(int x, int y)
    {
        Tiles[x,y].Remove(CurrentSeek);
        return CreateTileChangeResult(x,y);
    }
    public Board() // problem - potrzebuje playera do wywolania generate - Pamietac ze miedzy konstruktorem a generate musi byc player assignment
    {
        _currentlySeeked = 0;
    }

    public Player GetPlayer => _player;

    // private void CentralHallWithLootgenerate(DungeonBuilder builder)
    // {
    //     builder.CreateEmptyDungeon();
    //     builder.AddCentralHall(12,13);
    //     builder.AddWeapons(30);
    //     builder.AddItems(20);
    // }
    //
    // private void ManySmallRoomsWithLootGenerate(DungeonBuilder builder)
    // {
    //     builder.CreateEmptyDungeon();
    //     // builder.AddCentralHall(12,13);
    //     builder.AddRooms(30);
    //     builder.AddWeapons(30);
    //     builder.AddItems(20);
    // }

    // private void OnlyMazeGenerate(DungeonBuilder builder)
    // {
    //     builder.CreateFilledDungeon();
    //     // builder.AddCorridors(10);
    // }
    //
    // private void MixGenerate(DungeonBuilder builder)
    // {
    //     builder.CreateEmptyDungeon();
    //     // builder.CreateFilledDungeon();
    //     builder.AddCentralHall(10,7);
    //     builder.AddRooms(10);
    //     builder.AddCorridors(10);
    //     builder.AddWeapons(20);
    //     builder.AddItems(10);
    //     builder.AddEnemies(5);
    // }
    //
    
    



    public BoardObject? GetCurrentlySeeked()
    {
        var tiles = Tiles[_player.Position[1], _player.Position[0]].Content;
        int currentSeek = CurrentSeek;
        if (tiles == null || Tiles[_player.Position[1], _player.Position[0]].Objects <= currentSeek)
            return null;
        return tiles[currentSeek];
    }

    public void ResetSeek()
    {
        CurrentSeek = 0;
    }

    public int TryIncreaseSeek()
    {
        int currentSeek = CurrentSeek;
        if (currentSeek < Tiles[_player.Position[1], _player.Position[0]].Objects - 1)
        {
            CurrentSeek = currentSeek + 1;
        }
        return CurrentSeek;
    }
    
    public int TryDecreaseSeek()
    {
        int currentSeek = CurrentSeek;
        if (currentSeek > 0)
            CurrentSeek = currentSeek - 1;
        return CurrentSeek;
    }

    public void AddPlayer(Player player, string? playerName = null, int playerId = 0)
    {
        if (!players.Contains(player))
            players.Add(player);

        _player = player;
        player.AssignBoard(this);
        player.NetworkId = playerId;
        player.Name = playerName != null ? playerName : playerId > 0 ? $"Player {playerId}" : "Player 1";

        var spawn = FindSpawnPosition();
        player.Position[0] = spawn.X;
        player.Position[1] = spawn.Y;
        CurrentSeek = 0;
    }

    public void SetActivePlayer(Player player)
    {
        if (!players.Contains(player))
            AddPlayer(player);

        _player = player;
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
        seekByPlayer.Remove(player);
        fightsByPlayer.Remove(player);

        if (ReferenceEquals(_player, player) && players.Count > 0)
            _player = players[0];
    }
    
    public List<MoveResult> MoveRight()
    {
        int x = _player.Position[0];
        int y = _player.Position[1];
        int newX = x + 1;
        int newY = y;
        var logger = Logger.GetInstance;


        if (!CanMove(newX, newY))
        {
            logger.Log($"- {_player.Name} Tried To Walk into {GetBlockedByName(newX, newY)} at {newX}, {newY}");
            var list = EnemiesTurn();
            return PrepareMoveResults(new List<MoveResult> { new(x, y, newX, newY, false) }.Concat(list).ToList());
        }
            
        _player.Position[0] = newX;
        _player.Position[1] = newY;
        var mr = new MoveResult(x, y, newX, newY, true);
        var listAfterMove = EnemiesTurn();
        return PrepareMoveResults(new List<MoveResult> { mr }.Concat(listAfterMove).ToList());
    }
    public List<MoveResult> MoveLeft()
    {
        int x = _player.Position[0];
        int y = _player.Position[1];
        int newX = x - 1;
        int newY = y;
        
        var logger = Logger.GetInstance;


        if (!CanMove(newX, newY))
        {
            logger.Log($"- {_player.Name} Tried To Walk into {GetBlockedByName(newX, newY)} at {newX}, {newY}");
            var list = EnemiesTurn();
            return PrepareMoveResults(new List<MoveResult> { new(x, y, newX, newY, false) }.Concat(list).ToList());
        }
            
        _player.Position[0] = newX;
        _player.Position[1] = newY;
        var mr = new MoveResult(x, y, newX, newY, true);
        var listAfterMove = EnemiesTurn();
        return PrepareMoveResults(new List<MoveResult> { mr }.Concat(listAfterMove).ToList());
    }
    public List<MoveResult> MoveDown()
    {
        int x = _player.Position[0];
        int y = _player.Position[1];
        int newX = x;
        int newY = y + 1;

        var logger = Logger.GetInstance;


        if (!CanMove(newX, newY))
        {
            logger.Log($"- {_player.Name} Tried To Walk into {GetBlockedByName(newX, newY)} at {newX}, {newY}");
            var list = EnemiesTurn();
            return PrepareMoveResults(new List<MoveResult> { new(x, y, newX, newY, false) }.Concat(list).ToList());
        }
        _player.Position[0] = newX;
        _player.Position[1] = newY;
        var mr = new MoveResult(x, y, newX, newY, true);
        var listAfterMove = EnemiesTurn();
        return PrepareMoveResults(new List<MoveResult> { mr }.Concat(listAfterMove).ToList());
    }
    public List<MoveResult> MoveUp()
    {
        int x = _player.Position[0];
        int y = _player.Position[1];
        int newX = x;
        int newY = y - 1;

        var logger = Logger.GetInstance;
        
        
        if (!CanMove(newX, newY))
        {
            logger.Log($"- {_player.Name} Tried To Walk into {GetBlockedByName(newX, newY)} at {newX}, {newY}");
            var l = EnemiesTurn();
            return PrepareMoveResults(new List<MoveResult>{new (x,y,newX,newY,false)}.Concat(l).ToList());
        }

        _player.Position[0] = newX;
        _player.Position[1] = newY;
        
        var mr = new MoveResult(x,y,newX,newY, true);
        // _actionBox.AfterMoveAsessment(Tiles[_player.Position[1], _player.Position[0]].Content,Tiles[_player.Position[1], _player.Position[0]].Objects );
        var list = EnemiesTurn();
        // _fightBox.AfterMoveAssesment(GetNearestEnemy());
        return PrepareMoveResults(new List<MoveResult>{mr}.Concat(list).ToList());
    }
    
 
    // public void RefreshActionBox(int x, int y)
    // {
    //     _actionBox.AfterMoveAsessment(Tiles[x,y].Content,Tiles[x,y].Objects); 
    // }
    //
    // public void RefreshActionBox()
    // {
    //     int x = _player.Position[1];
    //     int y = _player.Position[0];
    //     _actionBox.AfterMoveAsessment(Tiles[x,y].Content,Tiles[x,y].Objects, _currentlySeeked); 
    // }

    public void DropItem(Tool tool)
    {
        int x = _player.Position[1];
        int y = _player.Position[0];
        tool.X = x;
        tool.Y = y;
        tool.ObjBoard = this;
        Tiles[x,y].AddObj(tool);
    }

    private bool IsEnemy(int x, int y)
    {
        if (Tiles[y, x].Objects > 0 && Tiles[y, x].Content[0].Fightable) // TODO WIEM WIEM WIEM KURCZe
            return true;
        return false;
    }
    

    public Enemy? GetNearestEnemy()
    {
        int x = _player.Position[0];
        int y = _player.Position[1];
        if (y> 1 && IsEnemy(x, y - 1))
            return (Enemy)Tiles[y-1,x].Content[0];
        if (y < GameConstants.Height - 2 && IsEnemy(x, y+1))
            return (Enemy)Tiles[y+1,x].Content[0];
        if (x > 0 && IsEnemy(x-1, y))
            return (Enemy)Tiles[y,x-1].Content[0];
        if (x < GameConstants.Width - 2 && IsEnemy(x + 1, y))
            return (Enemy)Tiles[y, x + 1].Content[0];
        return null;
    }

    public FightStartResult FightNearestEnemy()
    {
        if (_player.Position[1] > 1 && IsEnemy(_player.Position[0], _player.Position[1] - 1))
        {
            return BeginFight(_player.Position[1] - 1, _player.Position[0]);
        }

        if (_player.Position[1] < GameConstants.Height - 2 && IsEnemy(_player.Position[0], _player.Position[1] + 1))
        {
            return BeginFight(_player.Position[1] + 1, _player.Position[0]);
        }

        if (_player.Position[0] > 0 && IsEnemy(_player.Position[0] - 1, _player.Position[1]))
        {
            return BeginFight(_player.Position[1], _player.Position[0] - 1);
        }

        if (_player.Position[0] < GameConstants.Width - 2 && IsEnemy(_player.Position[0] + 1, _player.Position[1]))
        {
            return BeginFight(_player.Position[1], _player.Position[0] + 1);
        }

        return new FightStartResult(false, errorMessage: "No Enemies Nearby to fight");
    }

    public FightLoopEndResult RunCurrentFight()
    {
        FightState? fightState = CurrentFightState;
        if (fightState == null)
        {
            return new FightLoopEndResult(false, false, false);
        }

        FightLoopEndResult result = fightState.Menu.StartFight();

        if (result.EnemyKilled)
        {
            Tiles[fightState.EnemyRow, fightState.EnemyColumn].Reset();
            result = new FightLoopEndResult(true, result.ExitedByPlayer, result.PlayerDied,
                CreateTileChangeResult(fightState.EnemyRow, fightState.EnemyColumn));
        }

        fightsByPlayer.Remove(_player);
        return result;
    }

    private FightStartResult BeginFight(int enemyRow, int enemyColumn)
    {
        bool enemyAlreadyEngaged = fightsByPlayer.Values.Any(fight =>
            fight.EnemyRow == enemyRow && fight.EnemyColumn == enemyColumn);
        if (enemyAlreadyEngaged)
            return new FightStartResult(false, errorMessage: "Enemy already fighting another player");

        Enemy? enemy = Tiles[enemyRow, enemyColumn].TryGetEnemy();
        if (enemy == null)
        {
            return new FightStartResult(false, errorMessage: "No Enemies Nearby to fight");
        }

        fightsByPlayer[_player] = new FightState(new FightMenu(_player, enemy), enemyRow, enemyColumn);
        return new FightStartResult(true, CreateEnemyViewResult(enemy), CreatePlayerFightResult());
    }

    public EnemyViewResult? CreateCurrentFightEnemyResult()
    {
        FightState? fightState = CurrentFightState;
        if (fightState == null)
            return null;

        Enemy? enemy = Tiles[fightState.EnemyRow, fightState.EnemyColumn].TryGetEnemy();
        return enemy != null ? CreateEnemyViewResult(enemy) : null;
    }

    public FightAttackSelectionResult? SelectCurrentFightAttack(int attack)
    {
        return CurrentFightState?.Menu.SetAttack(attack);
    }

    public FightHandSelectionResult? SelectCurrentFightHand(char hand)
    {
        return CurrentFightState?.Menu.SetHand(hand);
    }

    public int? CurrentFightAttack => CurrentFightState?.Menu.CurrentAttack;

    public char? CurrentFightHand => CurrentFightState?.Menu.CurrentHand;

    public FightExitResult? ExitCurrentFight()
    {
        FightState? fightState = CurrentFightState;
        if (fightState == null)
            return null;

        FightExitResult result = fightState.Menu.ExitFight();
        fightsByPlayer.Remove(_player);
        return result;
    }

    public FightTurnResult? SimulateCurrentFightAttack()
    {
        FightState? fightState = CurrentFightState;
        if (fightState == null)
            return null;

        FightTurnResult result = fightState.Menu.SimulateAttack();

        if (result.EnemyDead)
        {
            Tiles[fightState.EnemyRow, fightState.EnemyColumn].Reset();
            fightsByPlayer.Remove(_player);
        }

        if (result.PlayerDead || result.ExitFightMode)
            fightsByPlayer.Remove(_player);

        return result;
    }
    
    
    

    private MoveResult? EnemyMove(Enemy e)
    {
        Player? adjacentPlayer = GetAdjacentActivePlayer(e);
        if (adjacentPlayer != null)
            return EnemyAttack(e, adjacentPlayer);

        MoveResult? destinationMove = MoveEnemyTowardsDestination(e);
        if (destinationMove != null)
        {
            adjacentPlayer = GetAdjacentActivePlayer(e);
            if (adjacentPlayer != null)
                ApplyEnemyAttack(e, adjacentPlayer, destinationMove);

            return destinationMove;
        }

        if (destinationMove != null || e.Destination != null)
            return destinationMove;

        MoveResult? randomMove = RandomEnemyMove(e);
        if (randomMove != null)
        {
            adjacentPlayer = GetAdjacentActivePlayer(e);
            if (adjacentPlayer != null)
                ApplyEnemyAttack(e, adjacentPlayer, randomMove);
        }

        return randomMove;
    }

    private MoveResult? MoveEnemyTowardsDestination(Enemy e)
    {
        if (e.Destination == null)
            return null;

        var nextStep = GetNextEnemyStepToDestination(e);
        if (nextStep == null)
        {
            if (IsEnemyAdjacentToDestination(e))
                return null;

            e.ClearDestination();
            return null;
        }

        return MoveEnemyTo(e, nextStep.Value.Y, nextStep.Value.X);
    }

    private MoveResult? RandomEnemyMove(Enemy e)
    {
        int x = enemyRandom.Next(0, 100);
        if (x < 33)
            return null;

        var direction = MovementDirections[enemyRandom.Next(0, MovementDirections.Length)];
        return MoveEnemyTo(e, e.X + direction.Y, e.Y + direction.X);
    }

    private MoveResult? MoveEnemyTo(Enemy e, int newRow, int newColumn)
    {
        if (!CanEnemyMoveTo(newColumn, newRow))
            return null;

        int oldRow = e.X;
        int oldColumn = e.Y;
        Tiles[oldRow, oldColumn].Reset();

        e.SetDirection(newColumn - oldColumn, newRow - oldRow);
        e.X = newRow;
        e.Y = newColumn;
        Tiles[e.X, e.Y].AddObj(e);

        e.ClearSoundIfSourceReached();
        if (e.Destination != null && e.Destination.Value.X == e.BoardX && e.Destination.Value.Y == e.BoardY)
            e.ClearDestination();

        return new MoveResult(oldColumn, oldRow, e.Y, e.X, true);
    }

    private Player? GetAdjacentActivePlayer(Enemy e)
    {
        if (_player.Dead())
            return null;

        foreach (var direction in MovementDirections)
        {
            int x = e.BoardX + direction.X;
            int y = e.BoardY + direction.Y;

            if (_player.Position[0] == x && _player.Position[1] == y)
                return _player;
        }

        return null;
    }

    private MoveResult EnemyAttack(Enemy e, Player player)
    {
        MoveResult result = new MoveResult(e.Y, e.X, e.Y, e.X, false);
        ApplyEnemyAttack(e, player, result);
        return result;
    }

    private static void ApplyEnemyAttack(Enemy e, Player player, MoveResult result)
    {
        e.Attack(player, 0);
        Logger.GetInstance.Log($"{e.Name} Attacked {player.Name} with {e.Damage} damage");
        result.SetPlayerDamageContext(e.Damage, player.Dead());
    }

    private static bool IsEnemyAdjacentToDestination(Enemy e)
    {
        if (e.Destination == null)
            return false;

        var destination = e.Destination.Value;
        return ManhattanDistance(e.BoardX, e.BoardY, destination.X, destination.Y) == 1;
    }

    private List<MoveResult> EnemiesTurn()
    {
        var list = new List<MoveResult>();
        foreach (var enemy in enemies)
        {
            enemy.RefreshDestination();
            var res = EnemyMove(enemy);
            if(res != null)
                list.Add(res);
        }
        return list;
    }

    public void DeleteEnemy(Enemy e)
    {
        enemies.Remove(e);
    }
    
    
}
