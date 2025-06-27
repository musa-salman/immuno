using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{


    [SerializeField] private Slider slider;
    private Transform target;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        target = GetComponentInParent<EnemyHealth>().transform;

        Assert.IsNotNull(target, "Enemy target not found for health bar.");
        Assert.IsNotNull(slider, "Slider component not assigned in EnemyHealthBar.");
    }

    public void UpdateHealthBar(float currHealth, float maxHealth)
    {
        slider.value = currHealth / maxHealth;
    }

    void Update()
    {
        transform.SetPositionAndRotation(target.position + offset, Camera.main.transform.rotation);
    }
}
