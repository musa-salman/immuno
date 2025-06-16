using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorDetector : MonoBehaviour
{
    public Tilemap doorTilemap;
    public Transform checkPoint;
    public string targetSceneName = "lungs";
    public float stayTimeRequired = 0.5f;
    private float stayTimer;
    [SerializeField] private EnemyManager enemyManager;

    void Update()
    {
        Vector3Int cellPosition = doorTilemap.WorldToCell(checkPoint.position);
        TileBase tile = doorTilemap.GetTile(cellPosition);

        if (tile != null && enemyManager.AllEnemiesDefeated())
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= stayTimeRequired)
            {
                SceneController.Instance.LoadScene(targetSceneName);
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }
}
