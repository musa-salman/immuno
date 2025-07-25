using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedVolumeParam = "MasterVolume";

    private bool isPaused = false;
    private const float muteVolumeDb = -80f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (isPaused)
        {
            audioMixer.SetFloat(exposedVolumeParam, muteVolumeDb);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        audioMixer.SetFloat(exposedVolumeParam, muteVolumeDb);
        Debug.Log("[PauseManager] Game Paused + Audio Muted");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float volumeInDb = Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedVolumeParam, volumeInDb);

        Debug.Log("[PauseManager] Game Resumed + Audio Restored");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        ResumeGame();
    }
}
