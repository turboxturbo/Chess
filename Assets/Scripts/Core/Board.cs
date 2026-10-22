using System;

public class Board
{
    public Cell[,] ChessBoard {get;} = new Cell[8, 8];

    public Board()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessBoard[x, y] = new Cell(x, y);
            }
        }
    }

    // public void PlacePiece(ChessPiece piece, int x, int y)
    // {
    //     if (!IsInsideBoard(x, y) || piece == null)
    //     {
    //         return;
    //     }
    //     Cell target = ChessBoard[x, y];
    //     if (!target.IsEmpty)
    //     {
    //         return;
    //     }
    //     target.CurrentPiece = piece;
    // }
    public void InitializeBoard()
    {
        
    }

    public bool IsPathClear(
    int fromX,
    int fromY,
    int toX,
    int toY)
{
    int stepX = Math.Sign(toX - fromX);
    int stepY = Math.Sign(toY - fromY);

    int x = fromX + stepX;
    int y = fromY + stepY;

    while (x != toX || y != toY)
    {
        if (!ChessBoard[x, y].IsEmpty)
        {
            return false;
        }

        x += stepX;
        y += stepY;
    }

    return true;
}

    private bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < 8 && y >= 0 && y < 8;
    }

    public bool TryMovePiece(int fromX, int fromY, int toX, int toY)
    {
        if (!IsInsideBoard(fromX, fromY) || !IsInsideBoard(toX, toY))
        {
            return false;
        }

        Cell fromCell = ChessBoard[fromX, fromY];
        Cell toCell = ChessBoard[toX, toY];

        if (fromCell.IsEmpty || !toCell.IsEmpty)
        {
            return false;
        }

        ChessPiece movingPiece = fromCell.CurrentPiece;

        if (movingPiece == null)
        {
            return false;
        }

        toCell.CurrentPiece = movingPiece;
        fromCell.CurrentPiece = null;

        return true;
    }
}