using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaDetector : MonoBehaviour
{
    public Health playerHealth;

    public Tilemap lavaTilemap;
    public Transform checkPoint;
    

    void Update()
    {
        Vector3Int cellPosition = lavaTilemap.WorldToCell(checkPoint.position);
        TileBase tile = lavaTilemap.GetTile(cellPosition);

        if (tile != null)
        {
            playerHealth.TakeDamage(0.5f * Time.deltaTime);
      
        }
    }
}

