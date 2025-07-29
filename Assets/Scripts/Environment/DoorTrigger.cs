using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class DoorDetector : MonoBehaviour
{
    public Tilemap doorTilemap;
    public Transform checkPoint;
    public string targetSceneName = "lungs";
    public float stayTimeRequired = 0.5f;
    private float stayTimer;

    [SerializeField] private EnemyManager enemyManager;

    private bool sceneTriggered = false;

    void Update()
    {
        if (sceneTriggered) return;

        Vector3Int cellPosition = doorTilemap.WorldToCell(checkPoint.position);
        TileBase tile = doorTilemap.GetTile(cellPosition);

        bool onDoor = tile != null;
        bool allEnemiesDead = enemyManager.AllEnemiesDefeated();
        bool notAlreadyInScene = SceneManager.GetActiveScene().name != targetSceneName;

        if (onDoor && allEnemiesDead && notAlreadyInScene)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayTimeRequired)
            {
                sceneTriggered = true;
                Debug.Log($"✅ Loading scene '{targetSceneName}'...");
                SceneController.Instance.LoadScene(targetSceneName);
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }
}