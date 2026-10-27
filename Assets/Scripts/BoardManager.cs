using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;

    private readonly Vector3Int _origin = new(-4, -5, 0);


    public Vector3Int GetTilePosition(int x, int y)
    {
        return _origin + new Vector3Int(x, y, 0);
    }

    public bool IsInsideBoard(int x, int y)
    {
        return x >= 0 && x < 8 &&
               y >= 0 && y < 8;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3Int tilePosition = GetTilePosition(x, y);

        return groundTilemap.GetCellCenterWorld(tilePosition);
    }


    public bool TryGetBoardPosition(Vector3 worldPosition, out int x, out int y)
    {
        Vector3Int tilePosition = groundTilemap.WorldToCell(worldPosition);

        x = tilePosition.x - _origin.x;
        y = tilePosition.y - _origin.y;

        return IsInsideBoard(x, y);
    }

}
