using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour
{
    private GameObject pauseMenu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedVolumeParam = "MasterVolume";

    private bool isPaused = false;
    private const float muteVolumeDb = -80f;

    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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


    void Update()
    {
        if (pauseMenu == null)
        {
            Debug.Log("Pause menu is not registered. Please ensure it is set in the PauseManager.");
            pauseMenu = FindInactiveInSceneByTag("Pause_Menu");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (isPaused)
                ResumeGame();
            else
            {
                PauseGame();

                // update the values of brightness and volume
                float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
                AudioVolumeController.Instance.ApplyVolume(savedVolume);

                float brightnessValue = PlayerPrefs.GetFloat("Brightness", 0.5f);
                BrightnessController.Instance.ApplyBrightness(brightnessValue);

                Debug.Log($"[PauseManager] Pause Menu Opened: Brightness {brightnessValue}, Volume {savedVolume}");
            }
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

        pauseMenu.SetActive(true);

        audioMixer.SetFloat(exposedVolumeParam, muteVolumeDb);
        Debug.Log("[PauseManager] Game Paused + Audio Muted");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        pauseMenu.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float volumeInDb = Mathf.Log10(Mathf.Clamp(savedVolume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedVolumeParam, volumeInDb);

        Debug.Log("[PauseManager] Game Resumed + Audio Restored");
    }

    public void RegisterPauseMenu(GameObject menu)
    {
        if (menu == null)
        {
            Debug.LogError("Pause menu GameObject is null. Please ensure it is assigned correctly.");
            return;
        }
        if (pauseMenu != null)
        {
            Debug.LogWarning("Pause menu is already registered. Overwriting the existing reference.");
        }
        Debug.Log("[PauseManager] Registering Pause Menu: " + menu.name);
        pauseMenu = menu;
        pauseMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        ResumeGame();
    }
}
