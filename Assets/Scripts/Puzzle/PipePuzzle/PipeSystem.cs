using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PipeSystem : MonoBehaviour
{
    [Header("Tile Placement Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TilePrefabPair> tilePrefabMappings;

    public Pipe[,] grid;
    public int rows;
    public int cols;

    private HiddenRoomRevealer hiddenRoomRevealer;

    private bool isPropagating = false;
    private int startRow;
    private int startCol;


    private void Start()
    {
        PlacePrefabsFromTilemap();

        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();
    }

    private void PlacePrefabsFromTilemap()
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

        cols = maxX - minX + 1;
        rows = maxY - minY + 1;

        grid = new Pipe[rows, cols];

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2f;

                GameObject prefabToSpawn = GetPrefabForTile(tile);
                if (prefabToSpawn != null)
                {
                    GameObject spawned = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
                    spawned.transform.position = new Vector3(spawned.transform.position.x, spawned.transform.position.y, 1f);

                    Pipe pipe = spawned.GetComponent<Pipe>();
                    pipe.pipeSystem = this;

                    int gridRow = maxY - pos.y;
                    int gridCol = pos.x - minX;

                    if (pipe.isStartPoint)
                    {
                        startRow = gridRow;
                        startCol = gridCol;
                    }
                    pipe.row = gridRow;
                    pipe.col = gridCol;

                    grid[gridRow, gridCol] = pipe;
                }

                tilemap.SetTile(pos, null);
            }
        }
    }

    private GameObject GetPrefabForTile(TileBase tile)
    {
        foreach (var mapping in tilePrefabMappings)
        {
            if (mapping.tile == tile)
                return mapping.prefab;
        }
        return null;
    }

    public void PropagateAcid()
    {
        if (isPropagating)
            return;
        isPropagating = true;
        EmptyPipes();

        Queue<Pipe> queue = new();
        HashSet<Pipe> visited = new();

        // Enqueue all start point pipes
        foreach (Pipe pipe in grid)
        {
            if (pipe != null && pipe.isFilled)
            {
                queue.Enqueue(pipe);
            }
        }

        while (queue.Count > 0)
        {
            Pipe current = queue.Dequeue();

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            // Check four neighbors
            TryPropagateToNeighbor(current, Pipe.Direction.Up, current.row - 1, current.col, queue, visited);
            TryPropagateToNeighbor(current, Pipe.Direction.Down, current.row + 1, current.col, queue, visited);
            TryPropagateToNeighbor(current, Pipe.Direction.Left, current.row, current.col - 1, queue, visited);
            TryPropagateToNeighbor(current, Pipe.Direction.Right, current.row, current.col + 1, queue, visited);
        }

        if (IsPuzzleSolved())
        {
            hiddenRoomRevealer.DonePuzzle(transform);
        }
        isPropagating = false;
    }

    private void TryPropagateToNeighbor(Pipe current, Pipe.Direction dir, int neighborRow, int neighborCol, Queue<Pipe> queue,
                                        HashSet<Pipe> visited)
    {
        if (neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < cols)
        {
            Pipe neighbor = grid[neighborRow, neighborCol];
            if (neighbor != null && !visited.Contains(neighbor))
            {
                if (current.HasConnection(dir) && neighbor.HasConnection(current.GetOppositeDirection(dir)))
                {
                    neighbor.SetFilled(true);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private void EmptyPipes()
    {
        foreach (Pipe pipe in grid)
        {
            if (!pipe.isStartPoint)
            {
                pipe.SetFilled(false);
            }
        }
    }

    private bool IsPuzzleSolved()
    {
        foreach (Pipe pipe in grid)
        {
            if (!pipe.isFilled)
            {
                return false;
            }

            if (!IsAllConnectorsConnected(pipe))
            {
                return false;

            }
        }

        return !HasCycle(grid[startRow, startCol], null, new HashSet<Pipe>());
    }

    private bool IsAllConnectorsConnected(Pipe pipe)
    {
        foreach (Pipe.Direction dir in pipe.currentConnections)
        {
            int neighborRow = pipe.row;
            int neighborCol = pipe.col;

            switch (dir)
            {
                case Pipe.Direction.Up: neighborRow -= 1; break;
                case Pipe.Direction.Down: neighborRow += 1; break;
                case Pipe.Direction.Left: neighborCol -= 1; break;
                case Pipe.Direction.Right: neighborCol += 1; break;
            }

            if (neighborRow < 0 || neighborRow >= rows || neighborCol < 0 || neighborCol >= cols)
            {
                return false;
            }

            Pipe neighbor = grid[neighborRow, neighborCol];
            if (neighbor == null)
            {
                return false;
            }

            if (!neighbor.HasConnection(pipe.GetOppositeDirection(dir)))
            {
                return false;
            }
        }
        return true;
    }

    private bool HasCycle(Pipe current, Pipe parent, HashSet<Pipe> visited)
    {
        visited.Add(current);

        foreach (Pipe.Direction dir in current.currentConnections)
        {
            int neighborRow = current.row;
            int neighborCol = current.col;

            switch (dir)
            {
                case Pipe.Direction.Up: neighborRow -= 1; break;
                case Pipe.Direction.Down: neighborRow += 1; break;
                case Pipe.Direction.Left: neighborCol -= 1; break;
                case Pipe.Direction.Right: neighborCol += 1; break;
            }

            if (neighborRow < 0 || neighborRow >= rows || neighborCol < 0 || neighborCol >= cols)
            {
                continue;
            }

            Pipe neighbor = grid[neighborRow, neighborCol];
            if (neighbor != null && neighbor.HasConnection(current.GetOppositeDirection(dir)))
            {
                if (!visited.Contains(neighbor))
                {
                    if (HasCycle(neighbor, current, visited))
                        return true;
                }
                else if (neighbor != parent)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
