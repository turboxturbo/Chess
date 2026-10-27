using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PieceView _knightPrefab;
    [SerializeField] private PieceView _whitekingPrefab;
    [SerializeField] private PieceView _blackkingPrefab;
    [SerializeField] private PieceView _bishopPrefab;
    [SerializeField] private PieceView _pawnPrefab;
    [SerializeField] private PieceView _rookPrefab;
    [SerializeField] private PieceView _queenPrefab;

    private readonly Dictionary<ChessPiece, PieceView> _pieceViews = new();

    public void Initialize(Board board)
    {
        if (board == null)
        {
            Debug.LogError("Game board is not initialized.");
            return;
        }

        CreatePieces(board);
    }

    public void Register(PieceView pieceView)
    {
        if (pieceView == null || pieceView.Piece == null)
        {
            return;
        }

        _pieceViews[pieceView.Piece] = pieceView;
    }

    private PieceView GetPrefab(ChessPiece piece)
    {
        return piece switch
        {
            WhiteKing => _whitekingPrefab,
            BlackKing => _blackkingPrefab,
            Pawn => _pawnPrefab,
            Knight => _knightPrefab,
            Bishop => _bishopPrefab,
            Rook => _rookPrefab,
            Queen => _queenPrefab,
            _ => null
        };
    }

    public void CreatePieces(Board board)
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                ChessPiece piece = board.ChessBoard[x, y].CurrentPiece;

                if (piece == null)
                {
                    continue;
                }

                PieceView prefab = GetPrefab(piece);

                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found for {piece.GetType().Name}");
                    continue;
                }

                PieceView pieceView = Instantiate(prefab, transform);
                pieceView.Initialize(x, y, piece, boardManager);
                Register(pieceView);
            }
        }
    }

    public void ApplyMove(PieceView movingPieceView, int toX, int toY, ChessPiece capturedPiece)
    {
        if (capturedPiece != null && _pieceViews.TryGetValue(capturedPiece, out PieceView capturedView))
        {
            _pieceViews.Remove(capturedPiece);
            Destroy(capturedView.gameObject);
        }

        movingPieceView.SetBoardPosition(toX, toY, boardManager);
    }
    public void ReturnToCell(PieceView pieceView)
    {
        pieceView.SetBoardPosition(pieceView.BoardX, pieceView.BoardY, boardManager);
    }
}
