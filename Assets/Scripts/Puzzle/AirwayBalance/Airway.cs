using UnityEngine;

public class Airway : MonoBehaviour
{
    [HideInInspector] public AirwayPuzzleManager manager;

    public enum State { Empty, Open, Closed }
    public State currentState = State.Empty;

    public bool isTarget = false;

    private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite openSprite;
    public Sprite closedSprite;

    [Header("Grid Position")]
    public int gridRow;
    public int gridCol;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (isTarget)
            spriteRenderer.color = Color.yellow;
        else
            spriteRenderer.color = Color.white;

        UpdateVisual();
    }

    private void OnMouseDown()
    {
        if (!isTarget)
        {
            Toggle();
            manager.ValidatePuzzle();
        }
    }

    public void SetState(State state)
    {
        currentState = state;
        UpdateVisual();
    }

    public void Toggle()
    {
        if (currentState == State.Empty)
            currentState = State.Open;
        else if (currentState == State.Open)
            currentState = State.Closed;
        else if (currentState == State.Closed)
            currentState = State.Empty;

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (isTarget)
        {
            return;
        }

        switch (currentState)
        {
            case State.Open:
                spriteRenderer.sprite = openSprite;
                spriteRenderer.color = Color.white;
                break;
            case State.Closed:
                spriteRenderer.sprite = closedSprite;
                spriteRenderer.color = Color.white;
                break;
            default:
                spriteRenderer.sprite = closedSprite;
                spriteRenderer.color = new Color(1, 1, 1, 0.3f);
                break;
        }
    }
}
