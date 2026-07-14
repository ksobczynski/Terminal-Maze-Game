namespace PROJ.Enemies;

public class SoundMessage
{
    private string _message;
    private int _x;
    private int _y;
    private int _dist;
    public SoundMessage(int x, int y, int dist, string message)
    {
        _x = x;
        _y = y;
        _dist = dist;
        _message = message;
    }
    
    public string GetMessage => _message;
    public int GetX => _x;
    public int GetY => _y;
    public int GetDist => _dist;
}