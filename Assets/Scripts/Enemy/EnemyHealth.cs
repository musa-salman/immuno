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


    private bool isDead = false;

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

        if (currentHealth > 0)
        {
            SoundManager.instance.PlaySound(hurtSound);
        }
        else if (!isDead)
        {
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Die();
            }
            else if (TryGetComponent<Boss>(out var boss))
            {
                boss.Die();
            }
            isDead = true;
        }
    }
}