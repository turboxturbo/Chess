using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    private readonly Dictionary<ChessPiece, PieceView> _pieceViews = new();
    public void Register(PieceView pieceView)
    {
        if (pieceView.Piece == null || pieceView == null)
        {
            return;
        }

        _pieceViews[pieceView.Piece] = pieceView;
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
