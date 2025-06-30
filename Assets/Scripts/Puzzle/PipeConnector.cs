using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnector : MonoBehaviour
{
    private Pipe parentPipe;

    public Pipe.Direction baseDirection;
    public Pipe.Direction direction;


    private void Awake()
    {
        baseDirection = direction;
        parentPipe = GetComponentInParent<Pipe>();

        if (parentPipe == null)
        {
            Debug.LogError($"PipeConnector on {gameObject.name} has no parent Pipe!");
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryConnectWith(other);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (
            other.TryGetComponent<PipeConnector>(out var otherConnector) &&
            otherConnector.direction == parentPipe.GetOppositeDirection(direction))
        {
            Debug.Log($"Disconnecting {parentPipe.name} from {otherConnector.parentPipe.name} in direction {direction}");
            parentPipe.RemoveNeighbor(otherConnector.parentPipe);
        }
    }

    public void SetDirection(Pipe.Direction newDirection)
    {
        direction = newDirection;
    }

    public void TryConnect()
    {
        if (TryGetComponent<Collider2D>(out var col))
        {
            ContactFilter2D filter = new()
            {
                useTriggers = true
            };
            filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

            Collider2D[] results = new Collider2D[5];
            int count = Physics2D.OverlapCollider(col, filter, results);

            HashSet<Pipe> currentOverlappingNeighbors = new();

            for (int i = 0; i < count; i++)
            {
                PipeConnector otherConnector = results[i].GetComponent<PipeConnector>();
                if (otherConnector != null)
                {
                    if (otherConnector.direction == parentPipe.GetOppositeDirection(direction))
                    {
                        parentPipe.AddNeighbor(otherConnector.parentPipe);
                        otherConnector.parentPipe.AddNeighbor(parentPipe);
                        currentOverlappingNeighbors.Add(otherConnector.parentPipe);
                        Debug.Log($"TryConnect: Connected {parentPipe.name} to {otherConnector.parentPipe.name}");
                    }
                }
            }

            List<Pipe> neighborsToRemove = new();
            foreach (Pipe neighbor in parentPipe.cachedNeighbors)
            {
                if (!currentOverlappingNeighbors.Contains(neighbor))
                {
                    neighborsToRemove.Add(neighbor);
                    Debug.Log($"TryConnect: Disconnected {parentPipe.name} from {neighbor.name}");
                }
            }

            foreach (Pipe neighbor in neighborsToRemove)
            {
                parentPipe.RemoveNeighbor(neighbor);
                neighbor.RemoveNeighbor(parentPipe);
            }
        }
    }


    private void TryConnectWith(Collider2D other)
    {
        if (other.TryGetComponent<PipeConnector>(out var otherConnector))
        {
            if (otherConnector.direction == parentPipe.GetOppositeDirection(direction))
            {
                parentPipe.AddNeighbor(otherConnector.parentPipe);
            }
        }
    }
}
