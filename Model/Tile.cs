using PROJ.Enemies;

namespace PROJ;

public class Tile
{
    public List<BoardObject>? Content;
    public int Objects;
    public bool BlocksMovement;
    public bool IsEmpty;

    public Tile()
    {
        Content = null;
        Objects = 0;
        BlocksMovement = false;
        IsEmpty = true;
    }
    public void Reset()
    {
        Content = null;
        BlocksMovement = false;
        Content = new List<BoardObject>();
        Objects = 0;
        IsEmpty = true;
    }

    public void AddObj(BoardObject obj) // TODO logika - wall jako jedyny obj
    {

        if (obj.Blocker && Objects > 0)
            return;
        if (Content == null)
            Content = new List<BoardObject>();
        
        Content.Add(obj);
        Objects++;
        BlocksMovement = obj.Blocker;
        IsEmpty = false;
    }

    public void Remove(int i)
    {
        if (Content == null || Objects <= i)
            return;
        Content.RemoveAt(i);
        Objects--;
        if (Objects == 0)
            IsEmpty = true;
    }

    public char GetVisual()
    {
        if (Content == null || Objects == 0)
            return ' ';

        var enemy = TryGetEnemy();
        if (enemy != null)
            return enemy.Visual;

        return Content[0].Visual;
    }

    public Enemy? TryGetEnemy()
    {
        if (Content == null)
            return null;

        foreach (var obj in Content)
        {
            if (obj is Enemy enemy)
                return enemy;
        }

        return null;
    }
    
    
}
    
