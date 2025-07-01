using UnityEngine;

public class SmokePuzzleManager : MonoBehaviour
{
    [Header("Tiles")]
    public Smoke[,] tiles;

    [HideInInspector]
    public Vector2Int gridOrigin;

    [HideInInspector]
    public int gridRows;
    [HideInInspector]
    public int gridColumns;

    private HiddenRoomRevealer hiddenRoomRevealer;

    private void Start()
    {
        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();

        SmokePlacementSystem placementSystem = GetComponent<SmokePlacementSystem>();
        placementSystem.PlaceAllSmokeTiles(this);
    }

    public void ToggleAdjacentTiles(Smoke center)
    {
        int col = center.gridCol;
        int row = center.gridRow;

        Debug.Log($"Toggling adjacent tiles for {center.gameObject.name} at grid ({col},{row})");

        TryToggle(col, row + 1); // Up
        TryToggle(col, row - 1); // Down
        TryToggle(col - 1, row); // Left
        TryToggle(col + 1, row); // Right

        CheckPuzzleSolved();
    }

    private void TryToggle(int col, int row)
    {
        if (col >= 0 && col < gridRows && row >= 0 && row < gridColumns)
        {
            Smoke tile = tiles[col, row];
            if (tile != null)
            {
                tile.Toggle();
            }
        }
    }

    private void CheckPuzzleSolved()
    {
        foreach (Smoke tile in tiles)
        {
            if (tile != null && !tile.isCleared)
                return;
        }

        Debug.Log($"Smoke puzzle solved! ({gameObject.name})");
        hiddenRoomRevealer.DonePuzzle(transform);
    }
}
