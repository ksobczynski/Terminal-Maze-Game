namespace PROJ.Communication.Results;

public class MessageBusResult
{
    private int _X;
    private int _Y;
    private int _range;

    public MessageBusResult(int X, int Y, int range)
    {
        _X = X;
        _Y = Y;
        _range = range;
    }

    public int X => _X;
    public int Y => _Y;
    public int Range => _range;
    
    
    
}