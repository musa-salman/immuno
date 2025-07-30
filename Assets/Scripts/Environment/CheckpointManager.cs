using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector2 lastCheckpoint;

    private readonly List<Vector2> usedCheckpoints = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Transform newCheckpoint)
    {
        Vector2 newCheckpointPosition = Hash(newCheckpoint);
        if ((lastCheckpoint != null && lastCheckpoint == newCheckpointPosition)
            || usedCheckpoints.Contains(newCheckpointPosition))
        {
            return;
        }

        usedCheckpoints.Add(lastCheckpoint);

        lastCheckpoint = newCheckpointPosition;
    }

    public void RespawnPlayer()
    {
        FindObjectOfType<PlayerMovement>().transform.position = new Vector3(lastCheckpoint.x, lastCheckpoint.y, 2);
    }

    public bool IsCheckpointUsed(Transform checkpoint) => usedCheckpoints.Contains(Hash(checkpoint));

    private Vector2 Hash(Transform checkpoint)
    {
        return new Vector2(checkpoint.position.x, checkpoint.position.y);
    }
}
