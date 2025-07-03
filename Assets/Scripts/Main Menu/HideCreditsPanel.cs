using UnityEngine;

public class HideCreditsPanel : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    public void OnCloseButtonClick()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}
