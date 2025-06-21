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
        Debug.Log($"Enemy took damage: {CurrentHealth} - {_damage}");
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, StartingHealth);

        if (CurrentHealth > 0)
        {
            SoundManager.instance.PlaySound(hurtSound);
        }
        else if (!isDead)
        {
            GetComponent<Enemy>().Die();
            isDead = true;
        }
    }
}