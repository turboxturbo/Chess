public abstract class ChessPiece
{
    public PieceColor Color { get; }
    public int Health { get; private set; }
    protected ChessPiece(PieceColor color, int health)
    {
        Color = color;
        Health = health;
    }

    public abstract bool CanMove(Board board, int fromX, int fromY, int toX, int toY);
}


