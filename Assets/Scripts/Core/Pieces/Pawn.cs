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
        int dy = Math.Abs(toY - fromY);

        if (dx == 0 && dy == 0)
        {
            return false;
        }

        Cell target = board.ChessBoard[toX, toY];

        // Обычный ход вперёд.
        if (dx == 0 && dy == direction)
        {
            return target.IsEmpty;
        }
        
        if (Math.Abs(dx) == 1 && dy == direction)
        {
            return !target.IsEmpty &&
               target.CurrentPiece.Color != Color;
        }

        return false;
    }
}
