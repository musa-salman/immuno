using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


public enum PowerUpType
{
    PowerUp,
    DamageUp,
    SpeedUp,
    UltraShield,
    InstantHealth,
    None
}
public class CollectionsManager : MonoBehaviour
{
    public static CollectionsManager Instance;
    // private int powerUpsCollected = 0;
    private int dmgUps = 0;
    private int instaHealth = 0;
    private int speedUps = 0;
    private int remainingUltraShields = 0;
    public bool powerUpActive = false;

    float powerUpCoolDownDuration = 5f;

    [Header("PowerUp Settings")]
    [SerializeField] private float speedUpDuration = 10f;
    [SerializeField] private float speedUpLevelsBoost = 2f;

    [SerializeField] private float ultraShieldDuration = 5f;
    // [SerializeField] private float powerUpAddedXp = 500f;

    [SerializeField] private float damageUpDuration = 5f;
    [SerializeField] private float damageUpBoostLevels = 0.25f;

    [SerializeField] private float instantHealthAmount = 50f;

    private PlayerMovement playerMovement;
    private UpgradeMenuToggle upgradeMenuToggle;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        upgradeMenuToggle = FindObjectOfType<UpgradeMenuToggle>();
    }

    public void HandlePowerUp(PowerUpType type)
    {
        switch (type)
        {

            case PowerUpType.DamageUp:
                if (dmgUps > 0)
                {
                    SkillManager.Instance.BoostFor(SkillManager.SkillType.ProjectilePower, damageUpBoostLevels, damageUpDuration);
                    dmgUps--;
                    StartCoroutine(PowerUpCooldown());

                }
                break;

            case PowerUpType.InstantHealth:
                if (instaHealth > 0)
                {
                    playerHealth.AddHealth(instantHealthAmount);
                    instaHealth--;
                    StartCoroutine(PowerUpCooldown());

                }
                break;

            case PowerUpType.SpeedUp:
                if (speedUps > 0)
                {
                    SkillManager.Instance.BoostFor(SkillManager.SkillType.Speed, speedUpLevelsBoost, speedUpDuration);
                    speedUps--;
                    StartCoroutine(PowerUpCooldown());

                }
                break;
            case PowerUpType.UltraShield:
                if (remainingUltraShields > 0)
                {
                    StartCoroutine(playerHealth.FlashSprite());
                    StartCoroutine(UltraShieldCoroutine());
                    StartCoroutine(PowerUpCooldown());
                }
                break;
        }

        RefreshUi();
    }

    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.PowerUp:
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddPoints(500);
                }
                if (upgradeMenuToggle != null)
                {
                    upgradeMenuToggle.show_menu();
                }
                break;
            case PowerUpType.DamageUp:
                dmgUps++;
                break;

            case PowerUpType.InstantHealth:
                instaHealth++;
                break;

            case PowerUpType.SpeedUp:
                speedUps++;
                break;

            case PowerUpType.UltraShield:
                remainingUltraShields++;
                break;
        }

        RefreshUi();
    }

    private IEnumerator UltraShieldCoroutine()
    {
        remainingUltraShields--;
        playerMovement.canTakeDamage = false;
        yield return new WaitForSeconds(ultraShieldDuration);
        playerMovement.canTakeDamage = true;

        RefreshUi();

    }

    private IEnumerator PowerUpCooldown()
    {
        powerUpActive = true;

        DisableAllPowerUps();

        yield return new WaitForSeconds(powerUpCoolDownDuration);

        EnableAllPowerUps();
        powerUpActive = false;

    }

    private void DisableAllPowerUps()
    {
        PowerUpUI[] powerUpUIs = FindObjectsOfType<PowerUpUI>();
        foreach (PowerUpUI powerUpUI in powerUpUIs)
        {
            powerUpUI.DisableUI();
        }
    }

    private void EnableIfNotEmpty(PowerUpUI powerUpUI, int count)
    {
        if (count > 0)
        {
            powerUpUI.EnableUi();
        }
        else
        {
            powerUpUI.DisableUI();
        }
    }

    private void EnableAllPowerUps()
    {
        PowerUpUI[] powerUpUIs = FindObjectsOfType<PowerUpUI>();
        foreach (PowerUpUI powerUpUI in powerUpUIs)
        {
            switch (powerUpUI.Id)
            {
                case PowerUpType.DamageUp:
                    EnableIfNotEmpty(powerUpUI, dmgUps);
                    break;
                case PowerUpType.SpeedUp:
                    EnableIfNotEmpty(powerUpUI, speedUps);
                    break;
                case PowerUpType.UltraShield:
                    EnableIfNotEmpty(powerUpUI, remainingUltraShields);
                    break;
                case PowerUpType.InstantHealth:
                    EnableIfNotEmpty(powerUpUI, instaHealth);
                    break;
            }
        }
    }

    public void RefreshUi()
    {
        PowerUpUI[] powerUpUIs = FindObjectsOfType<PowerUpUI>();
        foreach (PowerUpUI powerUpUI in powerUpUIs)
        {
            switch (powerUpUI.Id)
            {
                case PowerUpType.DamageUp:
                    powerUpUI.SetCounterText(dmgUps);
                    break;
                case PowerUpType.SpeedUp:
                    powerUpUI.SetCounterText(speedUps);
                    break;
                case PowerUpType.UltraShield:
                    powerUpUI.SetCounterText(remainingUltraShields);
                    break;
                case PowerUpType.InstantHealth:
                    powerUpUI.SetCounterText(instaHealth);
                    break;
            }
        }

        EnableAllPowerUps();
    }
}
