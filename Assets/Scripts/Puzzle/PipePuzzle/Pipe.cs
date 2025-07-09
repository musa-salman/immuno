using UnityEngine;

public class Pipe : MonoBehaviour
{
    public enum Direction { Up, Right, Down, Left }
    private GameObject acidFillObject;

    [Header("Pipe configuration")]
    public Direction[] baseConnections;

    public Direction[] currentConnections;

    [Header("Pipe state")]
    public bool isFilled = false;
    public bool isStartPoint = false;

    [Header("Grid position")]
    public int row;
    public int col;

    private int rotationSteps = 0;
    public PipeSystem pipeSystem;

    private void Start()
    {
        currentConnections = new Direction[baseConnections.Length];
        baseConnections.CopyTo(currentConnections, 0);

        int randomRotations = Random.Range(0, 4);
        SetRotationSteps(randomRotations);

        if (isStartPoint)
        {
            isFilled = true;
        }

        acidFillObject = transform.Find("AcidFill").gameObject;
        UpdateSprite();
    }

    private void OnMouseDown()
    {
        RotatePipe();
        pipeSystem.PropagateAcid();
    }

    public void RotatePipe()
    {
        SetRotationSteps(rotationSteps - 1);
    }


    private void SetRotationSteps(int steps)
    {
        rotationSteps = steps % 4;
        transform.rotation = Quaternion.Euler(0, 0, rotationSteps * 90f);
        UpdateConnections();
    }

    private static Direction RotateDirection(Direction dir, int steps)
    {
        int dirInt = ((int)dir - steps + 4) % 4;
        return (Direction)dirInt;
    }

    private void UpdateConnections()
    {
        for (int i = 0; i < baseConnections.Length; i++)
        {
            currentConnections[i] = RotateDirection(baseConnections[i], rotationSteps);
        }
    }



    public bool HasConnection(Direction dir)
    {
        foreach (Direction d in currentConnections)
        {
            if (d == dir) return true;
        }
        return false;
    }

    public void SetFilled(bool filled)
    {
        isFilled = filled;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        acidFillObject.SetActive(isFilled);
    }

    public Direction GetOppositeDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            _ => dir,
        };
    }
}
