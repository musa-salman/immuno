using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Management")]
    [Tooltip("The total number of enemies currently in the scene.")]
    [SerializeField] private int enemyCount;

    [Header("Global Enemy Control")]
    [SerializeField] private bool isActive = true;

    public bool IsActive => isActive;

    public void SetEnemiesActive(bool state)
    {
        isActive = state;
        Debug.Log($"[EnemyManager] Enemies {(state ? "resumed" : "paused")}");
    }

    public void RegisterEnemy() => enemyCount++;

    public void EnemyKilled(int scoreValue = 0)
    {
        enemyCount--;

        ScoreManager.Instance.AddPoints(scoreValue);
        if (enemyCount <= 0)
            Debug.Log("All enemies defeated");
    }

    public bool AllEnemiesDefeated() => enemyCount <= 0;
}
