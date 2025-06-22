using System.Collections;
using UnityEngine;

public class StomachBoss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private GameObject attackPrefab;
    [Header("Projectile Pooling")]
    [SerializeField] private int bulletPoolSize = 20;
    private GameObject[] bulletPool;

    [Header("Spin Attack")]
    [SerializeField] private float spinSpeed = 4f;
    [SerializeField] private float spinDuration = 5f;
    [SerializeField] private float spinDamage = 10f;

    private float spinTimer = 0f;
    private bool isSpinning = false;
    private Vector3 initialScale;
    private Transform playerTransform;



    [Header("Minion Summon")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] summonPoints;

    [SerializeField]
    private EnemyPatrol enemyPatrol;

    private Animator animator;

    private GameObject[] summonedMinions;

    private float attackTimer;


    private enum BossState
    {
        Patrolling,
        PreparingAttack,
        SpinningAndFiring,
        SpinningCharge,
        Summoning
    }

    private BossState currentState = BossState.Patrolling;


    private void Start()
    {
        initialScale = transform.localScale;

        bulletPool = new GameObject[bulletPoolSize];

        for (int i = 0; i < bulletPoolSize; i++)
        {
            bulletPool[i] = Instantiate(attackPrefab);
            bulletPool[i].SetActive(false);
        }

        summonedMinions = new GameObject[summonPoints.Length];

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Do not rotate the boss
        transform.rotation = Quaternion.identity;
        switch (currentState)
        {
            case BossState.Patrolling:
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    ChooseAttack();
                    attackTimer = 0f;
                }
                break;

        }
    }

    private void ChooseAttack()
    {
        enemyPatrol.StopMovement();

        bool canSummon = CanSummonMinions();
        int pattern = Random.Range(0, canSummon ? 2 : 1);

        switch (pattern)
        {
            case 0:
                StartCoroutine(SpinAndFire());
                break;
            case 1:
                StartCoroutine(PrepareAndSummon());
                break;
        }
    }

    private IEnumerator SpinAndFire()
    {
        yield return StartCoroutine(PlayPreparationEffect());

        currentState = BossState.SpinningAndFiring;
        isSpinning = true;
        spinTimer = spinDuration;
        animator.SetBool("isSpin", true);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        float direction = 1f;
        if (playerTransform != null)
        {
            direction = Mathf.Sign(playerTransform.position.x - transform.position.x);
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x) * direction, initialScale.y, initialScale.z);
        }

        float fireInterval = 1f;
        float fireTimer = 0f;
        int projectileCount = 10;

        while (spinTimer > 0f)
        {
            spinTimer -= Time.deltaTime;
            fireTimer += Time.deltaTime;

            // Move the boss while spinning
            transform.position += new Vector3(direction * spinSpeed * Time.deltaTime, 0, 0);

            if (fireTimer >= fireInterval)
            {
                fireTimer = 0f;

                // Shoot in all directions
                float angleStep = 360f / projectileCount;
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = i * angleStep;
                    Vector2 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    FireBullet(dir);
                }
            }

            yield return null;
        }

        isSpinning = false;
        animator.SetBool("isSpin", false);
        currentState = BossState.Patrolling;
        enemyPatrol.ResumeMovement();
    }


    private bool CanSummonMinions()
    {
        for (int i = 0; i < summonedMinions.Length; i++)
        {
            if (summonedMinions[i] == null || !summonedMinions[i].activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator PlayPreparationEffect(float duration = 1.2f, float intensity = 0.05f)
    {
        currentState = BossState.PreparingAttack;

        float time = 0f;
        Vector3 originalScale = transform.localScale;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color originalColor = sr != null ? sr.color : Color.white;

        while (time < duration)
        {
            float pulse = 1f + Mathf.Sin(time * 10f) * intensity;
            transform.localScale = originalScale * pulse;

            if (sr != null)
            {
                float glow = 0.5f + 0.5f * Mathf.Sin(time * 20f);
                sr.color = Color.Lerp(originalColor, new Color(1f, 0.5f, 1f), glow);
            }

            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        if (sr != null) sr.color = originalColor;
    }


    private void FireBullet(Vector2 direction)
    {
        for (int i = 0; i < bulletPool.Length; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                bulletPool[i].transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                bulletPool[i].SetActive(true);
                bulletPool[i].GetComponent<EnemyProjectile>().ActivateProjectile((Vector2)transform.position + direction);
                break;
            }
        }
    }

    private IEnumerator PrepareAndSummon()
    {
        yield return PlayPreparationEffect(1.2f, 0.05f);
        SummonMinions();
    }


    private void SummonMinions()
    {
        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonedMinions[i] == null || !summonedMinions[i].activeInHierarchy)
            {
                if (summonedMinions[i] != null)
                {
                    Destroy(summonedMinions[i]);
                }
                GameObject minion = Instantiate(minionPrefab, summonPoints[i].position, Quaternion.identity);
                summonedMinions[i] = minion;
            }
        }

        currentState = BossState.Patrolling;
        enemyPatrol.ResumeMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSpinning) return;

        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(spinDamage);
            }
        }
    }


    public void Die() => animator.SetTrigger("isDeath");

    public void DestroyBoss()
    {
        FindObjectOfType<EnemyManager>().EnemyKilled(500);
        gameObject.SetActive(false);
    }
}
