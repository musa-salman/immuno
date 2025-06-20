using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;

    [Header("Chase Parameters")]
    [SerializeField] private float chaseDuration = 2f;
    [SerializeField] private float chaseSpeed = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color patrolColor = Color.white;
    [SerializeField] private Color alertColor = Color.red;

    [Header("Collider Parameters")]
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Range Attack")]
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private GameObject[] bullets;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;

    [SerializeField] private LayerMask obstacleLayer;

    private Animator animator;

    private float cooldownTimer = Mathf.Infinity;
    private float chaseTimer = 0f;
    private bool hasSeenPlayer = false;
    private Transform playerTransform;
    private Vector3 initialScale;

    private bool isDead = false;
    public bool IsDead => isDead;


    private void Start()
    {
        FindObjectOfType<EnemyManager>().RegisterEnemy();
        initialScale = transform.localScale;
        if (spriteRenderer != null)
            spriteRenderer.color = patrolColor;

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            hasSeenPlayer = true;
            chaseTimer = chaseDuration;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            if (spriteRenderer != null)
                spriteRenderer.color = alertColor;

            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                RangedAttack();
            }
        }
        else if (hasSeenPlayer)
        {
            chaseTimer -= Time.deltaTime;

            if (chaseTimer > 0 && playerTransform != null)
            {
                ChasePlayer();
            }
            else
            {
                hasSeenPlayer = false;
                if (spriteRenderer != null)
                    spriteRenderer.color = patrolColor;
            }
        }
    }

    private bool PlayerInSight()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        float dx = player.transform.position.x - transform.position.x;
        float dy = Mathf.Abs(player.transform.position.y - transform.position.y);

        if (Mathf.Abs(dx) <= rangeX &&
            Mathf.Sign(dx) == Mathf.Sign(transform.localScale.x) &&
            dy <= rangeY)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, player.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleLayer);

            if (hit.collider == null)
            {
                return true;
            }
        }
        return false;
    }


    private void ChasePlayer()
    {
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * direction, initialScale.y, initialScale.z);
        transform.position += new Vector3(direction * Time.deltaTime * chaseSpeed, 0, 0);
    }

    private void RangedAttack()
    {
        SoundManager.instance.PlaySound(attackSound);

        int index = FindBullet();
        if (index >= bullets.Length || playerTransform == null)
            return;

        Vector2 playerPos = playerTransform.position;

        if (index < bullets.Length)
        {
            bullets[index].transform.SetParent(null);
            bullets[index].transform.position = bulletPoint.position;
            bullets[index].GetComponent<EnemyProjectile>().ActivateProjectile(playerPos);
            Debug.Log("Enemy fired a projectile at: " + playerPos + " from position: " + bulletPoint.position);
        }
    }

    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].activeInHierarchy)
                return i;
        }

        Debug.LogWarning("No available bullets found!");
        return 0;
    }

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("isDeath");
    }

    public void DestroySelf()
    {
        FindObjectOfType<EnemyManager>().EnemyKilled(100);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.right * rangeX / 2 * (transform.localScale.x / Mathf.Abs(transform.localScale.x));
        Vector3 size = new(rangeX, rangeY * 2, 0);
        Gizmos.DrawWireCube(center, size);
    }
}
