using System;
using System.Collections.Generic;

// Coordinates and distances are measured in board cells, independently of Unity physics.
public sealed class ShotgunAttack
{
    public int PelletCount { get; }
    public int PelletDamage { get; }
    public float SpreadDegrees { get; }
    public float Range { get; }
    private readonly Random _random;

    public ShotgunAttack(int pelletCount = 8, int pelletDamage = 1,
        float spreadDegrees = 30f, float range = 5f, int? seed = null)
    {
        if (pelletCount <= 0 || pelletDamage <= 0 || range <= 0 ||
            float.IsNaN(range) || float.IsInfinity(range) ||
            spreadDegrees < 0 || spreadDegrees > 180 || float.IsNaN(spreadDegrees))
            throw new ArgumentOutOfRangeException();
        PelletCount = pelletCount;
        PelletDamage = pelletDamage;
        SpreadDegrees = spreadDegrees;
        Range = range;
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public Dictionary<ChessPiece, int> Fire(Board board, int originX, int originY,
        float directionX, float directionY)
    {
        if (!(board.ChessBoard[originX, originY].CurrentPiece is BlackKing))
            throw new InvalidOperationException("Only the black king can fire.");
        if (float.IsNaN(directionX) || float.IsInfinity(directionX) ||
            float.IsNaN(directionY) || float.IsInfinity(directionY) ||
            directionX * directionX + directionY * directionY < 0.000001f)
            throw new ArgumentException("Aim direction must be nonzero and finite.");

        var damage = new Dictionary<ChessPiece, int>();
        double aim = Math.Atan2(directionY, directionX);
        for (int pellet = 0; pellet < PelletCount; pellet++)
        {
            double angle = aim + (_random.NextDouble() - 0.5) * SpreadDegrees * Math.PI / 180;
            double dx = Math.Cos(angle), dy = Math.Sin(angle);
            double nearest = double.PositiveInfinity;
            ChessPiece target = null;
            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;
                if (piece == null || (x == originX && y == originY)) continue;
                double entry = 0, exit = Range;
                if (IntersectAxis(originX, dx, x, ref entry, ref exit) &&
                    IntersectAxis(originY, dy, y, ref entry, ref exit) && entry < nearest)
                {
                    nearest = entry;
                    target = piece;
                }
            }
            if (target != null)
                damage[target] = (damage.TryGetValue(target, out int hits) ? hits : 0) + PelletDamage;
        }

        // Resolve every pellet before removing casualties: no shooting through a dying blocker.
        foreach (var hit in damage) hit.Key.TakeDamage(hit.Value);
        for (int x = 0; x < board.Width; x++)
        for (int y = 0; y < board.Height; y++)
        {
            ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;
            if (piece != null && piece.Health == 0) board.ChessBoard[x, y].CurrentPiece = null;
        }
        return damage;
    }

    private static bool IntersectAxis(double origin, double direction, int cell,
        ref double entry, ref double exit)
    {
        if (Math.Abs(direction) < 1e-10) return origin >= cell - 0.5 && origin <= cell + 0.5;
        double a = (cell - 0.5 - origin) / direction;
        double b = (cell + 0.5 - origin) / direction;
        entry = Math.Max(entry, Math.Min(a, b));
        exit = Math.Min(exit, Math.Max(a, b));
        return entry <= exit;
    }
}
