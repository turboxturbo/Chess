using System;

public class BlackKing : ChessPiece
{
    public int AttackPower { get; }

    public BlackKing(int health, int attackPower)
        : base(PieceColor.Black, health)
    {
        AttackPower = attackPower;
    }

    public override bool CanMove(Board board, int fromX, int fromY, int toX, int toY)
    {
        int dx = Math.Abs(toX - fromX);
        int dy = Math.Abs(toY - fromY);

        return dx <= 1 && dy <= 1 && (dx != 0 || dy != 0);
    }

    public void Attack(ChessPiece target)
    {
        
    }
}
