using UnityEngine;
using UnityEngine.UI;

public class QuitMenuController : MonoBehaviour
{
    private Button quitButton;

    private void Start()
    {
        quitButton = GetComponent<Button>();
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClick);
        }
        else
        {
            Debug.LogError("Quit button not found on QuitMenuController.");
        }
    }

    public void OnQuitClick()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}