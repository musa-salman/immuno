using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float ydistanceToPlayer = 5f;
    private float cooldownTimer = Mathf.Infinity;

    private Health playerHealth;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                DamagePlayer();
            }
        }
        Debug.Log(PlayerInSight());
    }

private bool PlayerInSight()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null)
    {
        float distanceToPlayerX = player.transform.position.x - transform.position.x;
        float distanceToPlayerY = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (Mathf.Abs(distanceToPlayerX) <= range &&
            Mathf.Sign(distanceToPlayerX) == Mathf.Sign(transform.localScale.x) &&
            distanceToPlayerY <= ydistanceToPlayer)
        {
            playerHealth = player.GetComponent<Health>();
            return true;
        }
    }

    return false;
}

private void OnDrawGizmos()
{
    Gizmos.color = Color.red;
    Vector3 center = transform.position + Vector3.right * range / 2 * (transform.localScale.x / Mathf.Abs(transform.localScale.x));
    Vector3 size = new Vector3(range, ydistanceToPlayer * 2, 0);
    Gizmos.DrawWireCube(center, size);
}

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
            cooldownTimer = 0;
        }
    }
}
