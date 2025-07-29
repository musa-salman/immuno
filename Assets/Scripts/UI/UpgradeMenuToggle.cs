using UnityEngine;

public class UpgradeMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject upgradeMenuUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.U;

    private void Start()
    {
        upgradeMenuUI.SetActive(false);
    }

    public void ShowMenu()
    {
        upgradeMenuUI.SetActive(true);
    }

    public void HideMenu()
    {
        upgradeMenuUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (upgradeMenuUI != null)
            {
                bool isActive = upgradeMenuUI.activeSelf;

                upgradeMenuUI.SetActive(!isActive);

                if (!isActive)
                    ShowMenu();
                else
                    HideMenu();
            }
        }
    }
}
