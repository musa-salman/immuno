using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoomRevealer : MonoBehaviour
{
    public TilemapRenderer hiddenLayerRenderer;
    public float fadeDuration = 1f;

    private Material _material;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _material = hiddenLayerRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(0f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float elapsed = 0f;
        Color startColor = _material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _material.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        _material.color = targetColor;
    }
}
