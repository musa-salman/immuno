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

    public void ValidateSolution()
    {

        // Validate rows
        for (int r = 0; r < gridRows; r++)
        {
            int filled = CountFilledInRow(r);
            if (filled != rowClues[r])
            {
                Debug.Log($"Row {r} invalid: Expected {rowClues[r]}, found {filled}");
                return;
            }
        }

        // Validate columns
        for (int c = 0; c < gridColumns; c++)
        {
            int filled = CountFilledInColumn(c);
            if (filled != columnClues[c])
            {
                Debug.Log($"Column {c} invalid: Expected {columnClues[c]}, found {filled}");
                return;
            }
        }

        foreach (var thermo in thermometers)
        {
            bool anyFilled = false;
            bool foundUnfilledAfterFilled = false;

            foreach (var seg in thermo.segments)
            {
                if (seg.isFilled)
                {
                    anyFilled = true;
                    if (foundUnfilledAfterFilled)
                    {
                        Debug.Log($"Thermometer continuity invalid in thermometer starting at ({thermo.segments[0].gridRow}, {thermo.segments[0].gridCol})");
                        return;
                    }
                }
                else
                {
                    if (anyFilled)
                    {
                        foundUnfilledAfterFilled = true;
                    }
                }
            }
        }


        hiddenRoomRevealer.DonePuzzle(transform);
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
