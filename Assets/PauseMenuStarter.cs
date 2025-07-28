using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuStarter : MonoBehaviour
{
    void Start()
    {
        PauseManager pauseMenu = FindObjectOfType<PauseManager>();
        if (pauseMenu == null)
        {
            Debug.LogError("PauseManager not found in the scene. Please ensure it is added to the Managers prefab.");
            return;
        }
        pauseMenu.RegisterPauseMenu(gameObject);
    }
}
