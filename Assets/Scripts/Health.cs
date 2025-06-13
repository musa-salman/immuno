using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float regenRate = 0.01f;
    [SerializeField] private float regenDelay = 10f;

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSoundPlayer;
    [SerializeField] private AudioClip hurtSoundEnemy;
    [SerializeField] private AudioClip deathSoundPlayer;
    [SerializeField] private AudioClip deathSoundEnemy;





    public float currentHealth { get; private set; }

    private bool isDead = false;
    private float lastDamageTime;
    [SerializeField] private float hurtSoundCooldown = 0.5f;
    private float lastHurtSoundTime = -1f;


    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        lastDamageTime = Time.time;

        if (currentHealth > 0)
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
                    GetComponent<PlayerMovment>().enabled = false;
                }

                if (GetComponent<Enemy>() != null)
                {
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(0.25f);
        }

        if (!isDead && Time.time - lastDamageTime >= regenDelay && currentHealth < startingHealth)
        {
            RegenerateHealth();
        }
    }

    private void RegenerateHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth + regenRate * Time.deltaTime, 0, startingHealth);
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
        currentHealth = startingHealth;
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