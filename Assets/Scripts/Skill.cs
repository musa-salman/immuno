using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Skill : MonoBehaviour
{
    public Image fillBar;
    public TMP_Text xpText;
    public Button upgradeButton;       // reference to the upgrade button
    private Image buttonImage;         // reference to the button background

    [Header("Colors")]
    public Color canUpgradeColor = new Color32(255, 184, 0, 255);   // yellow
    public Color cannotUpgradeColor = new Color32(100, 40, 40, 255); // muted red
    public Color maxedColor = new Color32(120, 120, 120, 255);      // gray

    void Awake()
    {
        buttonImage = upgradeButton.GetComponent<Image>();
    }

    public void SetLevel(int currentLevel, int maxLevel, int requiredXP, int currentScore)
    {
        float percent = (float)currentLevel / maxLevel;
        fillBar.fillAmount = percent;

        bool isMaxed = currentLevel >= maxLevel;
        bool canAfford = currentScore >= requiredXP;

        if (isMaxed)
        {
            fillBar.fillAmount = 1f;
            xpText.text = "Max Level Reached";
            upgradeButton.interactable = false;
            SetButtonColor(maxedColor);
        }
        else
        {
            xpText.text = $"Next Level: {requiredXP} XP";
            upgradeButton.interactable = canAfford;
            SetButtonColor(canAfford ? canUpgradeColor : cannotUpgradeColor);
        }
    }

    private void SetButtonColor(Color c)
    {
        if (buttonImage != null)
            buttonImage.color = c;

        if (xpText != null)
            xpText.color = c;
    }
}
