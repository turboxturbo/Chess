
using System;

public class MoveEvaluator
{
    private readonly IShotgunThreatProvider _shotgunThreatProvider;
    private readonly MoveGenerator _moveGenerator;
    private readonly MoveEvaluationWeights _weights;

    public MoveEvaluator(IShotgunThreatProvider shotgunThreatProvider, MoveGenerator moveGenerator, MoveEvaluationWeights weights)
    {
        _shotgunThreatProvider = shotgunThreatProvider;
        _moveGenerator = moveGenerator;
        _weights = weights;
    }

    public float Evaluate(Board board, Move move, ChessPiece lastMovedPiece = null)
    {
        if (!board.IsLegalMove(move.FromX, move.FromY, move.ToX, move.ToY))
        {
            return float.NegativeInfinity;
        }

        ChessPiece movingPiece = board.ChessBoard[move.FromX, move.FromY].CurrentPiece;
        ChessPiece targetPiece = board.ChessBoard[move.ToX, move.ToY].CurrentPiece;

        if (targetPiece is BlackKing)
        {
            return float.PositiveInfinity;
        }

        int mobilityBefore = _moveGenerator.GenerateMoves(board, PieceColor.White).Count;

        Board simulatedBoard = board.CreateCopy();

        if (!simulatedBoard.TryMovePiece(move.FromX, move.FromY, move.ToX, move.ToY, out _))
        {
            return float.NegativeInfinity;
        }

        if (!simulatedBoard.TryFindPiece<BlackKing>(out int kingX, out int kingY))
        {
            return float.PositiveInfinity;
        }

        float attackScore = EvaluateAttack(simulatedBoard, movingPiece, move.ToX, move.ToY, kingX, kingY);
        float positionScore = EvaluatePosition(simulatedBoard, movingPiece, move.ToX, move.ToY, kingX, kingY);
        float safetyScore = EvaluateSafety(simulatedBoard, movingPiece, move.ToX, move.ToY, kingX, kingY);
        float cooperationScore = EvaluateCooperation(simulatedBoard, mobilityBefore);
        float recentPenalty = movingPiece == lastMovedPiece ? -_weights.RecentlyMovedPenalty : 0f;

        return attackScore + positionScore + safetyScore + cooperationScore + recentPenalty;
    }

    private float EvaluateAttack(Board board, ChessPiece movingPiece, int pieceX, int pieceY, int kingX, int kingY)
    {
        float score = 0f;
        int attackingPieces = CountKingAttackers(board, kingX, kingY);

        score += attackingPieces * _weights.KingThreatWeight;

        int dx = Math.Abs(kingX - pieceX);
        int dy = Math.Abs(kingY - pieceY);

        if (movingPiece is Rook && (pieceX == kingX || pieceY == kingY))
        {
            score += _weights.LineAlignmentWeight;
        }

        if (movingPiece is Bishop && dx == dy)
        {
            score += _weights.LineAlignmentWeight;
        }

        if (movingPiece is Queen && (pieceX == kingX || pieceY == kingY || dx == dy))
        {
            score += _weights.LineAlignmentWeight;
        }

        return score;
    }

    private float EvaluatePosition(Board board, ChessPiece movingPiece, int pieceX, int pieceY, int kingX, int kingY)
    {
        float centerX = (board.Width - 1) / 2f;
        float centerY = (board.Height - 1) / 2f;

        float centerDistance = Math.Abs(pieceX - centerX) + Math.Abs(pieceY - centerY);
        float centerScore = -centerDistance * _weights.CenterWeight;

        int kingDistance = Math.Max(Math.Abs(kingX - pieceX), Math.Abs(kingY - pieceY));
        float distanceMultiplier = GetDistanceMultiplier(movingPiece);
        float distanceScore = -kingDistance * _weights.KingDistanceWeight * distanceMultiplier;

        int mobility = CountPieceMoves(board, pieceX, pieceY);
        float mobilityScore = mobility * _weights.MobilityWeight;

        return centerScore + distanceScore + mobilityScore;
    }

    private float EvaluateSafety(Board board, ChessPiece movingPiece, int pieceX, int pieceY, int kingX, int kingY)
    {
        float score = 0f;
        float pieceValue = GetPieceValue(movingPiece);

        if (board.IsLegalMove(kingX, kingY, pieceX, pieceY))
        {
            score -= _weights.KingCaptureRiskWeight * pieceValue;
        }

        float shotgunDanger = Math.Max(0f, _shotgunThreatProvider.GetDanger(board, pieceX, pieceY));
        score -= shotgunDanger * _weights.ShotgunDangerWeight * pieceValue;

        return score;
    }

    private float EvaluateCooperation(Board board, int mobilityBefore)
    {
        int mobilityAfter = _moveGenerator.GenerateMoves(board, PieceColor.White).Count;
        int mobilityDifference = mobilityAfter - mobilityBefore;

        return mobilityDifference * _weights.CooperationWeight;
    }

    private int CountKingAttackers(Board board, int kingX, int kingY)
    {
        int count = 0;

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;

                if (piece == null || piece.Color != PieceColor.White)
                {
                    continue;
                }

                if (board.IsLegalMove(x, y, kingX, kingY))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private int CountPieceMoves(Board board, int fromX, int fromY)
    {
        int count = 0;

        for (int toX = 0; toX < board.Width; toX++)
        {
            for (int toY = 0; toY < board.Height; toY++)
            {
                if (board.IsLegalMove(fromX, fromY, toX, toY))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private float GetDistanceMultiplier(ChessPiece piece)
    {
        if (piece is Pawn)
        {
            return 1.4f;
        }

        if (piece is Knight)
        {
            return 1.1f;
        }

        if (piece is WhiteKing)
        {
            return 1.2f;
        }

        return 0.4f;
    }

    private float GetPieceValue(ChessPiece piece)
    {
        if (piece is Pawn)
        {
            return 1f;
        }

        if (piece is Knight || piece is Bishop)
        {
            return 3f;
        }

        if (piece is Rook)
        {
            return 5f;
        }

        if (piece is Queen)
        {
            return 9f;
        }

        if (piece is WhiteKing)
        {
            return 12f;
        }

        return 1f;
    }
}
