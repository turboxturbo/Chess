using System;

public class Knight : ChessPiece
{
    public Knight(PieceColor color, int health)
        : base(color, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        int dx = Math.Abs(toX - fromX);
        int dy = Math.Abs(toY - fromY);

        return dx == 1 && dy == 2 ||
            dx == 2 && dy == 1;
    }
}
