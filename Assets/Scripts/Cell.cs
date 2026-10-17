
using UnityEngine;

public class Cell
{
    public CellPosition Position { get; }
    public ChessPiece CurrentPiece { get; private set; }
    public bool IsEmpty => CurrentPiece == null;
    public Cell(int x, int y, ChessPiece piece = null)
    {
        Position = new CellPosition(x, y);
        CurrentPiece = piece;
    }
    public void PutPiece(ChessPiece piece)
    {
        CurrentPiece = piece;
    }
    public void RemovePiece()
    {
        CurrentPiece = null;
    }
}

public class ChessPiece
{
    public int Health { get; private set; }
    public int AttackPower { get; private set; }
    public PieceType Type { get; private set; }
}
public readonly struct CellPosition
{
    public int X { get; }
    public int Y { get; }

    public CellPosition(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}
