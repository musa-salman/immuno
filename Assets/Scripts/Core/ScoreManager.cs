using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    [Tooltip("Current score of the player. You can edit this in the Inspector.")]
    public int currentScore;

    [SerializeField]
    private int pointsPerPuzzle = 500;

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
        if (currentScore < 0)
        {
            currentScore = 0;
        }
        FindFirstObjectByType<ScoreText>().UpdateScore(currentScore);
    }

    public void PayForPuzzle()
    {
        if (CanPayForPuzzle())
        {
            currentScore -= pointsPerPuzzle;
            pointsPerPuzzle *= 2;
            FindFirstObjectByType<ScoreText>().UpdateScore(currentScore);
        }
        else
        {
            Debug.LogWarning("Not enough points to pay for the puzzle.");
        }
    }

    public bool CanPayForPuzzle() => currentScore >= pointsPerPuzzle;

    public int GetPointsPerPuzzle => pointsPerPuzzle;


    public void ResetScore()
    {
        currentScore = 0;
        FindFirstObjectByType<ScoreText>().UpdateScore(currentScore);
    }
}
