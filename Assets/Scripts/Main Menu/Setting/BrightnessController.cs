using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BrightnessController : MonoBehaviour
{
    public static BrightnessController Instance;

    [SerializeField] private Slider brightnessSlider;
    private Image brightnessOverlay;
    private readonly List<Image> backgroundImages = new();
    private float currentBrightness;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);

        if (brightnessSlider != null)
        {
            brightnessSlider.value = currentBrightness;
            brightnessSlider.onValueChanged.AddListener(ApplyBrightness);
        }

        ScanScene();
        ApplyBrightness(currentBrightness);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScanScene();
        ApplyBrightness(currentBrightness);
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

    private void ApplyBrightness(float value)
    {
        currentBrightness = value;

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
