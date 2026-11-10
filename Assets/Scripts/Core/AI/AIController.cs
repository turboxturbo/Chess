
using System;
using System.Collections.Generic;

public class AIController
{
    private readonly MoveGenerator _moveGenerator;
    private readonly MoveEvaluator _moveEvaluator;
    private readonly MoveEvaluationWeights _weights;
    private readonly Random _random;
    private readonly int _movesPerRound;
    private ChessPiece _lastMovedPiece;
    public event Action<Move, ChessPiece> MoveExecuted;
    public event Action<ChessPiece, ChessPiece, int, int> PawnPromoted; // oldPawn, newPiece, x, y

    public AIController(MoveGenerator moveGenerator, MoveEvaluator moveEvaluator, MoveEvaluationWeights weights, int movesPerRound = 2, int? randomSeed = null)
    {
        _moveGenerator = moveGenerator;
        _moveEvaluator = moveEvaluator;
        _weights = weights;
        _movesPerRound = movesPerRound;
        _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
    }

    public bool MakeTurn(Board board)
    {
        HashSet<ChessPiece> movedPieces = new HashSet<ChessPiece>();

        for (int actionIndex = 0; actionIndex < _movesPerRound; actionIndex++)
        {
            List<Move> availableMoves = _moveGenerator.GenerateMoves(board, PieceColor.White, movedPieces);

            if (availableMoves.Count == 0)
            {
                break;
            }

            if (TryFindKingCapture(board, availableMoves, out Move killingMove))
            {
                return ExecuteMove(board, killingMove, movedPieces);
            }

            if (!TryChooseBestMove(board, availableMoves, out Move bestMove))
            {
                break;
            }

            _lastMovedPiece = board.ChessBoard[bestMove.FromX, bestMove.FromY].CurrentPiece;
            bool blackKingCaptured = ExecuteMove(board, bestMove, movedPieces);

            if (blackKingCaptured)
            {
                return true;
            }
        }

        return false;
    }

    private bool ExecuteMove(Board board, Move move, HashSet<ChessPiece> movedPieces)
    {
        ChessPiece movingPiece = board.ChessBoard[move.FromX, move.FromY].CurrentPiece;

        if (movingPiece == null)
        {
            return false;
        }

        if (!board.TryMovePiece(move.FromX, move.FromY, move.ToX, move.ToY, out ChessPiece capturedPiece))
        {
            return false;
        }

        movedPieces.Add(movingPiece);
        MoveExecuted?.Invoke(move, capturedPiece);

        TryPromote(board, move.ToX, move.ToY);

        return capturedPiece is BlackKing;
    }

    private void TryPromote(Board board, int x, int y)
    {
        ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;
        if (piece is not Pawn || piece.Color != PieceColor.White) return;
        if (y < board.Height - 1) return;

        ChessPiece promoted = ChoosePromotion(board, x, y, piece.Health);
        ChessPiece oldPawn = board.PromotePawn(x, y, promoted);
        PawnPromoted?.Invoke(oldPawn, promoted, x, y);
    }

    private ChessPiece ChoosePromotion(Board board, int x, int y, int health)
    {
        if (!board.TryFindPiece<BlackKing>(out int kingX, out int kingY))
            return new Queen(PieceColor.White, health);

        int distToKing = System.Math.Max(System.Math.Abs(kingX - x), System.Math.Abs(kingY - y));

        return distToKing <= 2
            ? new Knight(PieceColor.White, health)
            : new Queen(PieceColor.White, health);
    }

    private bool TryFindKingCapture(Board board, List<Move> moves, out Move killingMove)
    {
        foreach (Move move in moves)
        {
            ChessPiece target = board.ChessBoard[move.ToX, move.ToY].CurrentPiece;

            if (target is BlackKing)
            {
                killingMove = move;
                return true;
            }
        }

        killingMove = default;
        return false;
    }

    private bool TryChooseBestMove(Board board, List<Move> moves, out Move bestMove)
    {
        List<ScoredMove> scoredMoves = new List<ScoredMove>();
        float bestScore = float.NegativeInfinity;

        foreach (Move move in moves)
        {
            float score = _moveEvaluator.Evaluate(board, move, _lastMovedPiece);
            scoredMoves.Add(new ScoredMove(move, score));

            if (score > bestScore)
            {
                bestScore = score;
            }
        }

        List<Move> nearBestMoves = new List<Move>();
        float minimumAcceptedScore = bestScore - _weights.NearBestTolerance;

        foreach (ScoredMove scoredMove in scoredMoves)
        {
            if (scoredMove.Score >= minimumAcceptedScore)
            {
                nearBestMoves.Add(scoredMove.Move);
            }
        }

        if (nearBestMoves.Count == 0)
        {
            bestMove = default;
            return false;
        }

        bestMove = nearBestMoves[_random.Next(nearBestMoves.Count)];
        return true;
    }

    private readonly struct ScoredMove
    {
        public Move Move { get; }
        public float Score { get; }
        public ScoredMove(Move move, float score)
        {
            Move = move;
            Score = score;
        }
    }
}
