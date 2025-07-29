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

    [SerializeField] private Material disabledImageMaterial;

    private readonly string enabledTextColor = "#FFB800";

    public void EnableUi()
    {
        powerUpImage.material = null;
        powerKeyImage.material = null;
        powerUpText.color = ColorUtility.TryParseHtmlString(enabledTextColor, out Color color) ? color : Color.white;
        counterText.color = ColorUtility.TryParseHtmlString(enabledTextColor, out Color color2) ? color2 : Color.white;
    }
    public void SetCounterText(int count)
    {
        counterText.text = "x" + count.ToString();
    }

    public void DisableUi()
    {
        powerUpImage.material = disabledImageMaterial;
        powerKeyImage.material = disabledImageMaterial;
        powerUpText.color = Color.white;
        counterText.color = Color.white;
    }
}
