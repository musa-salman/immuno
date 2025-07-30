using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private GameObject attackPrefab;
    [Header("Projectile Pooling")]
    [SerializeField] private int bulletPoolSize = 20;
    private GameObject[] bulletPool;


    [Header("Minion Summon")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] summonPoints;

    [SerializeField]
    private EnemyPatrol enemyPatrol;

    private Animator animator;

    private GameObject[] summonedMinions;

    private float attackTimer;

    private float totalRotationZ = 0f;

    private enum BossState
    {
        Patrolling,
        PreparingAttack,
        RotatingAndFiring,
        FinishingRotation,
        Summoning
    }

    private bool isPhase2 = false;
    private bool isDead = false;

    private BossState currentState = BossState.Patrolling;

    private EnemyHealth enemyHealth;

    private void Start()
    {
        bulletPool = new GameObject[bulletPoolSize];

        for (int i = 0; i < bulletPoolSize; i++)
        {
            bulletPool[i] = Instantiate(attackPrefab);
            bulletPool[i].SetActive(false);
        }

        summonedMinions = new GameObject[summonPoints.Length];

        animator = GetComponent<Animator>();

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (isDead) return;

        if (!isPhase2 && enemyHealth.CurrentHealth / enemyHealth.MaxHealth <= 0.5f)
        {
            StartCoroutine(EnterPhase2());
        }

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

            case BossState.RotatingAndFiring:
                RotateBoss();
                break;
        }
    }

    private IEnumerator EnterPhase2()
    {
        isPhase2 = true;
        currentState = BossState.PreparingAttack;
        enemyPatrol.StopMovement();

        yield return PlayPreparationEffect(1.5f, 0.1f);

        attackCooldown *= 0.7f;

        Debug.Log("Boss entered Phase 2");

        currentState = BossState.Patrolling;
        enemyPatrol.ResumeMovement();
    }

    private void RotateBoss()
    {
        float delta = 90f * Time.deltaTime;
        transform.Rotate(0f, 0f, delta);
        totalRotationZ += delta;
    }

    private void ChooseAttack()
    {
        enemyPatrol.StopMovement();

        int pattern;
        if (isPhase2)
        {
            pattern = CanSummonMinions() ? Random.Range(0, 2) : 0;
        }
        else
        {
            pattern = 0;
        }

        switch (pattern)
        {
            case 0:
                StartCoroutine(FireInAllDirections());
                break;
            case 1:
                StartCoroutine(PrepareAndSummon());
                break;
        }
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


    private IEnumerator FireInAllDirections()
    {
        yield return StartCoroutine(PlayPreparationEffect());
        currentState = BossState.RotatingAndFiring;

        float duration = 2f;
        float elapsed = 0f;
        int projectileCount = 30;

        while (elapsed < duration)
        {
            float angleStep = 360f / projectileCount;
            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * angleStep;
                Vector2 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                FireBullet(dir);
            }

            yield return new WaitForSeconds(0.2f);
            elapsed += 0.3f;
        }

        currentState = BossState.FinishingRotation;
        yield return StartCoroutine(ReturnToIdleRotation());
        currentState = BossState.Patrolling;
        enemyPatrol.ResumeMovement();
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

    private IEnumerator ReturnToIdleRotation()
    {
        float speed = 90f;

        float targetRotation = Mathf.Floor(totalRotationZ / 360f) * 360f;

        int it = 0;
        while (totalRotationZ > targetRotation)
        {
            float delta = speed * Time.deltaTime;

            transform.Rotate(0f, 0f, -delta);
            totalRotationZ -= delta;

            if (it % 10 == 0)
            {
                float angle = totalRotationZ % 360f;
                Vector2 dir = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                FireBullet(dir);
            }

            it++;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        totalRotationZ = 0f;
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

    public void Die()
    {
        isDead = true;
        animator.SetTrigger("isDeath");
    }

    public void DestroyBoss()
    {
        FindObjectOfType<EnemyManager>().EnemyKilled(500);

        foreach (GameObject minion in summonedMinions)
        {
            if (minion != null)
            {
                Destroy(minion);
            }
        }
        FindObjectOfType<Objective>().CompleteObjective();

        gameObject.SetActive(false);
    }
}
