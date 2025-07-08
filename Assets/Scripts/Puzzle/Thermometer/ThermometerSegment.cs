using UnityEngine;

public class ThermometerSegment : MonoBehaviour
{
    [HideInInspector]
    public ThermometerPuzzleManager puzzleManager;

    public enum SegmentType { Head, Tail, MiddleHorizontal, MiddleVertical }
    public enum Direction { Up, Down, Left, Right }

    public SegmentType type;
    public Direction openingDirection;
    public bool isFilled = false;
    public Thermometer parentThermometer;
    private GameObject fillVisual;

    public int gridRow;
    public int gridCol;

    private void Start()
    {
        fillVisual = transform.Find("RedFill").gameObject;
        Unfill();

    }

    private void OnMouseDown()
    {
        if (isFilled)
        {
            Unfill();
        }
        else
        {
            Fill();
        }
        puzzleManager.ValidateSolution();
    }

    private void Fill()
    {
        isFilled = true;
        fillVisual.SetActive(true);
    }

    private void Unfill()
    {
        isFilled = false;
        fillVisual.SetActive(false);
    }
}
