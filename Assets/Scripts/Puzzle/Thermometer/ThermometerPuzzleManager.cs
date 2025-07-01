using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ThermometerPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Clues")]
    public int[] rowClues;
    public int[] columnClues;

    [Header("Grid Settings")]
    public int gridRows;
    public int gridColumns;

    [Header("Tilemap Reference")]
    public Tilemap puzzleTilemap;

    private List<Thermometer> thermometers;
    private HiddenRoomRevealer hiddenRoomRevealer;
    private bool isValidating = false;

    private void Start()
    {
        thermometers = GetComponent<ThermometerPlacementSystem>().thermometers;
        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (!isValidating)
            {
                isValidating = true;
                ValidateAndReveal();
            }
        }
    }

    private void ValidateAndReveal()
    {
        bool solved = ValidateSolution();

        if (solved)
        {
            Debug.Log("Thermometer Puzzle Solved!");
            hiddenRoomRevealer.DonePuzzle(transform);
        }
        else
        {
            Debug.Log("Thermometer Puzzle not solved yet.");
        }

        isValidating = false;
    }

    public bool ValidateSolution()
    {
        bool valid = true;

        // Validate rows
        for (int r = 0; r < gridRows; r++)
        {
            int filled = CountFilledInRow(r);
            if (filled != rowClues[r])
            {
                Debug.Log($"Row {r} invalid: Expected {rowClues[r]}, found {filled}");
                valid = false;
            }
        }

        // Validate columns
        for (int c = 0; c < gridColumns; c++)
        {
            int filled = CountFilledInColumn(c);
            if (filled != columnClues[c])
            {
                Debug.Log($"Column {c} invalid: Expected {columnClues[c]}, found {filled}");
                valid = false;
            }
        }

        return valid;
    }

    private int CountFilledInRow(int row)
    {
        int count = 0;
        foreach (var thermo in thermometers)
        {
            foreach (var seg in thermo.segments)
            {
                if (seg.gridRow == row && seg.isFilled)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private int CountFilledInColumn(int col)
    {
        int count = 0;
        foreach (var thermo in thermometers)
        {
            foreach (var seg in thermo.segments)
            {
                if (seg.gridCol == col && seg.isFilled)
                {
                    count++;
                }
            }
        }
        return count;
    }

}
