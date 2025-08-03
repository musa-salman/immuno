using System.Collections;
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
    private int dmgUps = 0;
    private int instaHealth = 0;
    private int speedUps = 0;
    private int remainingUltraShields = 0;
    public bool isPowerUpActive = false;

    float powerUpCoolDownDuration = 5f;

    [Header("PowerUp Settings")]
    [SerializeField] private float speedUpDuration = 15f;
    [SerializeField] private float speedUpLevelsBoost = 25f;

    [SerializeField] private float ultraShieldDuration = 30f;

    [SerializeField] private float damageUpDuration = 30f;
    [SerializeField] private float damageUpBoostLevels = 0.75f;

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

    public void HandlePowerUp(PowerUpType type)
    {
        PlayerVisuals visuals = FindAnyObjectByType<PlayerVisuals>();

        switch (type)
        {
            case PowerUpType.DamageUp:
                if (dmgUps > 0)
                {
                    SkillManager.Instance.BoostFor(SkillManager.SkillType.ProjectilePower, damageUpBoostLevels, damageUpDuration);
                    dmgUps--;

                    visuals.StartGlow(new Color(1f, 0.5f, 0.5f), damageUpDuration);
                    powerUpCoolDownDuration = damageUpDuration;
                    StartCoroutine(PowerUpCooldown());
                }
                break;

            case PowerUpType.InstantHealth:
                if (instaHealth > 0)
                {
                    FindAnyObjectByType<PlayerHealth>().FullHealth();
                    instaHealth--;

                    visuals.StartGlow(new Color(0.5f, 1f, 0.5f), powerUpCoolDownDuration);
                    powerUpCoolDownDuration = powerUpCoolDownDuration;

                    StartCoroutine(PowerUpCooldown());
                }
                break;

            case PowerUpType.SpeedUp:
                if (speedUps > 0)
                {
                    SkillManager.Instance.BoostFor(SkillManager.SkillType.Speed, speedUpLevelsBoost, speedUpDuration);
                    speedUps--;

                    visuals.StartGlow(new Color(0.5f, 0.5f, 1f), speedUpDuration);
                    powerUpCoolDownDuration = speedUpDuration;
                    StartCoroutine(PowerUpCooldown());
                }
                break;

            case PowerUpType.UltraShield:
                if (remainingUltraShields > 0)
                {
                    StartCoroutine(FindAnyObjectByType<PlayerHealth>().FlashSprite());

                    visuals.StartGlow(new Color(0.9f, 0.6f, 1f), ultraShieldDuration);
                    powerUpCoolDownDuration = ultraShieldDuration;
                    StartCoroutine(UltraShieldCoroutine());
                    StartCoroutine(PowerUpCooldown());
                }
                break;
        }

        RefreshUi();
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void AddAllCollectibles(int amount)
    {
        dmgUps += amount;
        instaHealth += amount;
        speedUps += amount;
        remainingUltraShields += amount;

        RefreshUi();
    }

    public void ResetAllCollectibles()
    {
        dmgUps = 0;
        instaHealth = 0;
        speedUps = 0;
        remainingUltraShields = 0;

        RefreshUi();
    }
#endif



    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.PowerUp:
                ScoreManager.Instance.AddPoints(500);
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
        PlayerMovement playerMovement = FindAnyObjectByType<PlayerMovement>();
        playerMovement.canTakeDamage = false;

        yield return new WaitForSeconds(ultraShieldDuration);

        playerMovement.canTakeDamage = true;

        RefreshUi();
    }

    private IEnumerator PowerUpCooldown()
    {
        isPowerUpActive = true;

        DisableAllPowerUps();

        yield return new WaitForSeconds(powerUpCoolDownDuration);

        EnableAllPowerUps();
        isPowerUpActive = false;

    }

    private void DisableAllPowerUps()
    {
        PowerUpUI[] powerUpUIs = FindObjectsOfType<PowerUpUI>();
        foreach (PowerUpUI powerUpUI in powerUpUIs)
        {
            powerUpUI.DisableUi();
        }
    }

    private void EnableIfNotEmpty(PowerUpUI powerUpUi, int count)
    {
        if (count > 0)
        {
            powerUpUi.EnableUi();
        }
        else
        {
            powerUpUi.DisableUi();
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

        if (!isPowerUpActive)
            EnableAllPowerUps();
    }
}
