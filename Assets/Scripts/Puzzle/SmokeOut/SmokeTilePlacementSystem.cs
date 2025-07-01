using UnityEngine;
using UnityEngine.Tilemaps;

public class SmokePlacementSystem : MonoBehaviour
{
    [Header("Tilemap Placement Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject smokePrefab;

    public void PlaceAllSmokeTiles(SmokePuzzleManager puzzleManager)
    {
        BoundsInt bounds = tilemap.cellBounds;

        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }
        }

        int gridWidth = maxX - minX + 1;
        int gridHeight = maxY - minY + 1;

        puzzleManager.gridRows = gridWidth;
        puzzleManager.gridColumns = gridHeight;
        puzzleManager.gridOrigin = new Vector2Int(minX, minY);
        puzzleManager.tiles = new Smoke[gridWidth, gridHeight];

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2f;
                GameObject spawned = Instantiate(smokePrefab, worldPos, Quaternion.identity, puzzleManager.transform);
                spawned.transform.position = new Vector3(spawned.transform.position.x, spawned.transform.position.y, 1f);

                if (spawned.TryGetComponent<Smoke>(out var smoke))
                {
                    int gridCol = pos.x - minX;
                    int gridRow = maxY - pos.y;

                    smoke.gridCol = gridCol;
                    smoke.gridRow = gridRow;
                    smoke.manager = puzzleManager;

                    puzzleManager.tiles[gridCol, gridRow] = smoke;

                    if (tile.name.Equals("fogs_7"))
                        smoke.Clear();
                    else
                        smoke.Unclear();
                }

                tilemap.SetTile(pos, null);
            }
        }
    }
}
