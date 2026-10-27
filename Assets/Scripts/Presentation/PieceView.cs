using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PieceView : MonoBehaviour
{
    public ChessPiece Piece { get; private set; }
    public int BoardX { get; private set; }
    public int BoardY { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y, ChessPiece piece, BoardManager boardManager)
    {
        Piece = piece;
        SetBoardPosition(x, y, boardManager);
    }

    public void SetBoardPosition(int x, int y, BoardManager boardManager)
    {
        BoardX = x;
        BoardY = y;
        transform.position = boardManager.GetWorldPosition(x, y);
    }
}
