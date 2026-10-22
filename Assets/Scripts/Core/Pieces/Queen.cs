using System;

public class Queen : ChessPiece
{
    public Queen(PieceColor color, int health)
        : base(color, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        int dx = Math.Abs(toX - fromX);
        int dy = Math.Abs(toY - fromY);

        if (dx == 0 && dy == 0)
        {
            return false;
        }

        bool movesStraight = fromX == toX || fromY == toY;
        bool movesDiagonally = dx == dy;

        return (movesStraight || movesDiagonally) &&
               board.IsPathClear(fromX, fromY, toX, toY);
    }
}
