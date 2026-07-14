using PROJ.GameConstansts;
using PROJ.Tools.Classes;

namespace PROJ;

public class Backpack
{
    private List<Tool> _inventory;
    private int _tools;
    private int _spaceTaken;
    public Backpack()
    {
        _inventory = new List<Tool>();
        _tools = 0;
        _spaceTaken = 0;
    }

    public bool TryAddItem(Tool tool)
    {
        
        if (_spaceTaken + tool.Space <= GameConstants.BackpackCapacity)
        {
            _inventory.Add(tool);
            _tools++;
            _spaceTaken += tool.Space;
            return true;
        }
        return false;
    }

    public bool TryOverwriteItemAt(Tool t, int i)
    {
        if (i >= 0 && i < _tools &&  _spaceTaken + t.Space - _inventory[i].Space <= GameConstants.BackpackCapacity)
        {
            _spaceTaken += t.Space - _inventory[i].Space;
            _inventory[i] = t;
            return true;
        }

        return false;
    }

    public Tool? TryGetItem(int i)
    {
        if (i >= _tools || i < 0)
        {
            return null;
        }

        return _inventory[i];
    }



    public bool IsItemAt(int i)
    {
        if (i >= _tools || i < 0)
            return false;
        return true;
    }
    
    public int GetItemsCount
    {
        get => _tools;
    }

    public Tool? Delete(int i)
    {
        if (i >= _tools || i < 0)
            return null;
        _spaceTaken -= _inventory[i].Space;
        Tool t = _inventory[i];
        _inventory.RemoveAt(i);
        _tools--;
        return t;
    }

    public bool IsEnoughCapForSwap(int newCap, int oldCap)
    {
        if (_spaceTaken + newCap - oldCap > GameConstants.BackpackCapacity)
            return false;
        return true;
    }
    
}