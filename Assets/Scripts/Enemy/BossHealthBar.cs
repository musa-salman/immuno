using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;


    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        Debug.Log($"Updating health bar: {currentHealth}/{maxHealth}");
        slider.value = currentHealth / maxHealth;
    }

}
