using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player_Health"))
            collision.GetComponent<EnemyCollider>().TakeDamage(damage);
    }
}
