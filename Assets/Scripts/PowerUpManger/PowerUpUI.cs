using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PowerUpUI : MonoBehaviour
{
    public PowerUpType Id = PowerUpType.None;
    [SerializeField] private Image powerUpImage;
    [SerializeField] private Image powerKeyImage;
    [SerializeField] private TMP_Text powerUpText;
    [SerializeField] private TMP_Text counterText;
    private Material imageMaterial;

    private string enabledTextColor = "#FFB800";

    public void EnableUI()
    {
        if (powerUpImage == null || powerKeyImage == null || powerUpText == null || counterText == null)
        {
            Debug.LogError("PowerUpUI: PowerUpImage or PowerUpText is not assigned.");
            return;
        }

        imageMaterial = powerUpImage.material;
        powerUpImage.material = null;
        powerKeyImage.material = null;
        powerUpText.color = ColorUtility.TryParseHtmlString(enabledTextColor, out Color color) ? color : Color.white;
        counterText.color = ColorUtility.TryParseHtmlString(enabledTextColor, out Color color2) ? color2 : Color.white;
    }
    public void setCounterText(int count)
    {
        if (counterText != null)
        {
            counterText.text = "x" + count.ToString();
        }
        else
        {
            Debug.LogError("PowerUpUI: CounterText is not assigned.");
        }
    }
    public void DisableUI()
    {
        if (imageMaterial != null)
        {
            powerUpImage.material = imageMaterial;
            powerKeyImage.material = imageMaterial;
            powerUpText.color = Color.white;
            counterText.color = Color.white;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
