using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    private Button startButton;

    private void Start()
    {
        startButton = GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClick);
        }
        else
        {
            Debug.LogError("Start button not found on StartMenuController.");
        }
    }

    public void OnStartClick()
    {
        if (startButton != null)
            startButton.interactable = false;

        SceneController.Instance.LoadScene("SampleScene");
    }
}