using UnityEngine;

public class Smoke : MonoBehaviour
{
    [HideInInspector]
    public SmokePuzzleManager manager;

    [Header("State")]
    public bool isCleared = false;

    private GameObject smokeVisual;
    private SpriteRenderer circleRenderer;

    [Header("Colors")]
    public Color unclearedColor = Color.gray;
    public Color clearedColor = Color.green;

    public int gridRow;
    public int gridCol;

    private void Start()
    {
        smokeVisual = transform.Find("SmokeVisual").gameObject;
        Transform circleTransform = transform.Find("Circle");
        if (circleTransform != null)
        {
            circleRenderer = circleTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogWarning("Circle child not found in " + gameObject.name);
        }

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        Toggle();
        manager.ToggleAdjacentTiles(this);
    }

    public void Clear()
    {
        isCleared = true;
        UpdateVisual();
    }

    public void Unclear()
    {
        isCleared = false;
        UpdateVisual();
    }

    public void Toggle()
    {
        isCleared = !isCleared;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (smokeVisual != null)
        {
            smokeVisual.SetActive(!isCleared);
        }

        if (circleRenderer != null)
        {
            circleRenderer.color = isCleared ? clearedColor : unclearedColor;
        }
    }
}
