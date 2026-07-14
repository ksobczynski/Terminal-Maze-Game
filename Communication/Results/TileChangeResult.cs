namespace PROJ.Communication.Results;

public class TileChangeResult
{
    private int _row;
    private int _column;
    private char _visual;
    private bool _isEmpty;

    public TileChangeResult(int row, int column, char visual = ' ', bool isEmpty = true)
    {
        _row = row;
        _column = column;
        _visual = visual;
        _isEmpty = isEmpty;
    }

    public int Row => _row;
    public int Column => _column;
    public char Visual => _visual;
    public bool IsEmpty => _isEmpty;

}
