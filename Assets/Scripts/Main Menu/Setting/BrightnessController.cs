using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    private Image brightnessOverlay;
    private Image[] backgroundImages;

    private void Start()
    {
        GameObject overlayObj = GameObject.FindGameObjectWithTag("BrightnessOverlay");
        if (overlayObj != null)
            brightnessOverlay = overlayObj.GetComponent<Image>();

        Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
        var backgroundList = new System.Collections.Generic.List<Image>();

        foreach (Image img in allImages)
        {
            if (img != null && img.CompareTag("BrightnessOverlay"))
            {
                backgroundList.Add(img);
            }
        }

        backgroundImages = backgroundList.ToArray();

        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);
        brightnessSlider.value = savedBrightness;
        ApplyBrightness(savedBrightness);

        brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
    }

    private void ApplyBrightness(float value)
    {
        // Apply to main overlay
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

        PlayerPrefs.SetFloat("Brightness", value);
    }
}
