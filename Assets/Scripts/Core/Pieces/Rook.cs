public class Rook : ChessPiece
{
    public Rook(PieceColor color, int health)
        : base(color, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        bool moves = fromX == toX || fromY == toY;

         return moves &&
           board.IsPathClear(fromX, fromY, toX, toY);
    }
}
