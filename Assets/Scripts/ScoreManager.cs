using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    [Tooltip("Current score of the player. You can edit this in the Inspector.")]
    public int currentScore;

    public int CurrentScore => currentScore;

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

    public void AddPoints(int points)
    {
        currentScore += points;
        HUD.Instance.UpdateScore(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        HUD.Instance.UpdateScore(currentScore);
    }
}
