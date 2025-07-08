using UnityEngine;

public class SettingsUIManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;

    public void ShowSettingPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void HideSettingPanel()
    {
        settingsPanel.SetActive(false);
    }
}
