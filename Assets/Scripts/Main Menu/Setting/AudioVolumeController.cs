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


    private void Start()
    {
        ApplyVolume(PauseManager.Instance.CurrentVolume);

        if (masterSlider != null)
        {
            masterSlider.value = PauseManager.Instance.CurrentVolume;
            masterSlider.onValueChanged.AddListener(ApplyVolume);
        }
    }

    public void ApplyVolume(float value)
    {
        value = 1f - value;
        Debug.Log($"Setting volume to: {value}");
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("MasterVolume", volumeInDb);

        PauseManager.Instance.SetVolume(value);
    }
}
