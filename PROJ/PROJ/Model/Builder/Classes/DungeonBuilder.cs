using PROJ.Enemies;
using PROJ.Enemies.Death_Reaction_Strategies;
using PROJ.Enemies.MovementStrategies;
using PROJ.Enemies.Species;
using PROJ.GameConstansts;
using PROJ.Tools.Classes;
using PROJ.Tools.Classes.Decorators;
using PROJ.Tools.Classes.Items;
using PROJ.Tools.Classes.Weapons;

namespace PROJ.Builder.Classes;

public class DungeonBuilder : IDungeonBuilder
{
    struct Room
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;
        public bool Connected;
    }
    

    struct Point
    {
        public int Left;
        public int Top;



        public Point(int l, int t)
        {
            Left = l;
            Top = t;
        }
    }

    private enum Direction
    {
        South = 0,
        North = 1,
        West = 2,
        East = 3,
    }

    private Tile[,] _tiles;
    private Board _board;
    private List<Room> _rooms;
    private bool _playerRoom;
    private Player _player;
    private IPlayerMovesBuilder _pmb;
    

    
    
    public DungeonBuilder(Board b, Player p, IPlayerMovesBuilder pmb)
    {
        _pmb = pmb;
        _player = p;
        _playerRoom = false;
        _board = b;
        _rooms = new List<Room>();
        _tiles = new Tile[GameConstants.Height, GameConstants.Width];
        for (int y = 0; y < GameConstants.Height; y++)
        {
            for (int x = 0; x < GameConstants.Width; x++)
            {
                _tiles[y, x] = new Tile();
            }
        }

        _tiles[0,0] = new Tile();
        _tiles[0, GameConstants.Width - 1] = new Tile();
        _tiles[GameConstants.Height - 1, 0] = new Tile();
        _tiles[GameConstants.Height - 1, GameConstants.Width - 1] = new Tile();
        
        
        _tiles[0,0].AddObj(new FrameObject('╔'));
        _tiles[0, GameConstants.Width - 1].AddObj(new FrameObject( '╗'));
        _tiles[GameConstants.Height - 1, 0].AddObj(new FrameObject('╚'));
        _tiles[GameConstants.Height - 1, GameConstants.Width - 1].AddObj(new FrameObject('╝'));
        

        

        for (int i = 1; i < GameConstants.Width - 1; i++)
        {

            _tiles[0, i] = new Tile();
            _tiles[GameConstants.Height - 1, i] = new Tile();
            
            _tiles[0,i].AddObj(new FrameObject('═'));
            _tiles[GameConstants.Height - 1,i].AddObj(new FrameObject('═'));
            
        }
        for (int j = 1; j < GameConstants.Height-1; j++)
        {

            _tiles[j, 0] = new Tile();
            _tiles[j, GameConstants.Width - 1] = new Tile();
            
            _tiles[j,0].AddObj(new FrameObject('║'));
            _tiles[j, GameConstants.Width - 1].AddObj(new FrameObject('║'));
        }
    }

    public void CreateEmptyDungeon()
    {
        _pmb.AddInitial();
        for (int i = 1; i < GameConstants.Width-1; i++)
        {
            for (int j = 1; j < GameConstants.Height-1; j++)
            {
                _tiles[j, i] = new Tile();
            }
        }
    }

    public void CreateFilledDungeon()
    {
        _pmb.AddInitial();
        for (int i = 1; i < GameConstants.Width-1; i++)
        {
            for (int j = 1; j < GameConstants.Height-1; j++)
            {
                _tiles[j, i] = new Tile();
                _tiles[j,i].AddObj(new Wall());
            }
        }
        _tiles[1,1].Reset();
    }

    public void AddCentralHall(int width, int height)
    {
        if (width >= GameConstants.Width - 2 || height >= GameConstants.Height - 2)
            return;

        int left = (GameConstants.Width - width) / 2;
        int right = left + width - 1;

        int top = (GameConstants.Height - height) / 2;
        int bottom = top + height - 1;
        if (DoRoomsOverlap(left, top, width, height))
            return;

        for (int x = left; x <= right; x++)
        {
            _tiles[top, x].Reset();
            _tiles[top, x].AddObj(new Wall());

            _tiles[bottom, x].Reset();
            _tiles[bottom, x].AddObj(new Wall());
        }

        for (int y = top; y <= bottom; y++)
        {
            _tiles[y, left].Reset();
            _tiles[y, left].AddObj(new Wall());

            _tiles[y, right].Reset();
            _tiles[y, right].AddObj(new Wall());
        }

        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                _tiles[y, x].Reset();
            }
        }

        Room r = new Room() { Top = top, Height = height, Width = width, Left = left, Connected = false };
        _rooms.Add(r);
    }

    private bool DoRoomsOverlap(int left, int top, int width, int height) // TODO zmien - niech pokoje paja przerwe 1 od siebie
    {
        int newLeft = left;
        int newRight = left + width - 1;
        int newTop = top;
        int newBottom = top + height - 1;

        foreach (var room in _rooms)
        {
            int roomLeft = room.Left;
            int roomRight = room.Left + room.Width - 1;
            int roomTop = room.Top;
            int roomBottom = room.Top + room.Height - 1;

            bool noOverlap =
                newRight < roomLeft - 1 ||
                newLeft - 1 > roomRight ||
                newBottom < roomTop - 1 ||
                newTop - 1 > roomBottom;
            if (!noOverlap)
                return true;
        }
        return false;
    }

    private void DrawRoom(Room room)
    {
        int left = room.Left;
        int right = room.Left + room.Width - 1;
        int top = room.Top;
        int bottom = room.Top + room.Height - 1;

        for (int x = left; x <= right; x++)
        {
            if (!_tiles[top, x].BlocksMovement)
            {
                _tiles[top, x].Reset();
                _tiles[top, x].AddObj(new Wall());
            }

            if (!_tiles[bottom, x].BlocksMovement)
            {
                _tiles[bottom, x].Reset();
                _tiles[bottom, x].AddObj(new Wall());
            }
            
        }

        for (int y = top; y <= bottom; y++)
        {
            if (!_tiles[y, left].BlocksMovement)
            {
                _tiles[y, left].Reset();
                _tiles[y, left].AddObj(new Wall());
            }

            if (!_tiles[y, right].BlocksMovement)
            {
                _tiles[y, right].Reset();
                _tiles[y, right].AddObj(new Wall());
            }
        }

        for (int y = top + 1; y < bottom; y++)
        {
            for (int x = left + 1; x < right; x++)
            {
                _tiles[y, x].Reset();
            }
        }
    }
    private void AddRoom()
    {
        Random r = new Random();
        int width = 0;
        int height = 0;
        int left = 0;
        int top = 0;
        int i;
        for (i = 0; i < GameConstants.RoomInsertAttempts; i++)
        {
            width = r.Next() % (GameConstants.Width / 4) + 5;
            height = r.Next() % (GameConstants.Height / 4) + 5;
            left = r.Next() % (GameConstants.Width - width - 2) + 2;
            top = r.Next() % (GameConstants.Height - height - 2) + 2;
            
            if (!DoRoomsOverlap(left, top, width, height))
                break;
        }

        if (i == GameConstants.RoomInsertAttempts)
            return;

        if (width == 0)
            return;
        Room room = new Room() { Height = height, Width = width, Left = left, Top = top, Connected = false};
        
        _rooms.Add(room);
        DrawRoom(room);

    }

    private void AddPlayerRoom() // Funkcja zakłada gracza w (1,1)
    {
        Random r = new Random();
        int width = 0;
        int height = 0;
        int i;
        for (i = 0; i < GameConstants.RoomInsertAttempts; i++)
        {
            width = r.Next() % (GameConstants.Width / 4) + 5;
            height = r.Next() % (GameConstants.Height / 4) + 5;
            if (!DoRoomsOverlap(0, 0, width, height))
                break;
        }

        if (i == GameConstants.RoomInsertAttempts)
            return;
        if (width == 0)
            return;
        Room room = new Room() { Height = height, Width = width, Left = 0, Top = 0, Connected = false};
        _rooms.Add(room);
        DrawRoom(room);
        _playerRoom = true;
    }

    private int GetDisconnectedRoom()
    {
        for (int i =0; i < _rooms.Count; i++)
        {
            if (!_rooms[i].Connected)
                return i;
        }

        return -1;
    }

    private readonly Random _rng = new Random();

    private (Point?, Direction) GetRandomWall(Room room)
    {
        List<(Point, Direction)> candidates = new List<(Point, Direction)>();

        int left = room.Left;
        int right = room.Left + room.Width - 1;
        int top = room.Top;
        int bottom = room.Top + room.Height - 1;

        for (int j = left + 1; j < right; j++)
        {
            if (_tiles[top, j].BlocksMovement &&
                _tiles[top, j - 1].BlocksMovement &&
                _tiles[top, j + 1].BlocksMovement)
            {
                candidates.Add((new Point(j, top), Direction.North));
            }
        }

        for (int j = left + 1; j < right; j++)
        {
            if (_tiles[bottom, j].BlocksMovement &&
                _tiles[bottom, j - 1].BlocksMovement &&
                _tiles[bottom, j + 1].BlocksMovement)
            {
                candidates.Add((new Point(j, bottom), Direction.South));
            }
        }

        for (int i = top + 1; i < bottom; i++)
        {
            if (_tiles[i, left].BlocksMovement &&
                _tiles[i - 1, left].BlocksMovement &&
                _tiles[i + 1, left].BlocksMovement)
            {
                candidates.Add((new Point(left, i), Direction.West));
            }
        }

        for (int i = top + 1; i < bottom; i++)
        {
            if (_tiles[i, right].BlocksMovement &&
                _tiles[i - 1, right].BlocksMovement &&
                _tiles[i + 1, right].BlocksMovement)
            {
                candidates.Add((new Point(right, i), Direction.East));
            }
        }

        if (candidates.Count == 0)
            return (null, Direction.South);

        return candidates[_rng.Next(candidates.Count)];
    }

    private bool CanDigNorth(Point p)
    {
        int left = p.Left;
        int top = p.Top - 1;

        while (top > 1)
        {
            if (IsInRoomCorner(top, left))
                return false;

            if (_tiles[top, left].BlocksMovement && !_tiles[top - 1, left].BlocksMovement)
                return true;

            top--;
        }

        return false;
    }

    private bool CanDigSouth(Point p)
    {
        int left = p.Left;
        int top = p.Top + 1;

        while (top < GameConstants.Height - 2)
        {
            if (IsInRoomCorner(top, left))
                return false;

            if (_tiles[top, left].BlocksMovement && !_tiles[top + 1, left].BlocksMovement)
                return true;

            top++;
        }

        return false;
    }

    private bool CanDigWest(Point p)
    {
        int left = p.Left - 1;
        int top = p.Top;

        while (left > 1)
        {
            if (IsInRoomCorner(top, left))
                return false;

            if (_tiles[top, left].BlocksMovement && !_tiles[top, left - 1].BlocksMovement)
                return true;

            left--;
        }

        return false;
    }

    private bool CanDigEast(Point p)
    {
        int left = p.Left + 1;
        int top = p.Top;

        while (left < GameConstants.Width - 2)
        {
            if (IsInRoomCorner(top, left))
                return false;

            if (_tiles[top, left].BlocksMovement && !_tiles[top, left + 1].BlocksMovement)
                return true;

            left++;
        }

        return false;
    }
    

    private bool CanDig(Point p, Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                return CanDigNorth(p);
            case Direction.South:
                return CanDigSouth(p);
            case Direction.East:
                return CanDigEast(p);
            case Direction.West:
                return CanDigWest(p);
            default:
                throw new InvalidOperationException("Invalid Direction param");
        }
        
    }

    private void CarveNorth(Point p)
    {
        _tiles[p.Top,p.Left].Reset();
        int left = p.Left;
        int top = p.Top - 1;

        while (top > 1)
        {
            if (_tiles[top, left].BlocksMovement && !_tiles[top - 1, left].BlocksMovement)
                break;

            if (left > 1)
            {
                _tiles[top, left - 1].Reset();
                _tiles[top, left - 1].AddObj(new Wall());
            }

            if (left < GameConstants.Width - 2)
            {
                _tiles[top, left + 1].Reset();
                _tiles[top, left + 1].AddObj(new Wall());
            }

            _tiles[top, left].Reset();
            top--;
        }
        _tiles[top, left].Reset();
    }

    private void CarveSouth(Point p)
    {
        _tiles[p.Top,p.Left].Reset();
        int left = p.Left;
        int top = p.Top + 1;

        while (top < GameConstants.Height - 2)
        {
            if (_tiles[top, left].BlocksMovement && !_tiles[top + 1, left].BlocksMovement)
                break;

            if (left > 1)
            {
                _tiles[top, left - 1].Reset();
                _tiles[top, left - 1].AddObj(new Wall());
            }

            if (left < GameConstants.Width - 2)
            {
                _tiles[top, left + 1].Reset();
                _tiles[top, left + 1].AddObj(new Wall());
            }

            _tiles[top, left].Reset();
            top++;
        }
        _tiles[top, left].Reset();
    }

    private void CarveWest(Point p)
    {
        _tiles[p.Top,p.Left].Reset();
        int left = p.Left - 1;
        int top = p.Top;

        while (left > 1)
        {
            if (_tiles[top, left].BlocksMovement && !_tiles[top, left - 1].BlocksMovement)
                break;

            if (top > 1)
            {
                _tiles[top - 1, left].Reset();
                _tiles[top - 1, left].AddObj(new Wall());
            }

            if (top < GameConstants.Height - 2)
            {
                _tiles[top + 1, left].Reset();
                _tiles[top + 1, left].AddObj(new Wall());
            }

            _tiles[top, left].Reset();
            left--;
        }
        _tiles[top, left].Reset();
    }

    private void CarveEast(Point p)
    {
        _tiles[p.Top,p.Left].Reset();
        int left = p.Left + 1;
        int top = p.Top;

        while (left < GameConstants.Width - 2)
        {
            if (_tiles[top, left].BlocksMovement && !_tiles[top, left + 1].BlocksMovement)
                break;

            if (top > 1)
            {
                _tiles[top - 1, left].Reset();
                _tiles[top - 1, left].AddObj(new Wall());
            }

            if (top < GameConstants.Height - 2)
            {
                _tiles[top + 1, left].Reset();
                _tiles[top + 1, left].AddObj(new Wall());
            }

            _tiles[top, left].Reset();
            left++;
        }
        _tiles[top, left].Reset();
    }
    private void CarveCorridors(Point p, Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                CarveNorth(p);
                break;
            case Direction.South:
                CarveSouth(p);
                break;
            case Direction.East:
                CarveEast(p);
                break;
            case Direction.West:
                CarveWest(p);
                break;
            default:
                throw new InvalidOperationException("Invalid Direction param");
        }
    }
    private void AddCorridor()
    {
        int rIdx = GetDisconnectedRoom();
        if (rIdx == -1)
            return;
        Room r = _rooms[rIdx];
        
        for (int i = 0; i < GameConstants.CorridorGenerationAttempts; i++)
        {
            (Point? p, Direction dir) = GetRandomWall(r);
            if (p == null)
                return;
            if (CanDig((Point)p,dir))
            {
                CarveCorridors((Point)p, dir);
                r.Connected = true;
                _rooms[rIdx] = r;
                return;
            }
        }
    }
    public void AddCorridors(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddCorridor();
        }
    }

    public void AddRooms(int counts)
    {
        for (int i = 0; i < counts; i++)
        {
            if(!_playerRoom)
                AddPlayerRoom();
            else
            {
                AddRoom();
            }
        }
    }

    private bool IsInRoomCorner(int i, int j)
    {
        foreach (var room in _rooms)
        {
            int left = room.Left;
            int right = room.Left + room.Width - 1;
            int top = room.Top;
            int bottom = room.Top + room.Height - 1;

            bool isCorner =
                (i == top && j == left) ||
                (i == top && j == right) ||
                (i == bottom && j == left) ||
                (i == bottom && j == right);

            if (isCorner)
                return true;
        }

        return false;
    }

    
    private void AddObjToBoard(int x, int y, BoardObject obj)
    {
        obj.X = x;
        obj.Y = y;
        obj.ObjBoard = _board;
        _tiles[x, y].AddObj(obj);
        
    }
    private void AddToMap(BoardObject obj, bool resetTile = false)
    {
        Random r = new Random();
        if (_rooms.Count > 0)
        {
            Room room = _rooms[r.Next(_rooms.Count)];

            int left = room.Left;
            int right = room.Left + room.Width - 1;
            int top = room.Top;
            int bottom = room.Top + room.Height - 1;

            int j = r.Next(left + 1, right);
            int i = r.Next(top + 1, bottom);
            if(resetTile)
                _tiles[i,j].Reset();
            AddObjToBoard(i,j,obj);
            return;
        }

        for (int attempt = 0; attempt < 100; attempt++)
        {
            int i = r.Next(1, GameConstants.Height - 1);
            int j = r.Next(1, GameConstants.Width - 1);

            if (_tiles[i, j].BlocksMovement)
                continue;

            AddObjToBoard(i,j,obj);
            return;
        }

    }
    public void AddItems(int count, Func<Item>[]? items = null)
    {
        if(items==null)
        {
            items = new Func<Item>[]
            {
                () => new MediumItem(_player),
                () => new CoolItem(_player),
                () => new SmallItem(_player)
            };
        }
        Random r = new Random();
        // _tiles[1,2] = items[r.Next(items.Length)]();
        for (int i = 0; i < count; i++)
        {
            AddToMap(items[r.Next(items.Length)]());
        }
    }

    public void AddArtifact(Tool t)
    {
        AddToMap(t);
    }

    public void AddWeapons(int count, Func<Weapon>[]? weapons = null)
    {
        _pmb.AddPickup();
        if (weapons == null)
        {
            weapons = new Func<Weapon>[]
            {
                () => new SmallWeapon(_player),
                () => new MediumWeapon(_player),
                () => new TwoHandedWeapon(_player),
                () => new SmallMagicWeapon(_player)
            };
        }
        Random r = new Random();
        for (int i = 0; i < count; i++)
        {
            int power = r.Next(4);
            if(power==0)
                AddToMap(weapons[r.Next(weapons.Length)]());
            else if(power==1)
                AddToMap(new UnluckyWeaponDecorator(_player,weapons[r.Next(weapons.Length)]()));
            else if(power==2)
            {
                AddToMap(new StrongWeaponDecorator(_player,weapons[r.Next(weapons.Length)]()));
            }
            else
            {
                AddToMap(new UnluckyWeaponDecorator(_player,new StrongWeaponDecorator(_player,weapons[r.Next(weapons.Length)]())));
            }
        }
    }

    public void AddEnemies(int count, Func<Enemy>[]? enemies = null)
    {
        _pmb.AddEnemy(); // TODO Add Enemy
        SpeciesGroup human = new SpeciesGroup(
            "Human",
            new AngryStrategy(),
            new FollowSoundReaction(),
            new FollowPlayerReaction(),
            new[] { EnemyBehaviorTrigger.Sound, EnemyBehaviorTrigger.Player });
        SpeciesGroup animal = new SpeciesGroup(
            "Animal",
            new ScaredStrategy(),
            new FleeSoundReaction(),
            new FleePlayerReaction());
        if (enemies == null)
        {
            enemies = new Func<Enemy>[]
            {
                () => new MediumEnemy(group: human, b: _board),
                () => new BigEnemy(group:  human,b: _board),
                () => new SmallEnemy(group:animal,b: _board),
            };
        }
        Random r = new Random();
        for (int i = 0; i < count; i++)
        {
            AddToMap(enemies[r.Next(enemies.Length)](), true);
        }
    }

    public Tile[,] GetDungeon()
    {
        return _tiles;
    }
}
