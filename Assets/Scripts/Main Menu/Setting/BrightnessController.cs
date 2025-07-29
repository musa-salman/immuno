using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BrightnessController : MonoBehaviour
{
    public static BrightnessController Instance;

    [SerializeField] private Slider brightnessSlider;
    private Image brightnessOverlay;
    private readonly List<Image> backgroundImages = new();

    private void Start()
    {
        brightnessSlider.value = PauseManager.Instance.CurrentBrightness;
        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);

        ScanScene();
        ApplyBrightness(brightnessSlider.value);
    }

    private void ScanScene()
    {
        GameObject overlayObj = GameObject.FindGameObjectWithTag("BrightnessOverlay");
        brightnessOverlay = overlayObj != null ? overlayObj.GetComponent<Image>() : null;

        backgroundImages.Clear();
        Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
        foreach (Image img in allImages)
        {
            if (img != null && img.CompareTag("BrightnessOverlay"))
            {
                backgroundImages.Add(img);
            }
        }
    }

    public void ApplyBrightness(float value)
    {
        PauseManager.Instance.SetBrightness(value);

        if (brightnessOverlay != null)
        {
            Color c = brightnessOverlay.color;
            c.a = 1f - value;
            brightnessOverlay.color = c;
        }

        foreach (var img in backgroundImages)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = Mathf.Lerp(0.2f, 1f, value);
                img.color = c;
            }
        }

        PauseManager.Instance.SetBrightness(value);
    }
}
