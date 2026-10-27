using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    private const int BoardSize = 8;

    [SerializeField] private Tilemap groundTilemap;

    private Vector3Int Origin => groundTilemap.origin;

    public Vector3Int GetTilePosition(int x, int y)
    {
        int reversedY = BoardSize - 1 - y;
        return Origin + new Vector3Int(x, reversedY, 0);
    }

    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < BoardSize &&
               y >= 0 && y < BoardSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3Int tilePosition = GetTilePosition(x, y);

        return groundTilemap.GetCellCenterWorld(tilePosition);
    }

    public bool TryGetBoardPosition(Vector3 worldPosition, out int x, out int y)
    {
        Vector3Int tilePosition = groundTilemap.WorldToCell(worldPosition);

        x = tilePosition.x - Origin.x;
        int tileY = tilePosition.y - Origin.y;
        y = BoardSize - 1 - tileY;

        return IsInsideBoard(x, y);
    }
}
