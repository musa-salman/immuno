using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillUIRefresher : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SkillManager.Instance != null)
            SkillManager.Instance.GenerateUi();

        if (CollectionsManager.Instance != null)
            CollectionsManager.Instance.RefreshUi();
    }
}
