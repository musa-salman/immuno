using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private float currentHealth;
    private float StartingHealth { get; set; }

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private EnemyHealthBar enemyHealthBar;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private bool isDead = false;
    private void Awake()
    {
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
    }
    private void Start()
    {
        StartingHealth = maxHealth;
        currentHealth = StartingHealth;
    }

    public void TakeDamage(float _damage)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _damage = CheatManager.OneShotKill ? currentHealth + 1 : _damage;
#endif
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, StartingHealth);
        Debug.Log($"Enemy took damage: {_damage}. Current health: {currentHealth}/{maxHealth}");
        Debug.Log($"Enemy {enemyHealthBar}");
        if (enemyHealthBar != null)
            enemyHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth > 0)
        {
            SoundManager.instance.PlaySound(hurtSound);
        }
        else if (!isDead)
        {
            SoundManager.instance.PlaySound(deathSound);
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Die();
            }
            else if (TryGetComponent<Boss>(out var boss))
            {
                boss.Die();
            }
            else if (TryGetComponent<StomachEnemy>(out var stomachEnemy))
            {
                stomachEnemy.Die();
            }
            else if (TryGetComponent<StomachBoss>(out var stomachBoss))
            {
                stomachBoss.Die();
            }
            isDead = true;
        }
    }
}