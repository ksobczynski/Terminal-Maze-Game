namespace PROJ.Builder.Classes;

public class PlayerMovesBuilder : IPlayerMovesBuilder
{
    private PlayerMovesBox _pmBox;

    public PlayerMovesBuilder(PlayerMovesBox pmBox)
    {
        _pmBox = pmBox;
    }
    public void AddInitial()
    {
        _pmBox.DisplayFrame();
        _pmBox.AddMove("W - Move Up");
        _pmBox.AddMove("A - Move Left");
        _pmBox.AddMove("S - Move Down");
        _pmBox.AddMove("D - Move Right");
        _pmBox.AddMove("B - Enter Backpack");
        _pmBox.AddMove("Esc - Leave Game");
    }

    public void AddPickup()
    {
        _pmBox.AddMove("E - Pick Up Tool");
        _pmBox.AddMove("← / → - Seek (Tile)");
        _pmBox.AddMove("↑ / ↓ - Seek (Backpack)");
    }

    public void AddEnemy()
    {
        _pmBox.AddMove("Enter - Fight Enemy");
    }
}
