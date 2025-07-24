using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioVolumeController : MonoBehaviour
{
    public static AudioVolumeController Instance;

    [Header("References")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private AudioMixer audioMixer;

    private float currentVolume = 0.75f;

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
            return;
        }
    }

    private void Start()
    {
        currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);

        ApplyVolume(currentVolume);

        if (masterSlider != null)
        {
            masterSlider.value = currentVolume;
            masterSlider.onValueChanged.AddListener(ApplyVolume);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVolume(currentVolume);
    }

    public void ApplyVolume(float value)
    {
        currentVolume = value;

        if (value <= 0.0001f)
        {
            audioMixer.SetFloat("MasterVolume", -80f); // Fully muted
        }
        else
        {
            float volumeInDb = Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("MasterVolume", volumeInDb);
        }

        PlayerPrefs.SetFloat("MasterVolume", value);
        Debug.Log($"[AudioVolumeController] Volume: {value} → saved & applied");
    }
}
