using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float StartingHealth { get; set; }
    public float flashSpeed = 0.2f; // Adjust for desired flash speed
    private SpriteRenderer spriteRenderer;

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSoundPlayer;
    [SerializeField] private AudioClip deathSoundPlayer;
    private PlayerMovement playerMovement;
    public float CurrentHealth { get; private set; }

    private bool isDead = false;
    private float lastDamageTime;

    private void Start()
    {
        CurrentHealth = SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.ToughenShell);
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void FullHealth()
    {
        CurrentHealth = StartingHealth;
    }

    public void TakeDamage(float _damage)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (CheatManager.UndeadMode && !CheatManager.SelfKill)
        {

            CurrentHealth = StartingHealth;
            return;
        }

        if (CheatManager.SelfKill)
        {
            _damage = CurrentHealth + 1;
        }

        Debug.Log($"Player took damage: {_damage}, Current Health: {CurrentHealth}");
#endif
        if (!playerMovement.canTakeDamage || isDead)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - _damage, 0, StartingHealth);
        lastDamageTime = Time.time;
        StartCoroutine(playerMovement.KnockBack(transform.localScale.x));
        StartCoroutine(FlashSprite());
        if (CurrentHealth > 0)
        {
            SoundManager.instance.PlaySound(hurtSoundPlayer);
        }
        if (CurrentHealth <= 0)
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
        StartingHealth = SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.ToughenShell);

        if (!isDead &&
            Time.time - lastDamageTime >= SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.RegenerationDelayReduction) &&
            CurrentHealth < StartingHealth)
        {
            RegenerateHealth();
        }
    }

    private void RegenerateHealth()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.HealthRegenerationRate) * Time.deltaTime,
                                    0, StartingHealth);
    }

    private IEnumerator HandleDeath()
    {
        GameManager.Instance.RegisterDeath();

        yield return new WaitForSeconds(1f);

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        SceneController.Instance.LoadScene(currentScene, () =>
        {
            CheckpointManager.Instance.RespawnPlayer();
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

    public IEnumerator FlashSprite()
    {
        while (!playerMovement.canTakeDamage)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashSpeed);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashSpeed);
        }
    }

}


