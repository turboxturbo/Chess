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

    public Vector3 BoardToWorld(float x, float y)
    {
        return GetWorldPosition(0, 0) +
            (GetWorldPosition(1, 0) - GetWorldPosition(0, 0)) * x +
            (GetWorldPosition(0, 1) - GetWorldPosition(0, 0)) * y;
    }

    public Vector2 WorldToBoard(Vector3 worldPosition)
    {
        Vector3 local = groundTilemap.transform.InverseTransformPoint(worldPosition);
        Vector3 zero = groundTilemap.transform.InverseTransformPoint(GetWorldPosition(0, 0));
        Vector3 xAxis = groundTilemap.transform.InverseTransformVector(GetWorldPosition(1, 0) - GetWorldPosition(0, 0));
        Vector3 yAxis = groundTilemap.transform.InverseTransformVector(GetWorldPosition(0, 1) - GetWorldPosition(0, 0));
        return new Vector2((local.x - zero.x) / xAxis.x, (local.y - zero.y) / yAxis.y);
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
