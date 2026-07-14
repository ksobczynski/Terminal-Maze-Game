namespace PROJ.Tools.Classes;

public abstract class Item : Tool
{
    public Item(Player player, string name="unnamed", char vis = 'X') : base(player, name, vis){}
    
}