
using TMPro;
using UnityEngine;

class ScoreText : MonoBehaviour
{
    private TMP_Text scoreText;

    private void Start()
    {
        scoreText = GetComponent<TMP_Text>();
        UpdateScore(ScoreManager.Instance.CurrentScore);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
