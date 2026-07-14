namespace PROJ.Communication.Snapshots;

public sealed class GameSnapshotMessage
{
    public string Type { get; set; } = "game_snapshot";
    public string[] Board { get; set; } = Array.Empty<string>();
    public PlayerSnapshot Player { get; set; } = new();
    public ActionBoxSnapshot? ActionBox { get; set; }
    public ErrorSnapshot Errors { get; set; } = new();
    public LoggerSnapshot Logger { get; set; } = new();
    public VisualEffectsSnapshot Effects { get; set; } = new();
    public PROJ.Communication.Results.EnemyViewResult? NearbyEnemy { get; set; }
    public FightSnapshot? Fight { get; set; }
    public bool GameEnded { get; set; }
    public bool EndedGood { get; set; }
}

public sealed class ErrorSnapshot
{
    public string? ActionError { get; set; }
    public string? FightError { get; set; }
}

public sealed class LoggerSnapshot
{
    public List<string> VisibleLines { get; set; } = new();
    public bool IsInLoggerMode { get; set; }
}

public sealed class VisualEffectsSnapshot
{
    public List<TileEffectSnapshot> TileEffects { get; set; } = new();
}

public sealed class TileEffectSnapshot
{
    public string Kind { get; set; } = string.Empty;
    public TileSnapshot Tile { get; set; } = new();
    public ConsoleColor Color { get; set; }
    public int DelayMs { get; set; }
}

public sealed class TileSnapshot
{
    public int Row { get; set; }
    public int Column { get; set; }
    public char Visual { get; set; }
    public bool IsEmpty { get; set; }
}

public sealed class ActionBoxSnapshot
{
    public List<BoardObjectSnapshot> Objects { get; set; } = new();
    public int Seek { get; set; }
}

public sealed class BoardObjectSnapshot
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Pickupable { get; set; }
}

public sealed class PlayerSnapshot
{
    public double Level { get; set; }
    public int Dexterity { get; set; }
    public int Health { get; set; }
    public int Luck { get; set; }
    public int Strength { get; set; }
    public int Wisdom { get; set; }
    public int Gold { get; set; }
    public int Coins { get; set; }
    public bool IsInBackpack { get; set; }
    public int BackpackIndex { get; set; }
    public List<ItemSnapshot?> Backpack { get; set; } = new();
    public ItemSnapshot? LeftHand { get; set; }
    public ItemSnapshot? RightHand { get; set; }
}

public sealed class ItemSnapshot
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public char Visual { get; set; }
    public int Space { get; set; }
}

public sealed class FightSnapshot
{
    public PROJ.Communication.Results.EnemyViewResult Enemy { get; set; } = null!;
    public PROJ.Communication.Results.PlayerFightViewResult Player { get; set; } = null!;
    public int? SelectedAttack { get; set; }
    public char? SelectedHand { get; set; }
}
