using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private GameObject attackPrefab;
    [Header("Projectile Pooling")]
    [SerializeField] private int bulletPoolSize = 20;
    private GameObject[] bulletPool;


    [Header("Minion Summon")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] summonPoints;

    private Animator animator;

    private GameObject[] summonedMinions;

    private float attackTimer;

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
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            AttackPattern();
            attackTimer = 0f;
        }
    }

    private void AttackPattern()
    {
        int pattern = Random.Range(0, 2);
        switch (pattern)
        {
            case 0:
                FireProjectiles();
                break;
            case 1:
                SummonMinions();
                break;
        }
    }

    private void FireProjectiles()
    {
        int projectileCount = 15;
        float angleStart = 0f;
        float angleEnd = 180f;
        float angleStep = (angleEnd - angleStart) / (projectileCount - 1);

        int bulletsUsed = 0;

        for (int i = 0; i < bulletPool.Length && bulletsUsed < projectileCount; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                float angle = angleStart + bulletsUsed * angleStep;

                float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(dirX, dirY).normalized;

                bulletPool[i].transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                bulletPool[i].SetActive(true);
                bulletPool[i].GetComponent<EnemyProjectile>().ActivateProjectile((Vector2)transform.position + direction);

                bulletsUsed++;
            }
        }
    }

    private void SummonMinions()
    {
        bool anyMinionDead = false;

        for (int i = 0; i < summonedMinions.Length; i++)
        {
            if (summonedMinions[i] == null || !summonedMinions[i].activeInHierarchy)
            {
                anyMinionDead = true;
                break;
            }
        }

        if (!anyMinionDead)
            return;

        for (int i = 0; i < summonPoints.Length; i++)
        {
            if (summonedMinions[i] == null || !summonedMinions[i].activeInHierarchy)
            {
                GameObject minion = Instantiate(minionPrefab, summonPoints[i].position, Quaternion.identity);
                summonedMinions[i] = minion;
            }
        }
    }

    public void Die()
    {
        animator.SetTrigger("isDeath");
    }

    public void DestroySelf()
    {
        FindObjectOfType<EnemyManager>().EnemyKilled(500);
        gameObject.SetActive(false);
    }
}
