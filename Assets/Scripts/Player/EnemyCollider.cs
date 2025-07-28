

using System.Collections;
using UnityEngine;

public class EnemyCollider : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    public void TakeDamage(float damage)
    {
        playerHealth.TakeDamage(damage);
    }
}