namespace PROJ.Communication.Results;

public sealed class TileViewResult
{
    public TileViewResult(int row, int column, char visual, bool isEmpty)
    {
        Row = row;
        Column = column;
        Visual = visual;
        IsEmpty = isEmpty;
    }

    public int Row { get; }
    public int Column { get; }
    public char Visual { get; }
    public bool IsEmpty { get; }
}

public sealed class BoardSnapshotResult
{
    public BoardSnapshotResult(IReadOnlyList<TileViewResult> tiles)
    {
        Tiles = tiles;
    }

    public IReadOnlyList<TileViewResult> Tiles { get; }
}

public sealed class BoardObjectViewResult
{
    public BoardObjectViewResult(string name, string description, bool pickupable)
    {
        Name = name;
        Description = description;
        Pickupable = pickupable;
    }

    public string Name { get; }
    public string Description { get; }
    public bool Pickupable { get; }
}

public sealed class ActionBoxResult
{
    public ActionBoxResult(IReadOnlyList<BoardObjectViewResult> objects, int seek)
    {
        Objects = objects;
        Seek = seek;
    }

    public IReadOnlyList<BoardObjectViewResult> Objects { get; }
    public int Seek { get; }
}

public sealed class EnemyViewResult
{
    public EnemyViewResult(string name, string description, bool fightable, char visual, int health, int armor)
    {
        Name = name;
        Description = description;
        Fightable = fightable;
        Visual = visual;
        Health = health;
        Armor = armor;
    }

    public string Name { get; }
    public string Description { get; }
    public bool Fightable { get; }
    public char Visual { get; }
    public int Health { get; }
    public int Armor { get; }
}

public sealed class PlayerFightViewResult
{
    public PlayerFightViewResult(char leftHandVisual, char rightHandVisual)
    {
        LeftHandVisual = leftHandVisual;
        RightHandVisual = rightHandVisual;
    }

    public char LeftHandVisual { get; }
    public char RightHandVisual { get; }
}

public sealed class TileBlinkResult
{
    public TileBlinkResult(TileViewResult tile, ConsoleColor color)
    {
        Tile = tile;
        Color = color;
    }

    public TileViewResult Tile { get; }
    public ConsoleColor Color { get; }
}
