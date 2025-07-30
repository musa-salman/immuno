using System;
using UnityEngine;

public class HiddenRoomPuzzleCollider : MonoBehaviour
{
    private HiddenRoomRevealer hiddenRoomRevealer;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Player pressed P on puzzle collider. Notifying HiddenRoomRevealer.");

            if (hiddenRoomRevealer != null)
            {
                hiddenRoomRevealer.StartPuzzle(transform);
                FindObjectOfType<EnemyManager>()?.SetEnemiesActive(false);
            }
        }
    }
}
