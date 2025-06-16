using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaDetector : MonoBehaviour
{
    public Health playerHealth;
    public Tilemap lavaTilemap;
    public Transform checkPoint;
    private AudioSource lavaAudioSource;

    [Header("Sound Settings")]
    public float maxVolume = 1f;
    public float minVolume = 0f;
    public float detectionRadius = 5f;

    private void Start()
    {
        lavaAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3Int cellPosition = lavaTilemap.WorldToCell(checkPoint.position);
        TileBase currentTile = lavaTilemap.GetTile(cellPosition);

        if (currentTile != null)
        {
            playerHealth.TakeDamage(0.5f * Time.deltaTime);
        }

        float closestDistance = detectionRadius + 1f;
        BoundsInt bounds = lavaTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = lavaTilemap.GetTile(pos);
            if (tile == null) continue;

            Vector3 tileWorldPos = lavaTilemap.GetCellCenterWorld(pos);
            float distance = Vector3.Distance(checkPoint.position, tileWorldPos);

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        float volume = Mathf.Lerp(maxVolume, minVolume, closestDistance / detectionRadius);
        lavaAudioSource.volume = Mathf.Clamp(volume, minVolume, maxVolume);
    }
}
