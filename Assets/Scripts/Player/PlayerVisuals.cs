using UnityEngine;
using System.Collections;

public class PlayerVisuals : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalColor = sr.color;
        }
    }


    public void StartGlow(Color glowColor, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(GlowCoroutine(glowColor, duration));
    }

    private IEnumerator GlowCoroutine(Color glowColor, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (sr != null)
            {
                float pulse = 0.5f + Mathf.Sin(Time.time * 10f) * 0.5f;
                sr.color = Color.Lerp(originalColor, glowColor, pulse);
            }

            timer += Time.deltaTime;
            yield return null;
        }


        if (sr != null)
            sr.color = originalColor;
    }
}
