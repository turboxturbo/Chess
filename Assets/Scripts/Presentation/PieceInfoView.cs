using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceInfoView : MonoBehaviour
{
    [Header("Movement Map")]
    [SerializeField] private Image movementMapImage;

    [Header("Movement Map Sprites")]
    [SerializeField] private Sprite pawnMap;
    [SerializeField] private Sprite knightMap;
    [SerializeField] private Sprite bishopMap;
    [SerializeField] private Sprite rookMap;
    [SerializeField] private Sprite queenMap;
    [SerializeField] private Sprite kingMap;

    [Header("Piece Name")]
    [SerializeField] private TextMeshProUGUI pieceNameText;

    [Header("Health")] // максимум 8 слотов (WhiteKing)
    [SerializeField] private Image[] heartSlots;
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartEmpty;

    private void Start()
    {
        Hide();
    }

    public void Show(ChessPiece piece)
    {
        gameObject.SetActive(true);
        movementMapImage.sprite = GetMapSprite(piece);
        if (pieceNameText != null)
            pieceNameText.text = GetPieceName(piece);

        for (int i = 0; i < heartSlots.Length; i++)
        {
            heartSlots[i].gameObject.SetActive(true);
            heartSlots[i].sprite = i < piece.Health ? heartFull : heartEmpty;
        }
    }

    public void Hide() { }

    private Sprite GetMapSprite(ChessPiece piece)
    {
        return piece switch
        {
            Pawn   => pawnMap,
            Knight => knightMap,
            Bishop => bishopMap,
            Rook   => rookMap,
            Queen  => queenMap,
            _      => kingMap
        };
    }

    private static string GetPieceName(ChessPiece piece)
    {
        return piece switch
        {
            Pawn      => "PAWN",
            Knight    => "KNIGHT",
            Bishop    => "BISHOP",
            Rook      => "ROOK",
            Queen     => "QUEEN",
            WhiteKing => "KING",
            _         => ""
        };
    }
}
