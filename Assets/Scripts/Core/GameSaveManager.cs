using System.Collections.Generic;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    public HashSet<string> CollectedPowerUps = new();
    public HashSet<string> SolvedPuzzles = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGameState()
    {
        CollectedPowerUps.Clear();
        SolvedPuzzles.Clear();
    }
}
