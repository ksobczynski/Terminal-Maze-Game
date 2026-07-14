using PROJ.Communication.Results;

namespace PROJ.Goods.Classes.Valuables;

public class Coin : Valuable
{
    public override int Value => 1;
    public override char Visual => '¢';
    public override string Name => "Coin";
    public override string Description => "A worn coin from the plague-stricken port.";
    
    public override PickUpResult? PickUp(Player player)
    {
        player.Coins++;
        TileChangeResult? tileChange = null;
        if (ObjBoard != null)
            tileChange = ObjBoard.RemoveFromMap(X, Y);
        // player.UpdateWealth();
        return new PickUpResult(true, null, tileChange);
    }
}
