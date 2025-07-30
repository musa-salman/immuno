using System;
using UnityEngine;

public class HiddenRoomPuzzleCollider : MonoBehaviour
{
    private HiddenRoomRevealer hiddenRoomRevealer;

    private TextPrompt textPrompt;

    private bool playerInRange = false;

    private string id;

    private void Start()
    {
        id = GetComponent<UniqueID>().ID;
        if (GameSaveManager.Instance.SolvedPuzzles.Contains(id))
        {
            Destroy(gameObject);
            return;
        }

        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();
        textPrompt = GetComponentInChildren<TextPrompt>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            textPrompt.ShowPrompt();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            textPrompt.HidePrompt();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {
            hiddenRoomRevealer.StartPuzzle(transform);
            FindObjectOfType<EnemyManager>().SetEnemiesActive(false);
        }
    }
}
