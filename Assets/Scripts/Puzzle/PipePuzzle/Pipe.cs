using UnityEngine;
using System.Collections.Generic;

public class Pipe : MonoBehaviour
{
    public enum Direction { Up, Right, Down, Left }
    private GameObject acidFillObject;

    [Header("Pipe configuration")]
    public Direction[] baseConnections;

    private Direction[] currentConnections;

    [Header("Pipe state")]
    public bool isFilled = false;
    public bool isStartPoint = false;

    [Header("Connection Points (auto-assigned)")]
    public PipeConnector[] connectionPoints;

    [Header("Cached neighbors")]
    public List<Pipe> cachedNeighbors = new();

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);

        currentConnections = new Direction[baseConnections.Length];
        baseConnections.CopyTo(currentConnections, 0);

        // Collect and sort connectors
        List<PipeConnector> foundConnections = new();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<PipeConnector>(out var connector))
            {
                foundConnections.Add(connector);
            }
        }

        connectionPoints = new PipeConnector[baseConnections.Length];

        for (int i = 0; i < baseConnections.Length; i++)
        {
            foreach (PipeConnector connector in foundConnections)
            {
                if (connector.direction == baseConnections[i])
                {
                    connectionPoints[i] = connector;
                    break;
                }
            }
        }

        int randomRotations = Random.Range(1, 4);
        for (int i = 0; i < randomRotations; i++)
        {
            RotatePipe();
        }

        if (isStartPoint)
        {
            isFilled = true;
        }

        acidFillObject = transform.Find("AcidFill").gameObject;

        UpdateSprite();
    }

    public bool IsAllConnectorsConnected()
    {
        return cachedNeighbors.Count == connectionPoints.Length;
    }

    private void OnMouseDown()
    {
        RotatePipe();
    }

    public void RotatePipe()
    {
        transform.Rotate(0, 0, 90f);
        UpdateConnections();
    }

    private void UpdateConnections()
    {
        for (int i = 0; i < baseConnections.Length; i++)
        {
            currentConnections[i] = (Direction)(((int)baseConnections[i] + GetRotationSteps()) % 4);
        }

        foreach (PipeConnector connector in connectionPoints)
        {
            if (connector != null)
            {
                connector.direction = (Direction)(((int)connector.baseDirection - GetRotationSteps() + 4) % 4);
                connector.SetDirection(connector.direction);
            }
        }

        foreach (PipeConnector connection in connectionPoints)
        {
            connection.TryConnect();

        }
    }

    private int GetRotationSteps()
    {
        int steps = Mathf.RoundToInt(transform.eulerAngles.z / 90f) % 4;
        return steps;
    }



    public void SetFilled(bool filled)
    {
        isFilled = filled;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        acidFillObject.SetActive(isFilled);
    }


    public Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            _ => dir,
        };
    }

    public void AddNeighbor(Pipe neighbor)
    {
        if (!cachedNeighbors.Contains(neighbor))
        {
            cachedNeighbors.Add(neighbor);
        }
    }

    public void RemoveNeighbor(Pipe neighbor)
    {
        if (cachedNeighbors.Contains(neighbor))
        {
            cachedNeighbors.Remove(neighbor);
        }
    }
}
