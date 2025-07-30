using UnityEngine;
using TMPro;
using System.Collections;

public class Objective : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "Start Scene";

    private bool isCompleted = false;
    private TextMeshProUGUI countdownText;

    private void Awake()
    {
        countdownText = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);

        countdownText.text = "";
    }

    public void CompleteObjective()
    {
        if (isCompleted)
            return;

        isCompleted = true;
        StartCoroutine(DelayedSceneLoad());
    }

    private IEnumerator DelayedSceneLoad()
    {
        int seconds = 3;

        while (seconds > 0)
        {
            if (countdownText != null)
                countdownText.text = seconds.ToString();

            yield return new WaitForSeconds(1f);
            seconds--;
        }

        if (countdownText != null)
            countdownText.text = "";

        SceneController.Instance.LoadScene(targetSceneName);
    }
}
