
using System.Collections.Generic;

public class MoveGenerator
{
    public List<Move> GenerateMoves(Board board, PieceColor color, ISet<ChessPiece> excludedPieces = null)
    {
        List<Move> moves = new List<Move>();

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;

                if (piece == null || piece.Color != color)
                {
                    continue;
                }

                if (excludedPieces != null && excludedPieces.Contains(piece))
                {
                    continue;
                }

                AddPieceMoves(board, x, y, moves);
            }
        }

        return moves;
    }

    public void AddPieceMoves(Board board, int fromX, int fromY, List<Move> moves)
    {
        for (int toX = 0; toX < board.Width; toX++)
        {
            for (int toY = 0; toY < board.Height; toY++)
            {
                if (board.IsLegalMove(fromX, fromY, toX, toY))
                {
                    moves.Add(new Move(fromX, fromY, toX, toY));
                }
            }
        }
    }

}
