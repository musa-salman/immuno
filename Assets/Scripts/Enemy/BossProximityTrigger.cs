using UnityEngine;

public class BossProximityTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject objectToActivate;

    [Header("Trigger Settings")]
    [SerializeField] private float triggerDistance = 5f;

    private bool hasTriggered = false;
    private void Start()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }
    private void Update()
    {
        if (hasTriggered || player == null || objectToActivate == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= triggerDistance)
        {
            hasTriggered = true;
            objectToActivate.SetActive(true);
            Debug.Log("✅ Player is close! Boss UI activated.");
        }
    }
}
