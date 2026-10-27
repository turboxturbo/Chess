using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerInputController : MonoBehaviour
{
    public event Action PlayerMoveCompleted;
    private Board _board;
    private BoardManager _boardManager;
    private BoardView _boardView;
    private Camera _worldCamera;
    private PieceView _selectedPiece;
    private bool _isInitialized;
    private bool _inputEnabled;

    public void Initialize(
        Board board,
        BoardManager boardManager,
        BoardView boardView,
        Camera worldCamera)
    {
        _board = board;
        _boardManager = boardManager;
        _boardView = boardView;
        _worldCamera = worldCamera;

        _isInitialized =
            _board != null &&
            _boardManager != null &&
            _boardView != null &&
            _worldCamera != null;

        _inputEnabled = _isInitialized;

        if (!_isInitialized)
        {
            Debug.LogError(
                "PlayerInputController dependencies are not assigned.");
        }
    }

    public void SetInputEnabled(bool inputEnabled)
    {
        _inputEnabled = inputEnabled;
    }

    private void Update()
    {
        if (!_isInitialized ||
            !_inputEnabled ||
            Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TrySelectPiece();
        }

        if (_selectedPiece != null &&
            Mouse.current.leftButton.isPressed)
        {
            DragSelectedPiece();
        }

        if (_selectedPiece != null &&
            Mouse.current.leftButton.wasReleasedThisFrame)
        {
            TryReleasePiece();
        }
    }

    private void TrySelectPiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D hit in hits)
        {
            PieceView pieceView = hit.GetComponentInParent<PieceView>();

            if (pieceView == null || pieceView.Piece is not BlackKing)
            {
                continue;
            }

            _selectedPiece = pieceView;
            return;
        }
    }

    private void DragSelectedPiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        worldPosition.z = _selectedPiece.transform.position.z;

        _selectedPiece.transform.position = worldPosition;
    }

    private void TryReleasePiece()
    {
        Vector3 worldPosition = GetMouseWorldPosition();

        if (!_boardManager.TryGetBoardPosition(
                worldPosition,
                out int toX,
                out int toY))
        {
            ReturnSelectedPiece();
            return;
        }

        int fromX = _selectedPiece.BoardX;
        int fromY = _selectedPiece.BoardY;

        bool moveSucceeded = _board.TryMovePiece(
            fromX,
            fromY,
            toX,
            toY,
            out ChessPiece capturedPiece);

        if (!moveSucceeded)
        {
            ReturnSelectedPiece();
            return;
        }

        _boardView.ApplyMove(
            _selectedPiece,
            toX,
            toY,
            capturedPiece);

        _selectedPiece = null;
        PlayerMoveCompleted?.Invoke();
    }

    private void ReturnSelectedPiece()
    {
        _boardView.ReturnToCell(_selectedPiece);
        _selectedPiece = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(
            mousePosition.x,
            mousePosition.y,
            -_worldCamera.transform.position.z);

        return _worldCamera.ScreenToWorldPoint(screenPosition);
    }
}
