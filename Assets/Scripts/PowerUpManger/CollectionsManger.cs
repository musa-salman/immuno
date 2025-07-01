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
public class CollectionsMannger : MonoBehaviour
{
    public static CollectionsMannger Instance;
    // private int powerUpsCollected = 0;
    private int dmgUps = 0;
    private int instaHealth = 0;
    private int speedUps = 0;
    private int ultraShileds = 0;
    public bool powerUpActive = false;

    float powerUpCoolDownDuration = 5f;

    [Header("PowerUp Settings")]
    [SerializeField] private float speedUpDuration = 10f;
    [SerializeField] private int speedUpLevelsBoost = 3;
    [SerializeField] private float ultraShieldDuration = 5f;
    // [SerializeField] private float powerUpAddedXp = 500f;
    [SerializeField] private float damageUpDuration = 5f;
    [SerializeField] private int damageUpBoostLevels = 3;
    [SerializeField] private float instantHealthAmount = 50f;

    [Header("PLayer scripts")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SkillManager skillManager;

    [Header("UI elements")]
    [SerializeField] private PowerUpUI damageUpUI;
    [SerializeField] private PowerUpUI speedUpUI;
    [SerializeField] private PowerUpUI ultraShieldUI;
    [SerializeField] private PowerUpUI instantHealthUI;



   
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
    public void handlePowerUp(PowerUpType type)
    {
        switch (type)
        {
            // case PowerUpType.PowerUp:
            //     powerUpsCollected++;
            //     if (powerUpsCollected >= 5 && !powerUpActive)
            //     {
            //         playerMovement.ActivatePowerUp();
            //         powerUpActive = true;
            //     }
            //     break;
            case PowerUpType.DamageUp:
                if (dmgUps > 0)
                {
                    skillManager.BoostFor("Power", damageUpBoostLevels, damageUpDuration);
                    dmgUps--;
                    damageUpUI.setCounterText(dmgUps);
                    StartCoroutine(PowerUpCooldown());

                }
                break;

            case PowerUpType.InstantHealth:
                if (instaHealth > 0)
                {
                    playerHealth.addHealth(instantHealthAmount);
                    instaHealth--;
                    instantHealthUI.setCounterText(instaHealth);
                    StartCoroutine(PowerUpCooldown());

                }
                break;

            case PowerUpType.SpeedUp:
                if (speedUps > 0)
                {
                    skillManager.BoostFor("surge_motion", speedUpLevelsBoost, speedUpDuration);
                    speedUps--;
                    speedUpUI.setCounterText(speedUps);
                     StartCoroutine(PowerUpCooldown());
                    
                }
                break;
            case PowerUpType.UltraShield:
                if (ultraShileds > 0)
                {
                    StartCoroutine(playerHealth.FlashSprite());
                    StartCoroutine(UltraShieldCoroutine());
                    StartCoroutine(PowerUpCooldown());
                }
                break;
        }
    }
  
    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {

            case PowerUpType.DamageUp:
                dmgUps++;
                damageUpUI.setCounterText(dmgUps);
                if (!powerUpActive)
                {
                    damageUpUI.EnableUI();
                }
                break;

            case PowerUpType.InstantHealth:
                instaHealth++;
                instantHealthUI.setCounterText(instaHealth);
                if (!powerUpActive)
                {
                    instantHealthUI.EnableUI();
                }
                break;

            case PowerUpType.SpeedUp:
                speedUps++;
                speedUpUI.setCounterText(speedUps);
                if (!powerUpActive)
                {
                    speedUpUI.EnableUI();
                }
                break;

            case PowerUpType.UltraShield:
                ultraShileds++;
                ultraShieldUI.setCounterText(ultraShileds);
                if (!powerUpActive)
                {
                    ultraShieldUI.EnableUI();
                }
                break;
        }
    }
    private IEnumerator UltraShieldCoroutine()
    {
        ultraShileds--;
        playerMovement.canTakeDamage = false;
        yield return new WaitForSeconds(ultraShieldDuration);
        playerMovement.canTakeDamage = true;
        ultraShieldUI.setCounterText(ultraShileds);

    }
    private IEnumerator PowerUpCooldown()
    {
        powerUpActive = true;
        damageUpUI.DisableUI();
        instantHealthUI.DisableUI();
        speedUpUI.DisableUI();
        ultraShieldUI.DisableUI();
        yield return new WaitForSeconds(powerUpCoolDownDuration);
        if (dmgUps > 0)
        {
            damageUpUI.EnableUI();
        }
        if (instaHealth > 0)
        {
            instantHealthUI.EnableUI();
        }
        if (speedUps > 0)
        {
            speedUpUI.EnableUI();
        }
        if (ultraShileds > 0)
        {
            ultraShieldUI.EnableUI();
        }
        powerUpActive = false;
      
    }
}
