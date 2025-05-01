using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float regenRate = 0.01f;
    [SerializeField] private float regenDelay = 10f;

    public float currentHealth { get; private set; }

    private bool isDead = false;
    private float lastDamageTime; 

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
            // Play hurt animation or sound here
        }
        else
        {
            if (!isDead)
            {
                GetComponent<PlayerMovment>().enabled = false;
                isDead = true;
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
}