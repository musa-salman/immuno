using UnityEngine;
using UnityEngine.UI;

public class ReturnToMainMenuButton : MonoBehaviour
{
    private Button returnButton;

    private void Start()
    {
        returnButton = GetComponent<Button>();
        if (returnButton != null)
        {
            returnButton.onClick.AddListener(OnReturnClick);
        }
        else
        {
            Debug.LogError("Return button not found on ReturnToMainMenuButton.");
        }
    }

    public void OnReturnClick()
    {
        if (returnButton != null)
            returnButton.interactable = false;
        Time.timeScale = 1f;
        SceneController.Instance.LoadScene("Start Scene");
    }
}
