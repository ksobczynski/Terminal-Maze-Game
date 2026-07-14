using PROJ.Communication.Results;
using PROJ.GameConstansts;
using PROJ.Tools.Classes;

namespace PROJ;

public class Player
{
    public double Level;
    public int Dexterity;
    public int Health;
    public int Luck;
    public int Strength;
    public int Wisdom;
    
    public Tool? LeftHand;
    public Tool? RightHand;
    
    private int[] _position; // 0 - x, 1 - y
    
    private Board _board;
    

    public Backpack? PlayerBackpack;
    private bool _backPackMode;
    private int _backpackIdx;
    

    public int Gold;
    public int Coins;
    public int NetworkId { get; set; }
    public char MapSymbol => NetworkId is >= 1 and <= 9
        ? (char)('0' + NetworkId)
        : GameConstants.PlayerSymbol;

    public Player() // Tak jak w przypadku board - pewne fieldy nie moga byc nullami
    {
        Level = 1.0;
        Dexterity = 5;
        Health = 100;
        Luck = 5;
        Strength = 5;
        Wisdom = 5;
        LeftHand = null;
        RightHand = null;
        _position = new int[2];
        _position[0] = 1;
        _position[1] = 1;
        Gold = 0;
        Coins = 0;
        PlayerBackpack = new Backpack();
        _backPackMode = false;
    }

    public void AssignBoard(Board b)
    {
        _board = b;
    }

    public int[] Position
    {
        get => _position;
        set => _position = value ?? throw new ArgumentNullException(nameof(value));
    }

    public PickUpResult? TryPickUp()
    {
        var content = _board.GetCurrentlySeeked();
        if (content == null)
            return new PickUpResult(false, null, null);
        
        PickUpResult? result = content.PickUp(this);
        if (result != null && result.TileChangeResult != null)
            result.SetActionBoxResult(_board.CreateActionBoxResult(result.TileChangeResult.Row, result.TileChangeResult.Column));

        return result;
    }

    public BackpackPointerMoveResult? TryIncrementBackpackIdx()
    {
        if (PlayerBackpack != null && _backpackIdx < PlayerBackpack.GetItemsCount ) // upsi
        {
            _backpackIdx++;
            // EqBox.PointerUp(_backpackIdx--);
            return new BackpackPointerMoveResult(true, _backpackIdx - 1, false);
        }

        return null;
    }
    
    public BackpackPointerMoveResult? TryDecrementBackpackIdx()
    {
        if (PlayerBackpack != null && _backpackIdx > 0)
        {
            
            _backpackIdx--;
            // EqBox.PointerUp(_backpackIdx--);
            return new BackpackPointerMoveResult(true, _backpackIdx + 1, true);
        }

        return null;

    }
    // private void EnterBackpackMode()
    // {
    //     
    //     EqBox.PointerInit();
    // }
    //
    // private void ExitBackpackMode()
    // {
    //     
    // }
    public BackpackModeSwitchResult SwitchBackpackMode()
    {
        if (!_backPackMode)
        {
            _backPackMode = true;
            _backpackIdx = 0;
            return new BackpackModeSwitchResult(true);
            // EnterBackpackMode();
        }
        else
        {
            _backPackMode = false;
            int prev = _backpackIdx;
            _backpackIdx = 0;
            return new BackpackModeSwitchResult(false,prev );
            // ExitBackpackMode();
        }
    }
    
    public bool IsInBackpack
    {
        get => _backPackMode;
    }

    public int BackpackIndex => _backpackIdx;
    
    private BackpackHandChangeResult? RightAdd()
    {
        if (PlayerBackpack == null)
            return null;
        
        Tool? t = PlayerBackpack.TryGetItem(_backpackIdx); // moze sie zmienic w trakcie
        if (t == null)
            return null;
        if (t.Space > 2) // jak wyzej
            return null;
        if (t.Space == 2)
        {
            if (LeftHand != null)
                return null;
        }
        if (LeftHand != null && LeftHand.Space == 2)
            return null;
        int previousIdx = _backpackIdx;
        PlayerBackpack.Delete(_backpackIdx);
        _backpackIdx = 0;
        RightHand = t;
        return new BackpackHandChangeResult(true, true, false, previousIdx, true, false, true);
    }
    private BackpackHandChangeResult? LeftAdd()
    {
        if (PlayerBackpack == null)
            return null;
        
        Tool? t = PlayerBackpack.TryGetItem(_backpackIdx);
        if (t == null)
            return null;
        if (t.Space > 2)
            return null;
        if (t.Space == 2)
        {
            if (RightHand != null)
                return null;
        }

        if (RightHand != null && RightHand.Space == 2)
            return null;
        
        int previousIdx = _backpackIdx;
        PlayerBackpack.Delete(_backpackIdx);
        _backpackIdx = 0;
        LeftHand = t;
        return new BackpackHandChangeResult(true, true, false, previousIdx, true, true, false);
    }

