using System;

public class Pawn : ChessPiece
{
    public Pawn(PieceColor color, int health)
        : base(color, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        int direction = Color == PieceColor.White ? 1 : -1;
        int dx = Math.Abs(toX - fromX);
        int dy = toY - fromY;

        Cell target = board.ChessBoard[toX, toY];

        if (dx == 0 && dy == direction)
        {
            return target.IsEmpty;
        }

        if (dx == 1 && dy == direction)
        {
            return !target.IsEmpty && target.CurrentPiece.Color != Color;
        }

        return false;
    }
}
