using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Management")]
    [Tooltip("The total number of enemies currently in the scene.")]
    [SerializeField] private int enemyCount;

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
