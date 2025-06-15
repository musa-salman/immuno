using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD Instance;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }
}
