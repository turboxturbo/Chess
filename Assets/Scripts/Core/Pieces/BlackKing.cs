public class BlackKing : ChessPiece
{
    public int AttackPower { get; }

    public BlackKing(int health, int attackPower)
        : base(PieceColor.Black, health)
    {
        AttackPower = attackPower;
    }
}
