using PROJ.Communication.Results;

namespace PROJ.Goods.Classes.Valuables;

public class Gold : Valuable
{
    public override int Value => 2;
    public override char Visual => '✶';
    public override string Name => "Gold";
    public override string Description => "A precious piece of gold hidden from the dying";

    public override PickUpResult? PickUp(Player player)
    {
        player.Gold++;
        TileChangeResult? tileChange = null;
        if (ObjBoard != null)
            tileChange = ObjBoard.RemoveFromMap(X, Y);
        // player.UpdateWealth();
        return new PickUpResult(true, null, tileChange);
    }
}
