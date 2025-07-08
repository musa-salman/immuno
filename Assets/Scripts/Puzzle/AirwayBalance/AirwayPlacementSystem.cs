using UnityEngine;
using UnityEngine.Tilemaps;

public class AirwayPlacementSystem : MonoBehaviour
{
    [Header("Placement Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject airwayPrefab;

    public void PlaceAllAirwaySegments(AirwayPuzzleManager puzzleManager)
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

        puzzleManager.gridRows = gridHeight;
        puzzleManager.gridColumns = gridWidth;
        puzzleManager.airways = new Airway[gridHeight, gridWidth];

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2f;
                GameObject spawned = Instantiate(airwayPrefab, worldPos, Quaternion.identity, puzzleManager.transform);

                if (spawned.TryGetComponent<Airway>(out var airway))
                {
                    int gridCol = pos.x - minX;
                    int gridRow = maxY - pos.y;

                    airway.gridRow = gridRow;
                    airway.gridCol = gridCol;
                    airway.manager = puzzleManager;

                    if (tile.name.Equals("lung_balance_puzzle_1"))
                    {
                        airway.SetState(Airway.State.Closed);
                        airway.isTarget = true;
                    }
                    else if (tile.name.Equals("lung_balance_puzzle_0"))
                    {
                        airway.SetState(Airway.State.Open);
                        airway.isTarget = true;
                    }
                    else
                    {
                        airway.SetState(Airway.State.Empty);
                        airway.isTarget = false;
                    }

                    Debug.Log($"Placed airway at grid ({gridRow},{gridCol}) with state {airway.currentState}");
                    puzzleManager.airways[gridRow, gridCol] = airway;
                }

                tilemap.SetTile(pos, null);
            }
        }
    }
}
