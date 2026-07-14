using PROJ.GameConstansts;
using PROJ.Communication.Results;
using PROJ.Logging.Classes;
using PROJ.Tools.Classes;

namespace PROJ.Communication.Snapshots;

public static class GameSnapshotFactory
{
    public static GameSnapshotMessage Create(
        Board board,
        Player player,
        bool gameEnded = false,
        bool endedGood = false,
        string? actionError = null,
        string? fightError = null,
        IReadOnlyList<GameOutput.RecordedTileEffect>? effects = null)
    {
        return new GameSnapshotMessage
        {
            Board = CreateBoardRows(board),
            Player = CreatePlayerSnapshot(player),
            ActionBox = CreateActionBoxSnapshot(board.CreatePlayerActionBoxResult()),
            Errors = new ErrorSnapshot
            {
                ActionError = actionError,
                FightError = fightError
            },
            Logger = CreateLoggerSnapshot(),
            Effects = CreateEffectsSnapshot(effects),
            NearbyEnemy = board.IsFightActive ? null : board.CreateNearbyEnemyResult(),
            Fight = CreateFightSnapshot(board),
            GameEnded = gameEnded,
            EndedGood = endedGood
        };
    }

    private static string[] CreateBoardRows(Board board)
    {
        string[] rows = new string[GameConstants.Height];

        for (int y = 0; y < GameConstants.Height; y++)
        {
            char[] row = new char[GameConstants.Width];

            for (int x = 0; x < GameConstants.Width; x++)
            {
                row[x] = board.GetVisualAt(x, y);
            }

            rows[y] = new string(row);
        }

        return rows;
    }

    private static PlayerSnapshot CreatePlayerSnapshot(Player player)
    {
        List<ItemSnapshot?> backpack = new();

        for (int i = 0; i < GameConstants.BackpackCapacity; i++)
        {
            Tool? tool = player.PlayerBackpack?.TryGetItem(i);
            backpack.Add(CreateItemSnapshot(tool));
        }

        return new PlayerSnapshot
        {
            Level = player.Level,
            Dexterity = player.Dexterity,
            Health = player.Health,
            Luck = player.Luck,
            Strength = player.Strength,
            Wisdom = player.Wisdom,
            Gold = player.Gold,
            Coins = player.Coins,
            IsInBackpack = player.IsInBackpack,
            BackpackIndex = player.BackpackIndex,
            Backpack = backpack,
            LeftHand = CreateItemSnapshot(player.LeftHand),
            RightHand = CreateItemSnapshot(player.RightHand)
        };
    }

    private static ActionBoxSnapshot CreateActionBoxSnapshot(ActionBoxResult result)
    {
        return new ActionBoxSnapshot
        {
            Seek = result.Seek,
            Objects = result.Objects
                .Select(obj => new BoardObjectSnapshot
                {
                    Name = obj.Name,
                    Description = obj.Description,
                    Pickupable = obj.Pickupable
                })
                .ToList()
        };
    }

    private static LoggerSnapshot CreateLoggerSnapshot()
    {
        Logger logger = Logger.GetInstance;
        return new LoggerSnapshot
        {
            VisibleLines = logger.GetVisibleLogs().ToList(),
            IsInLoggerMode = logger.LoggerMode
        };
    }

    private static VisualEffectsSnapshot CreateEffectsSnapshot(IReadOnlyList<GameOutput.RecordedTileEffect>? effects)
    {
        VisualEffectsSnapshot snapshot = new();
        if (effects == null)
            return snapshot;

        foreach (GameOutput.RecordedTileEffect effect in effects)
        {
            TileViewResult tile = effect.Result.Tile;
            snapshot.TileEffects.Add(new TileEffectSnapshot
            {
                Kind = effect.Kind,
                Tile = new TileSnapshot
                {
                    Row = tile.Row,
                    Column = tile.Column,
                    Visual = tile.Visual,
                    IsEmpty = tile.IsEmpty
                },
                Color = effect.Result.Color,
                DelayMs = effect.DelayMs
            });
        }

        return snapshot;
    }

    private static ItemSnapshot? CreateItemSnapshot(Tool? tool)
    {
        if (tool == null)
            return null;

        return new ItemSnapshot
        {
            Name = tool.Name,
            Description = tool.Description,
            Visual = tool.Visual,
            Space = tool.Space
        };
    }

    private static FightSnapshot? CreateFightSnapshot(Board board)
    {
        if (!board.IsFightActive)
            return null;

        var enemy = board.CreateCurrentFightEnemyResult();
        if (enemy == null)
            return null;

        return new FightSnapshot
        {
            Enemy = enemy,
            Player = board.CreatePlayerFightResult(),
            SelectedAttack = board.CurrentFightAttack,
            SelectedHand = board.CurrentFightHand
        };
    }
}
