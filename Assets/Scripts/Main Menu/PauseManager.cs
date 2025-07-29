using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedVolumeParam = "MasterVolume";

    private GameObject pauseMenu;
    private bool isPaused = false;
    private const float muteVolumeDb = -80f;

    private float currentVolume = 0.75f;
    private float currentBrightness = 0.5f;

    public float CurrentVolume => currentVolume;
    public float CurrentBrightness => currentBrightness;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved settings
            currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            currentBrightness = PlayerPrefs.GetFloat("Brightness", 0.5f);

            ApplySavedVolume(); // Apply volume at startup
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (pauseMenu == null)
        {
            pauseMenu = FindInactiveInSceneByTag("Pause_Menu");
            return;
        }

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

    private GameObject FindInactiveInSceneByTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) &&
                !obj.activeInHierarchy &&
                obj.hideFlags == HideFlags.None &&
                obj.scene.IsValid() &&
                obj.scene.isLoaded)
            {
                return obj;
            }
        }

        return null;
    }

    public void RegisterPauseMenu(GameObject menu)
    {
        if (menu == null)
        {
            Debug.LogError("Pause menu GameObject is null.");
            return;
        }

        pauseMenu = menu;
        pauseMenu.SetActive(false);
        Debug.Log("[PauseManager] Registered Pause Menu");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        audioMixer.SetFloat(exposedVolumeParam, muteVolumeDb);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        ApplySavedVolume();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        ResumeGame();
    }

    public void SetVolume(float value)
    {
        currentVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        float volumeInDb = Mathf.Log10(Mathf.Clamp(1f - value, 0.0001f, 1f)) * 20f; // inverted
        audioMixer.SetFloat(exposedVolumeParam, volumeInDb);
    }

    public void SetBrightness(float value)
    {
        currentBrightness = value;
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }

    private void ApplySavedVolume()
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(1f - currentVolume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedVolumeParam, volumeInDb);
    }
}
