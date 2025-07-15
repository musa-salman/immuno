
using System.Collections;
using UnityEngine;

public class StomachEnemy : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float rangeX;
    [SerializeField] private float rangeY;

    [Header("Spin Attack Parameters")]
    [SerializeField] private float maxSpinRange = 2f;
    [SerializeField] private float spinSpeed = 4f;
    [SerializeField] private float spinDuration = 1.5f;

    [SerializeField] private float spinCooldown = 1.5f;

    private float spinCooldownTimer = 0f;
    private bool isSpinning = false;
    private float spinTimer = 0f;


    [Header("Chase Parameters")]
    [SerializeField] private float chaseDuration = 2f;
    [SerializeField] private float chaseSpeed = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color patrolColor = Color.white;
    [SerializeField] private Color alertColor = Color.red;
    [SerializeField] private SpriteRenderer detectionIndicator;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip detectSound;

    [SerializeField] private LayerMask obstacleLayer;

    private Animator animator;

    private float cooldownTimer = Mathf.Infinity;
    private float chaseTimer = 0f;
    private bool hasSeenPlayer = false;
    private Transform playerTransform;
    private Vector3 initialScale;

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
        if (isSpinning)
        {
            SpinAttack();
            return;
        }

        cooldownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            hasSeenPlayer = true;
            chaseTimer = chaseDuration;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            float verticalDistanceToPlayer = Mathf.Abs(playerTransform.position.y - transform.position.y);
            if (verticalDistanceToPlayer <= maxSpinRange)
            {
                if (isSpinning)
                    return;

                if (spinCooldownTimer > 0f)
                {
                    spinCooldownTimer -= Time.deltaTime;
                    if (spinCooldownTimer <= 0f)
                    {
                        spinCooldownTimer = 0f;
                    }
                }
                else if (cooldownTimer >= spinCooldown)
                {
                    SpinAttack();
                    return;
                }
            }

            if (spriteRenderer != null)
                spriteRenderer.color = alertColor;
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

    private void SpinAttack()
    {
        if (playerTransform == null)
        {
            isSpinning = false;
            return;
        }

        if (!isSpinning)
        {
            isSpinning = true;
            spinTimer = spinDuration;
            spinCooldownTimer = spinCooldown;
            animator.SetBool("isSpin", true);
        }

        spinTimer -= Time.deltaTime;
        float direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
        transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * direction, initialScale.y, initialScale.z);
        transform.position += new Vector3(direction * spinSpeed * Time.deltaTime, 0, 0);

        if (spinTimer <= 0f)
        {
            isSpinning = false;
            animator.SetBool("isSpin", false);
            cooldownTimer = 0f;
            spinCooldownTimer = spinCooldown;
        }
    }

    private bool PlayerInSight()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (CheatManager.GhostMode)
            return false;
#endif

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
                if (hasSeenPlayer == false && detectSound != null)
                {
                    SoundManager.instance.PlaySound(detectSound);
                    StartCoroutine(showDetectionIndicator());
                }
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

    public void Die()
    {
        if (isSpinning)
        {
            DestroySelf();
            return;
        }
        animator.SetTrigger("isDeath");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSpinning)
            return;

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void DestroySelf()
    {
        FindObjectOfType<EnemyManager>().EnemyKilled(100);
        gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + Vector3.right * rangeX / 2 * (transform.localScale.x / Mathf.Abs(transform.localScale.x));
        Vector3 size = new(rangeX, rangeY * 2, 0);
        Gizmos.DrawWireCube(center, size);
    }
    private IEnumerator showDetectionIndicator()
    {
        detectionIndicator.SetActive(true);
        yield return new WaitForSeconds(1f);
        detectionIndicator.SetActive(false);
    }
}
