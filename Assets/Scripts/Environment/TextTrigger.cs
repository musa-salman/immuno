using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    [SerializeField] private bool isPlayerTriggered = true;
    [SerializeField] private bool isOneTime = false;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private TextPrompt textPrompt;
    private bool isTriggered = false;
    private bool isPlayerInteracting = false;
    private CircleCollider2D environmentCollider;

    void Start()
    {
        environmentCollider = GetComponent<CircleCollider2D>();
        environmentCollider.isTrigger = true;
        environmentCollider.radius = triggerDistance;
    }

    IEnumerator TriggerTextCoroutine()
    {
        Debug.Log("Triggering text prompt" + textPrompt);
        if (textPrompt != null)
        {
            textPrompt.ShowPrompt();
            while (isPlayerInteracting)
            {
                yield return null; // Wait until the player is no longer interacting
            }
            yield return new WaitForSeconds(displayDuration);
            textPrompt.HidePrompt();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isOneTime && isTriggered)
        {
            return; // Exit if this is a one-time trigger and it has already been triggered
        }
        if (!isPlayerTriggered)
        {
            return; // Exit if the player is already triggered
        }
        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            isPlayerInteracting = true;
            StartCoroutine(TriggerTextCoroutine());
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInteracting = false;
        }
        if (isOneTime && isTriggered)
        {
            return; // Exit if this is a one-time trigger and it has already been triggered
        }
        if (!isPlayerTriggered)
        {
            return; // Exit if the player is already triggered
        }
    }

}
