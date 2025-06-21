using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float StartingHealth { get; set; }

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    public float CurrentHealth { get; private set; }

    private bool isDead = false;

    private void Start()
    {
        StartingHealth = 5;
        CurrentHealth = StartingHealth;
    }

    public void TakeDamage(float _damage)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _damage = CheatManager.OneShotKill ? CurrentHealth + 1 : _damage;
#endif
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, StartingHealth);

        if (CurrentHealth > 0)
        {
            SoundManager.instance.PlaySound(hurtSound);
        }
        else if (!isDead)
        {
            if (TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Die();
            }
            isDead = true;
        }
    }
}