    private BackpackHandChangeResult? LeftSwap()
    {
        if (PlayerBackpack == null)
            return null;
        Tool? t = PlayerBackpack.TryGetItem(_backpackIdx);
        if (t == null)
            return null;
        if (LeftHand.Space == t.Space)
        {
            Tool newLeft = t;
            if (PlayerBackpack.TryOverwriteItemAt(LeftHand, _backpackIdx))
            {
                LeftHand = newLeft;
                return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, true, false);
            }
        }
        else if (LeftHand.Space > t.Space)
        {
            if (PlayerBackpack.IsEnoughCapForSwap(LeftHand.Space, t.Space))
            {
                Tool newLeft = t;
                if (PlayerBackpack.TryOverwriteItemAt(LeftHand, _backpackIdx))
                {
                    LeftHand = newLeft;
                    return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, true, false);
                }
            }
        }
        else
        {
            if (t.Space == 2) // TODO na ten moment nie ma innych ale nie moge czegos o pojemnosci  > 2 wziac do reki na razie
            {
                if (RightHand != null)
                    return null;
                Tool newLeft = t;
                if (PlayerBackpack.TryOverwriteItemAt(LeftHand, _backpackIdx))
                {
                    LeftHand = newLeft;
                    return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, true, false);
                }
                
            }
        }
        return null;
    }

    private BackpackHandChangeResult? RightSwap()
    {
        if (PlayerBackpack == null)
            return null;
        Tool? t = PlayerBackpack.TryGetItem(_backpackIdx);
        if (t == null)
            return null;
        if (RightHand.Space == t.Space)
        {
            Tool newRight = t;
            if (PlayerBackpack.TryOverwriteItemAt(RightHand, _backpackIdx))
            {
                RightHand = newRight;
                return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, false, true);
            }
        }
        else if (RightHand.Space > t.Space)
        {
            if (PlayerBackpack.IsEnoughCapForSwap(RightHand.Space, t.Space))
            {
                Tool newRight = t;
                if (PlayerBackpack.TryOverwriteItemAt(RightHand, _backpackIdx))
                {
                    RightHand = newRight;
                    return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, false, true);
                }
            }
        }
        else
        {
            if (t.Space == 2) // TODO na ten moment nie ma innych ale nie moge czegos o pojemnosci  > 2 wziac do reki na razie
            {
                if (LeftHand != null)
                    return null;
                Tool newRight = t;
                if (PlayerBackpack.TryOverwriteItemAt(RightHand, _backpackIdx))
                {
                    RightHand = newRight;
                    return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, false, true);
                }
                
            }
        }
        return null;
    }

    private BackpackHandChangeResult? LeftAddToBp()
    {
        if (PlayerBackpack == null || LeftHand == null)
            return null;
        if (!PlayerBackpack.IsEnoughCapForSwap(LeftHand.Space, 0))
            return null;
        Tool t = LeftHand;
        if (PlayerBackpack.TryAddItem(t))
        {
            LeftHand = null;
            return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, true, false);
        }
        return null;
    }
    
    private BackpackHandChangeResult? RightAddToBp()
    {
        if (PlayerBackpack == null || RightHand == null)
            return null;
        if (!PlayerBackpack.IsEnoughCapForSwap(RightHand.Space, 0))
            return null;
        Tool t = RightHand;
        if (PlayerBackpack.TryAddItem(t))
        {
            RightHand = null;
            return new BackpackHandChangeResult(true, true, true, _backpackIdx, false, false, true);
        }
        return null;
    }
    public BackpackHandChangeResult? TrySwap(char c)
    {
        if (c == 'l')
        {
            if (LeftHand == null && PlayerBackpack.TryGetItem(_backpackIdx) == null)
                return null;
            if (LeftHand == null)
                return LeftAdd();
            else if(PlayerBackpack.TryGetItem(_backpackIdx) == null)
            {
                return LeftAddToBp();
            }
            else
            {
                return LeftSwap();
            }
        }
        else if (c == 'r')
        {
            if (RightHand == null && PlayerBackpack.TryGetItem(_backpackIdx) == null)
                return null;
            if (RightHand == null)
                return RightAdd();
            else if (PlayerBackpack.TryGetItem(_backpackIdx) == null)
            {
                return RightAddToBp();
            }
            else
            {
                return RightSwap();
            }
        }
        return null;
    }

    public BackpackDropResult? BackpackDrop()
    {
        if (PlayerBackpack == null)
            return null;
        int previousIdx = _backpackIdx;
        Tool? tool = PlayerBackpack.Delete(_backpackIdx);
        if (tool == null)
        {
            return null;
        }
        _backpackIdx = 0;
        _board.DropItem(tool);
        return new BackpackDropResult(true, previousIdx, tool.X, tool.Y, _board.CreateTileChangeResult(tool.X, tool.Y));
    }
    
    public bool Dead()
    {
        return Health == 0;
    }
    
    
    public string Name { get; set; }
}
