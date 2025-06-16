using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float StartingHealth { get; set; }
    [SerializeField] private float regenRate = 0.01f;
    [SerializeField] private float regenDelay = 10f;

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSoundPlayer;
    [SerializeField] private AudioClip hurtSoundEnemy;
    [SerializeField] private AudioClip deathSoundPlayer;
    [SerializeField] private AudioClip deathSoundEnemy;

    private EnemyManager enemyManager;

    public float CurrentHealth { get; private set; }

    private bool isDead = false;
    private float lastDamageTime;
    [SerializeField] private float hurtSoundCooldown = 0.5f;
    private float lastHurtSoundTime = -1f;


    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        if (GetComponent<PlayerMovment>() != null)
        {
            StartingHealth = SkillManager.Instance.GetLevel("toughen_shell") + 1;
        }
        else if (GetComponent<Enemy>() != null)
        {
            StartingHealth = 5;
        }

        CurrentHealth = StartingHealth;
    }

    public void TakeDamage(float _damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, StartingHealth);
        lastDamageTime = Time.time;

        if (CurrentHealth > 0)
        {
            if (Time.time - lastHurtSoundTime >= hurtSoundCooldown)
            {
                if (GetComponent<PlayerMovment>() != null)
                {
                    SoundManager.instance.PlaySound(hurtSoundPlayer);
                }
                else if (GetComponent<Enemy>() != null)
                {
                    SoundManager.instance.PlaySound(hurtSoundEnemy);
                }

                lastHurtSoundTime = Time.time;
            }
        }
        else
        {
            if (!isDead)
            {
                if (GetComponent<PlayerMovment>() != null)
                {
                    SoundManager.instance.PlaySound(deathSoundPlayer);
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = Vector2.zero;
                        rb.bodyType = RigidbodyType2D.Kinematic;
                        rb.simulated = false;
                    }

                    Animator animator = GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("isDead");
                    }
                    GetComponent<PlayerMovment>().enabled = false;
                }

                if (GetComponent<Enemy>() != null)
                {
                    enemyManager.EnemyKilled(100);
                    SoundManager.instance.PlaySound(deathSoundEnemy);
                    GetComponent<Enemy>().enabled = false;
                    gameObject.SetActive(false);
                }
                isDead = true;

                if (GetComponent<PlayerMovment>() != null)
                {
                    StartCoroutine(HandleDeath());
                }
            }
        }
    }

    private void Update()
    {
        if (GetComponent<PlayerMovment>() != null)
        {
            StartingHealth = SkillManager.Instance.GetLevel("toughen_shell") + 1;

        }
        if (!isDead && Time.time - lastDamageTime >= regenDelay && CurrentHealth < StartingHealth)
        {
            RegenerateHealth();
        }
    }

    private void RegenerateHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + regenRate * Time.deltaTime, 0, StartingHealth);
    }

    private IEnumerator HandleDeath()
    {
        GameManager.Instance?.RegisterDeath();

        yield return new WaitForSeconds(1f);

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SceneController.Instance.LoadScene(currentScene, () =>
        {
            Debug.Log("Respawning player...");
        });
    }

    public void ResetStats()
    {
        CurrentHealth = StartingHealth;
        isDead = false;

        if (GetComponent<PlayerMovment>() != null)
        {
            GetComponent<PlayerMovment>().enabled = true;
        }

        if (GetComponent<Enemy>() != null)
        {
            GetComponent<Enemy>().enabled = true;
            gameObject.SetActive(true);
        }
        lastDamageTime = Time.time;
    }
}