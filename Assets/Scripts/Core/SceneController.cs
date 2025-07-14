using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneController : MonoBehaviour {
    [SerializeField] GameObject loadingScreen;
    public static SceneController Instance;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    public Action OnSceneTransitionStart;
    public Action OnSceneTransitionEnd;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, Action onComplete = null) {
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete = null) {
        OnSceneTransitionStart?.Invoke();
        yield return FadeOut(fadeDuration);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
        loadingScreen.SetActive(false);
        yield return FadeIn(fadeDuration);

        OnSceneTransitionEnd?.Invoke();
        onComplete?.Invoke();
    }

    private IEnumerator FadeOut(float duration) {
        float timer = 0f;
        while (timer < duration) {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = timer / duration;
            yield return null;
        }
    }

    private IEnumerator FadeIn(float duration) {
        float timer = 0f;
        while (timer < duration) {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = 1 - (timer / duration);
            yield return null;
        }
    }
}
