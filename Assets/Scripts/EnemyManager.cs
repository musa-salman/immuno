using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField]
    [Header("Enemy Management")]
    [Tooltip("The total number of enemies currently in the scene.")]
    private int enemyCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy() => enemyCount++;

    public void EnemyKilled(int scoreValue = 0)
    {
        enemyCount--;

        if (scoreValue > 0 && ScoreManager.Instance != null)
            ScoreManager.Instance.AddPoints(scoreValue);

        if (enemyCount <= 0)
            Debug.Log("All enemies defeated");
    }

    public bool AllEnemiesDefeated() => enemyCount <= 0;
}
