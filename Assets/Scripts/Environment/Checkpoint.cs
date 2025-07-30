using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void Start()
    {
        if (CheckpointManager.Instance.IsCheckpointUsed(transform))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.Instance.SetCheckpoint(transform);
        }
    }
}
