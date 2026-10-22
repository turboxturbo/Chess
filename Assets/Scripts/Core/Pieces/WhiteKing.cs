using System;

public class WhiteKing : ChessPiece
{
    public WhiteKing(int health)
        : base(PieceColor.White, health)
    {
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        int dx = Math.Abs(toX - fromX);
        int dy = Math.Abs(toY - fromY);

        return dx <= 1 && dy <= 1 && (dx != 0 || dy != 0);
    }
}
