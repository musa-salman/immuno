using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class SafePositionTracker : MonoBehaviour
{
    [Header("Tilemap Stuck Detection")]
    public Tilemap solidTilemap;
    public float stuckThresholdPercent = 0.6f;
    public int checkResolution = 5;

    private Collider2D playerCollider;
    private Vector2 lastSafePosition;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
        lastSafePosition = transform.position;
    }

    private void Update()
    {
        if (IsStuckInTilemap())
        {
            Debug.Log("Player stuck. Instantly teleporting to last safe position.");
            transform.position = lastSafePosition;
        }
        else
        {
            lastSafePosition = transform.position;
        }
    }

    private bool IsStuckInTilemap()
    {
        Bounds bounds = playerCollider.bounds;

        int xCount = checkResolution;
        int yCount = checkResolution;

        int solidHits = 0;
        int totalChecks = xCount * yCount;

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                float px = Mathf.Lerp(bounds.min.x, bounds.max.x, (float)x / (xCount - 1));
                float py = Mathf.Lerp(bounds.min.y, bounds.max.y, (float)y / (yCount - 1));

                Vector3Int tilePos = solidTilemap.WorldToCell(new Vector3(px, py, 0));
                if (solidTilemap.HasTile(tilePos))
                {
                    solidHits++;
                }
            }
        }

        float stuckPercent = (float)solidHits / totalChecks;
        return stuckPercent >= stuckThresholdPercent;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(playerCollider.bounds.center, playerCollider.bounds.size);
        }
    }
}
