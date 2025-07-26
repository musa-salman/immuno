using UnityEngine;

public class UpgradeMenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject upgradeMenuUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.U;

    private bool hasGeneratedUI = false;

    private void Start()
    {
        upgradeMenuUI.SetActive(false);
    }

    public void show_menu()
    {
        upgradeMenuUI.SetActive(true);

        if (!hasGeneratedUI && SkillManager.Instance != null)
        {
            SkillManager.Instance.GenerateUi();
            hasGeneratedUI = true;
        }
    }

    public void hide_menu()
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
                    show_menu(); 
                else
                    hide_menu(); 
            }
        }
    }
}
