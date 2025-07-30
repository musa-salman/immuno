using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    public static SceneController Instance;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    public Action OnSceneTransitionStart;
    public Action OnSceneTransitionEnd;

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

    private void Start()
    {
        loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName, Action onComplete = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, onComplete));
    }

    public IEnumerator LoadSceneCoroutine(string sceneName, Action onComplete = null)
    {
        OnSceneTransitionStart?.Invoke();
        yield return FadeOut(fadeDuration);
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {

            yield return null;
        }
        onComplete?.Invoke();
        loadingScreen.SetActive(false);
        yield return FadeIn(fadeDuration);

        OnSceneTransitionEnd?.Invoke();
    }

    private IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = timer / duration;
            yield return null;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = 1 - (timer / duration);
            yield return null;
        }
    }
}
