
using TMPro;
using UnityEngine;

class ScoreText : MonoBehaviour
{
    [Header("Score Text Settings")]
    [Tooltip("The text component that displays the score.")]
    public TMP_Text scoreText;

    private void Start()
    {
        UpdateScore(ScoreManager.Instance.CurrentScore);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
