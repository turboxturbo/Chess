
public class Cell
{
    public int X { get; }
    public int Y { get; }
    public ChessPiece CurrentPiece { get; internal set; }
    public bool IsEmpty => CurrentPiece == null;
    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }
}

