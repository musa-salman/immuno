using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private CollectionsMannger collectionsMannger;
    // Start is called before the first frame update
    [SerializeField] private PowerUpType powerUpType;
    void Start()
    {
        if (CollectionsMannger.Instance == null)
        {
            Debug.LogError("CollectionsMannger instance is not set.");
            return;
        }
        collectionsMannger = CollectionsMannger.Instance;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PowerUp collected: " + powerUpType);
            collectionsMannger.CollectPowerUp(powerUpType);
            Destroy(gameObject);
        }
    }

}
