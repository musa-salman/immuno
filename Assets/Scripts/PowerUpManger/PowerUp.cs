using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private CollectionsManager collectionsManager;

    [SerializeField] private PowerUpType powerUpType;

    private string id;

    void Start()
    {
        if (CollectionsManager.Instance == null)
        {
            Debug.LogError("CollectionsManager instance is not set.");
            return;
        }

        id = GetComponent<UniqueID>().ID;
        if (GameSaveManager.Instance.CollectedPowerUps.Contains(id))
        {
            Destroy(gameObject);
        }

        collectionsManager = CollectionsManager.Instance;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameSaveManager.Instance.CollectedPowerUps.Add(id);
            collectionsManager.CollectPowerUp(powerUpType);
            Destroy(gameObject);
        }
    }

}
