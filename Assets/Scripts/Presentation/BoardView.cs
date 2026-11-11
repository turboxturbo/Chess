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
    [Header("Drop Animation")]
    [SerializeField] private float dropHeight = 8f;
    [SerializeField] private float dropDelay = 0.1f;
    [SerializeField] private float blackKingDropDelay = 1.2f;

    private int _spawnIndex;
    private Board _board;

    private readonly Dictionary<ChessPiece, PieceView> _pieceViews = new();

    public void Initialize(Board board)
    {
        if (board == null)
        {
            Debug.LogError("Game board is not initialized.");
            return;
        }

        _board = board;
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
                float delay = piece is BlackKing ? blackKingDropDelay : 0f;
                pieceView.InitializeDrop(x, y, piece, boardManager, delay, dropHeight);
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

    public void ApplyMove(Move move, ChessPiece capturedPiece)
    {
        if (_board == null) return;

        ChessPiece movingPiece = _board.ChessBoard[move.ToX, move.ToY].CurrentPiece;

        if (movingPiece == null ||
            !_pieceViews.TryGetValue(movingPiece, out PieceView movingPieceView))
        {
            Debug.LogError("Moving PieceView was not found.");
            return;
        }

        if (capturedPiece != null && _pieceViews.TryGetValue(capturedPiece, out PieceView capturedView))
        {
            _pieceViews.Remove(capturedPiece);
            Destroy(capturedView.gameObject);
        }

        movingPieceView.AnimateTo(move.ToX, move.ToY, boardManager);
    }

    public bool IsAnyPieceMoving()
    {
        foreach (var kv in _pieceViews)
        {
            if (kv.Value != null && kv.Value.IsMoving) return true;
        }
        return false;
    }
    [SerializeField] private GameObject damagePopupPrefab;

    public void ApplyShot(Dictionary<ChessPiece, int> damage)
    {
        foreach (var hit in damage)
        {
            if (!_pieceViews.TryGetValue(hit.Key, out PieceView view)) continue;

            if (hit.Key.Health == 0)
            {
                _pieceViews.Remove(hit.Key);
                Destroy(view.gameObject);
            }
            else
            {
                view.ShowHitEffect(hit.Value, damagePopupPrefab);
            }
        }
    }

    public void SpawnPieceView(ChessPiece piece, int x, int y)
    {
        PieceView prefab = GetPrefab(piece);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found for {piece.GetType().Name}");
            return;
        }

        PieceView pieceView = Instantiate(prefab, transform);
        pieceView.InitializeDrop(x, y, piece, boardManager, _spawnIndex * dropDelay, dropHeight);
        _spawnIndex++;
        Register(pieceView);
    }

    public void ResetSpawnIndex() => _spawnIndex = 0;

    public void ClearWhitePieces()
    {
        var toRemove = new System.Collections.Generic.List<ChessPiece>();
        foreach (var kv in _pieceViews)
        {
            if (kv.Key.Color == PieceColor.White)
                toRemove.Add(kv.Key);
        }
        foreach (var piece in toRemove)
        {
            DestroyImmediate(_pieceViews[piece].gameObject);
            _pieceViews.Remove(piece);
        }
    }

    public void ApplyPromotion(ChessPiece oldPawn, ChessPiece newPiece, int x, int y)
    {
        if (_pieceViews.TryGetValue(oldPawn, out PieceView pawnView))
        {
            _pieceViews.Remove(oldPawn);
            Destroy(pawnView.gameObject);
        }

        SpawnPieceView(newPiece, x, y);
    }

    public void ResetBlackKingView(int x, int y)
    {
        foreach (var kv in _pieceViews)
        {
            if (kv.Key is BlackKing)
            {
                kv.Value.DropIn(x, y, boardManager, blackKingDropDelay, dropHeight);
                return;
            }
        }
    }

    public void ReturnToCell(PieceView pieceView)
    {
        pieceView.SetBoardPosition(pieceView.BoardX, pieceView.BoardY, boardManager);
    }
}
