using UnityEngine;
using UnityEngine.UI;

public class ResumeButtonController : MonoBehaviour
{
    private Button resumeButton;

    private void Start()
    {
        resumeButton = GetComponent<Button>();
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClick);
        }
        else
        {
            Debug.LogError("Resume button not found on ResumeButtonController.");
        }
    }

    public void OnResumeClick()
    {
        if (resumeButton != null)
            resumeButton.interactable = false;

        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.ResumeGame();
        }
        else
        {
            Debug.LogWarning("PauseManager not found in scene.");
        }

        resumeButton.interactable = true; // re-enable after action if needed
    }
}
