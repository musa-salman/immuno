using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class PipeSystem : MonoBehaviour
{
    [Header("Tile Placement Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TilePrefabPair> tilePrefabMappings;

    [Header("Runtime References")]
    [SerializeField] private Pipe[] pipes;

    private HiddenRoomRevealer hiddenRoomRevealer;
    private int totalPipes = 0;
    private bool isPropagating = false;

    private void Start()
    {
        PlacePrefabsFromTilemap();

        totalPipes = transform.childCount;
        pipes = new Pipe[totalPipes];

        for (int i = 0; i < totalPipes; i++)
        {
            pipes[i] = transform.GetChild(i).GetComponent<Pipe>();
        }

        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();
    }

    private void PlacePrefabsFromTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isPropagating)
            {
                Debug.LogWarning("Acid propagation is already in progress.");
                return;
            }
            isPropagating = true;
            EmptyPipes();
            StartCoroutine(PropagateAcidBFSConcurrent(0.1f));
        }
    }

    private IEnumerator PropagateAcidBFSConcurrent(float delayPerStep)
    {
        Queue<Pipe> queue = new();
        HashSet<Pipe> visited = new();

        foreach (Pipe pipe in pipes)
        {
            if (pipe.isFilled)
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

            foreach (Pipe neighbor in current.cachedNeighbors)
            {
                if (!neighbor.isFilled)
                {
                    neighbor.SetFilled(true);
                    queue.Enqueue(neighbor);
                }
            }

            yield return new WaitForSeconds(delayPerStep);
        }

        if (IsPuzzleSolved())
        {
            hiddenRoomRevealer.DonePuzzle(transform);
        }
        isPropagating = false;
    }

    private void EmptyPipes()
    {
        foreach (Pipe pipe in pipes)
        {
            if (!pipe.isStartPoint)
            {
                pipe.SetFilled(false);
            }
        }
    }

    private bool IsPuzzleSolved()
    {
        foreach (Pipe pipe in pipes)
        {
            if (!pipe.isFilled || !pipe.IsAllConnectorsConnected())
            {
                return false;
            }
        }

        return !HasCycle(pipes[0], null, new HashSet<Pipe>());
    }

    private bool HasCycle(Pipe current, Pipe parent, HashSet<Pipe> visited)
    {
        visited.Add(current);

        foreach (Pipe neighbor in current.cachedNeighbors)
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
        return false;
    }

}
