using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private PlayerHealth playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    private void Start()
    {
        totalHealthBar.fillAmount = SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.ToughenShell) / 10f;
        currentHealthBar.fillAmount = totalHealthBar.fillAmount;
    }

    private void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        currentHealthBar.fillAmount = playerHealth.CurrentHealth / 10;
    }
}