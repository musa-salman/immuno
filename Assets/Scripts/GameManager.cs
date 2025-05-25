using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int totalDeaths;
    public string lastOrgan;


    private void Awake()
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

    public void RegisterDeath()
    {
        totalDeaths++;
    }
}

