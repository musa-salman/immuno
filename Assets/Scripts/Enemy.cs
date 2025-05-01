using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;
    [Header("Collider Parameters")]
    [SerializeField] private BoxCollider2D boxCollider;
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    [Header("Range Attack")]
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private GameObject[] bullets;


    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                RangedAttack();
            }
        }
    }

private bool PlayerInSight()
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    if (player != null)
    {
        float distanceToPlayerX = player.transform.position.x - transform.position.x;
        float distanceToPlayerY = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (Mathf.Abs(distanceToPlayerX) <= rangeX &&
            Mathf.Sign(distanceToPlayerX) == Mathf.Sign(transform.localScale.x) &&
            distanceToPlayerY <= rangeY)
        {
            return true;
        }
    }

    return false;
}

private void OnDrawGizmos()
{
    Gizmos.color = Color.red;
    Vector3 center = transform.position + Vector3.right * rangeX / 2 * (transform.localScale.x / Mathf.Abs(transform.localScale.x));
    Vector3 size = new Vector3(rangeX, rangeY * 2, 0);
    Gizmos.DrawWireCube(center, size);
}

    private void RangedAttack()
    {
        cooldownTimer = 0;
        bullets[FindBullet()].transform.position = bulletPoint.position;
        bullets[FindBullet()].GetComponent<EnemyProjectile>().ActivateProjectile(Mathf.Sign(transform.localScale.x));
    }

    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
}
