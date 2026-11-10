public abstract class ChessPiece
{
    public PieceColor Color { get; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    protected ChessPiece(PieceColor color, int health)
    {
        Color = color;
        Health = health;
        MaxHealth = health;
    }

    public bool TakeDamage(int damage)
    {
        if (damage < 0) throw new System.ArgumentOutOfRangeException(nameof(damage));
        Health = System.Math.Max(0, Health - damage);
        return Health == 0;
    }

    public abstract bool CanMove(Board board, int fromX, int fromY, int toX, int toY);
}


