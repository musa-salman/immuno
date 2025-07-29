using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioVolumeController : MonoBehaviour
{
    public static AudioVolumeController Instance;

    [Header("References")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private AudioMixer audioMixer;
    private const string VolumeKey = "MasterVolume";

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.75f);
        ApplyVolume(savedVolume);

        if (masterSlider != null)
        {
            masterSlider.value = savedVolume;
            masterSlider.onValueChanged.AddListener(sliderVal =>
            {
                float actualVolume = sliderVal;
                ApplyVolume(actualVolume);
            });
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.75f);
        ApplyVolume(savedVolume);
    }

    public void ApplyVolume(float value)
    {

        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MasterVolume", volumeInDb);

        PauseManager.Instance.SetVolume(value);
    }

}
