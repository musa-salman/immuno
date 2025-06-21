using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float StartingHealth { get; set; }
    [SerializeField] private float regenerationRate = 0.1f;
    [SerializeField] private float regenerationDelay = 10f;

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSoundPlayer;
    [SerializeField] private AudioClip deathSoundPlayer;

    public float CurrentHealth { get; private set; }

    private bool isDead = false;
    private float lastDamageTime;
    [SerializeField] private float hurtSoundCooldown = 0.5f;
    private float lastHurtSoundTime = -1f;

    private void Start()
    {
        CurrentHealth = SkillManager.Instance.GetLevel("toughen_shell") + 1;
    }

    public void TakeDamage(float _damage)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 1, StartingHealth);
        lastDamageTime = Time.time;

        if (CurrentHealth > 0)
        {
            if (Time.time - lastHurtSoundTime >= hurtSoundCooldown)
            {
                SoundManager.instance.PlaySound(hurtSoundPlayer);


                lastHurtSoundTime = Time.time;
            }
        }
        else if (!isDead)
        {
            SoundManager.instance.PlaySound(deathSoundPlayer);
            if (TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = false;
            }

            if (TryGetComponent<Animator>(out var animator))
            {
                animator.SetTrigger("isDead");
            }
            GetComponent<PlayerMovement>().enabled = false;


            isDead = true;

            StartCoroutine(HandleDeath());
        }
    }

    private void Update()
    {
        StartingHealth = SkillManager.Instance.GetLevel("toughen_shell") + 1;

        if (!isDead && Time.time - lastDamageTime >= regenerationDelay && CurrentHealth < StartingHealth)
        {
            RegenerateHealth();
        }
    }

    private void RegenerateHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + regenerationRate * Time.deltaTime, 0, StartingHealth);
    }

    private IEnumerator HandleDeath()
    {
        GameManager.Instance.RegisterDeath();

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

        GetComponent<PlayerMovement>().enabled = true;

        lastDamageTime = Time.time;
    }
}