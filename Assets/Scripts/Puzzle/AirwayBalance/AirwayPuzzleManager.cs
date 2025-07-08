using UnityEngine;

public class AirwayPuzzleManager : MonoBehaviour
{
    [Header("Grid")]
    public Airway[,] airways;

    [HideInInspector] public int gridRows;
    [HideInInspector] public int gridColumns;

    private HiddenRoomRevealer hiddenRoomRevealer;

    private void Start()
    {
        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();

        AirwayPlacementSystem placementSystem = GetComponent<AirwayPlacementSystem>();
        placementSystem.PlaceAllAirwaySegments(this);
    }

    public void ValidatePuzzle()
    {
        if (!CheckAllFilled())
        {
            Debug.Log("Validation failed: Not all cells are filled.");
            return;
        }

        if (!CheckAdjacencyRule())
        {
            Debug.Log("Validation failed: Adjacency rule failed.");
            return;
        }

        if (!CheckBalanceRule())
        {
            Debug.Log("Validation failed: Balance rule failed.");
            return;
        }

        if (!CheckUniquenessRule())
        {
            Debug.Log("Validation failed: Uniqueness rule failed.");
            return;
        }

        Debug.Log("Airway puzzle solved!");
        hiddenRoomRevealer.DonePuzzle(transform);
    }

    private bool CheckAllFilled()
    {
        foreach (Airway airway in airways)
        {
            if (airway == null) continue;
            if (airway.currentState == Airway.State.Empty)
                return false;
        }
        return true;
    }

    private bool CheckAdjacencyRule()
    {
        for (int row = 0; row < gridRows; row++)
        {
            for (int col = 0; col < gridColumns - 2; col++)
            {
                Airway a = airways[row, col];
                Airway b = airways[row, col + 1];
                Airway c = airways[row, col + 2];

                if (a == null || b == null || c == null) continue;
                if (a.currentState == Airway.State.Empty || b.currentState == Airway.State.Empty || c.currentState == Airway.State.Empty) continue;

                if (a.currentState == b.currentState && b.currentState == c.currentState)
                    return false;
            }
        }

        for (int col = 0; col < gridColumns; col++)
        {
            for (int row = 0; row < gridRows - 2; row++)
            {
                Airway a = airways[row, col];
                Airway b = airways[row + 1, col];
                Airway c = airways[row + 2, col];

                if (a == null || b == null || c == null) continue;
                if (a.currentState == Airway.State.Empty || b.currentState == Airway.State.Empty || c.currentState == Airway.State.Empty) continue;

                if (a.currentState == b.currentState && b.currentState == c.currentState)
                    return false;
            }
        }

        return true;
    }

    private bool CheckBalanceRule()
    {
        for (int row = 0; row < gridRows; row++)
        {
            int openCount = 0, closedCount = 0;

            for (int col = 0; col < gridColumns; col++)
            {
                Airway airway = airways[row, col];
                if (airway == null) continue;
                if (airway.currentState == Airway.State.Open) openCount++;
                if (airway.currentState == Airway.State.Closed) closedCount++;
            }

            if (openCount != closedCount)
                return false;
        }

        for (int col = 0; col < gridColumns; col++)
        {
            int openCount = 0, closedCount = 0;

            for (int row = 0; row < gridRows; row++)
            {
                Airway airway = airways[row, col];
                if (airway == null) continue;
                if (airway.currentState == Airway.State.Open) openCount++;
                if (airway.currentState == Airway.State.Closed) closedCount++;
            }

            if (openCount != closedCount)
                return false;
        }

        return true;
    }

    private bool CheckUniquenessRule()
    {
        for (int i = 0; i < gridRows; i++)
        {
            for (int j = i + 1; j < gridRows; j++)
            {
                if (AreRowsEqual(i, j))
                    return false;
            }
        }

        for (int i = 0; i < gridColumns; i++)
        {
            for (int j = i + 1; j < gridColumns; j++)
            {
                if (AreColumnsEqual(i, j))
                    return false;
            }
        }

        return true;
    }

    private bool AreRowsEqual(int rowA, int rowB)
    {
        for (int col = 0; col < gridColumns; col++)
        {
            Airway a = airways[rowA, col];
            Airway b = airways[rowB, col];

            if (a == null || b == null) return false;
            if (a.currentState == Airway.State.Empty || b.currentState == Airway.State.Empty) return false;
            if (a.currentState != b.currentState) return false;
        }
        return true;
    }

    private bool AreColumnsEqual(int colA, int colB)
    {
        for (int row = 0; row < gridRows; row++)
        {
            Airway a = airways[row, colA];
            Airway b = airways[row, colB];

            if (a == null || b == null) return false;
            if (a.currentState == Airway.State.Empty || b.currentState == Airway.State.Empty) return false;
            if (a.currentState != b.currentState) return false;
        }
        return true;
    }
}
