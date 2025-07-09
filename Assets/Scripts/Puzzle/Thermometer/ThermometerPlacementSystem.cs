using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public class ThermometerPlacementSystem : MonoBehaviour
{
    [Header("Tilemap Placement Settings")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private List<TilePrefabPair> tilePrefabMappings;

    private Dictionary<Vector3Int, ThermometerSegment> placedSegments = new();

    public void PlaceAllSegments(ThermometerPuzzleManager puzzleManager)
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                GameObject prefabToSpawn = GetPrefabForTile(tile);
                if (prefabToSpawn != null)
                {
                    Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2f;
                    GameObject spawned = Instantiate(prefabToSpawn, worldPos, Quaternion.identity, transform);
                    spawned.transform.position = new Vector3(spawned.transform.position.x, spawned.transform.position.y, 1f);

                    if (spawned.TryGetComponent<ThermometerSegment>(out var segment))
                    {
                        placedSegments[pos] = segment;
                        segment.gridCol = pos.x;
                        segment.gridRow = pos.y;
                        segment.puzzleManager = puzzleManager;
                        tilemap.SetTile(pos, null);
                    }
                }
            }
        }

        // Adjust segments to start from (0,0) and flip rows if needed
        if (placedSegments.Count > 0)
        {
            Vector3Int minPos = new(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int maxPos = new(int.MinValue, int.MinValue, int.MinValue);

            foreach (var kvp in placedSegments)
            {
                if (kvp.Key.x < minPos.x) minPos.x = kvp.Key.x;
                if (kvp.Key.y < minPos.y) minPos.y = kvp.Key.y;

                if (kvp.Key.y > maxPos.y) maxPos.y = kvp.Key.y;
            }

            int maxRowIndex = maxPos.y - minPos.y;

            Dictionary<Vector3Int, ThermometerSegment> updatedSegments = new();
            foreach (var kvp in placedSegments)
            {
                Vector3Int newPos = kvp.Key - minPos;
                ThermometerSegment segment = kvp.Value;
                segment.gridCol = newPos.x;
                segment.gridRow = maxRowIndex - newPos.y;

                updatedSegments[newPos] = segment;
            }

            placedSegments = updatedSegments;
        }

        GroupThermometers(puzzleManager);
    }


    private void GroupThermometers(ThermometerPuzzleManager puzzleManager)
    {
        foreach (var kvp in placedSegments)
        {
            ThermometerSegment segment = kvp.Value;

            if (segment.type == ThermometerSegment.SegmentType.Head && segment.parentThermometer == null)
            {
                Thermometer thermo = CreateNewThermometer($"Thermometer_{kvp.Key.x}_{kvp.Key.y}");
                segment.parentThermometer = thermo;
                thermo.AddSegment(segment);

                Vector3Int currentPos = kvp.Key;
                Vector3Int nextPos = GetNextPos(currentPos, segment.openingDirection);

                while (placedSegments.ContainsKey(nextPos))
                {
                    ThermometerSegment nextSeg = placedSegments[nextPos];

                    // Prevent adding to multiple thermometers
                    if (nextSeg.parentThermometer != null) break;

                    // Validate correct type and orientation
                    if (!IsValidConnection(segment.openingDirection, nextSeg.type))
                    {
                        Debug.LogWarning($"Invalid thermometer connection at {nextPos}");
                        break;
                    }

                    nextSeg.parentThermometer = thermo;
                    thermo.AddSegment(nextSeg);

                    if (nextSeg.type == ThermometerSegment.SegmentType.Tail)
                        break; // Reached tail, done with this thermometer

                    nextPos = GetNextPos(nextPos, segment.openingDirection);
                }

                puzzleManager.thermometers.Add(thermo);
            }
        }
    }

    private bool IsValidConnection(ThermometerSegment.Direction dir, ThermometerSegment.SegmentType segType)
    {
        if (segType == ThermometerSegment.SegmentType.Tail) return true;
        if (dir == ThermometerSegment.Direction.Left || dir == ThermometerSegment.Direction.Right)
            return segType == ThermometerSegment.SegmentType.MiddleHorizontal;
        else
            return segType == ThermometerSegment.SegmentType.MiddleVertical;
    }

    private Vector3Int GetNextPos(Vector3Int current, ThermometerSegment.Direction dir)
    {
        return dir switch
        {
            ThermometerSegment.Direction.Up => current + Vector3Int.up,
            ThermometerSegment.Direction.Down => current + Vector3Int.down,
            ThermometerSegment.Direction.Left => current + Vector3Int.left,
            ThermometerSegment.Direction.Right => current + Vector3Int.right,
            _ => current,
        };
    }

    private Thermometer CreateNewThermometer(String name)
    {
        GameObject thermoObj = new(name);
        thermoObj.transform.parent = transform;
        return thermoObj.AddComponent<Thermometer>();
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
}
