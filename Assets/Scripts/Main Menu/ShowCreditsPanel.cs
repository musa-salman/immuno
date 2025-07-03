using UnityEngine;

public class ShowCreditsPanel : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;

    public void OnCreditsButtonClick()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }
}
