using System;

public class Bishop : ChessPiece
{
    public Bishop(PieceColor color, int health)
        : base(color, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        // Bishop moves diagonally
        int dx = Math.Abs(toX - fromX);
        int dy = Math.Abs(toY - fromY);

        return dx == dy && board.IsPathClear(fromX, fromY, toX, toY);
    }
}
