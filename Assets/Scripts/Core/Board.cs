using System;

public class Board
{
    public Cell[,] ChessBoard {get;} = new Cell[8, 8];
    public int Width => ChessBoard.GetLength(0);
    public int Height => ChessBoard.GetLength(1);

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

    public void PlacePiece(ChessPiece piece, int x, int y)
    {
        if (!IsInsideBoard(x, y) || piece == null)
        {
            return;
        }
        Cell target = ChessBoard[x, y];
        if (!target.IsEmpty)
        {
            return;
        }
        target.CurrentPiece = piece;
    }
    public void InitializeBoard()
    {
        PlacePiece(new BlackKing(1, 3), 4, 7);
    }

    public bool SpawnWhitePiece(ChessPiece piece, out int spawnX, out int spawnY)
    {
        if (!TryGetSpawnCell(0, 2, 1, out spawnX, out spawnY))
            return false;

        PlacePiece(piece, spawnX, spawnY);
        return true;
    }

    public void ResetBlackKing(int startX, int startY)
    {
        if (!TryFindPiece<BlackKing>(out int x, out int y)) return;
        ChessPiece king = ChessBoard[x, y].CurrentPiece;
        ChessBoard[x, y].CurrentPiece = null;
        PlacePiece(king, startX, startY);
    }

    public ChessPiece PromotePawn(int x, int y, ChessPiece newPiece)
    {
        ChessPiece oldPawn = ChessBoard[x, y].CurrentPiece;
        ChessBoard[x, y].CurrentPiece = newPiece;
        return oldPawn;
    }

    public void ClearWhitePieces()
    {
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
        {
            ChessPiece piece = ChessBoard[x, y].CurrentPiece;
            if (piece != null && piece.Color == PieceColor.White)
                ChessBoard[x, y].CurrentPiece = null;
        }
    }

    public bool TryGetSpawnCell(int startRow, int endRow, int rowStep, out int spawnX, out int spawnY)
    {
        int width = ChessBoard.GetLength(0);

        for (int y = startRow; y != endRow; y += rowStep)
        {
            for (int offset = 0; offset < width; offset++)
            {
                int x;

                if (offset % 2 == 0)
                {
                    x = (width - 1) / 2 - offset / 2;
                }
                else
                {
                    x = width / 2 + offset / 2;
                }

                if (ChessBoard[x, y].IsEmpty)
                {
                    spawnX = x;
                    spawnY = y;
                    return true;
                }
            }
        }

        spawnX = -1;
        spawnY = -1;
        return false;
    }

    public bool IsPathClear(int fromX, int fromY, int toX, int toY)
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

    public bool TryMovePiece(int fromX, int fromY, int toX, int toY, out ChessPiece capturedPiece)
    {
        capturedPiece = null;
        if (!IsLegalMove(fromX, fromY, toX, toY))
        {
            return false;
        }

        Cell fromCell = ChessBoard[fromX, fromY];
        Cell toCell = ChessBoard[toX, toY];

        capturedPiece = toCell.CurrentPiece;
        toCell.CurrentPiece = fromCell.CurrentPiece;
        fromCell.CurrentPiece = null;

        return true;
    }

    public bool IsLegalMove(int fromX, int fromY, int toX, int toY)
    {
        if (!IsInsideBoard(fromX, fromY) ||
            !IsInsideBoard(toX, toY))
        {
            return false;
        }

        if (fromX == toX && fromY == toY)
        {
            return false;
        }

        Cell fromCell = ChessBoard[fromX, fromY];
        Cell toCell = ChessBoard[toX, toY];

        if (fromCell.IsEmpty)
        {
            return false;
        }

        ChessPiece movingPiece = fromCell.CurrentPiece;
        ChessPiece targetPiece = toCell.CurrentPiece;

        if (targetPiece != null &&
            targetPiece.Color == movingPiece.Color)
        {
            return false;
        }

        return movingPiece.CanMove(
                this,
                fromX,
                fromY,
                toX,
                toY);
    }

    public bool TryFindPiece<T>(out int pieceX, out int pieceY) where T : ChessPiece
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (ChessBoard[x, y].CurrentPiece is T)
                {
                    pieceX = x;
                    pieceY = y;
                    return true;
                }
            }
        }

        pieceX = -1;
        pieceY = -1;
        return false;
    }

    public Board CreateCopy()
    {
        Board copy = new Board();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                copy.ChessBoard[x, y].CurrentPiece = ChessBoard[x, y].CurrentPiece;
            }
        }
        return copy;
    }

    public void ClearBoard()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                ChessBoard[x, y].CurrentPiece = null;
            }
        }
    }
}
