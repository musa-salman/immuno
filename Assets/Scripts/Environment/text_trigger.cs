using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class text_t : MonoBehaviour
{
    [SerializeField] private bool player_triggerd = false;
    [SerializeField] private bool was_triggered = false;
    [SerializeField] private bool is_one_time = false;
    [SerializeField] private float disply_duration = 2f;
    [SerializeField] private float trigger_distance = 5f;
    [SerializeField] private TextPrompt textPrompt;
    private CircleCollider2D collider;

    void Start()
    {
        collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            Debug.LogError("CircleCollider2D component not found on the text trigger object.");
        }
        else
        {
            collider.isTrigger = true;
            collider.radius = trigger_distance;
        }
    }
    IEnumerator TriggerTextCoroutine()
    {

        if (textPrompt != null)
        {
        Debug.Log("running text trigger coroutine");

            textPrompt.ShowPrompt();
            yield return new WaitForSeconds(disply_duration);
            textPrompt.HidePrompt();
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (is_one_time && was_triggered)
        {
            return; // Exit if this is a one-time trigger and it has already been triggered
        }
        if (!player_triggerd)
        {
            return; // Exit if the player is already triggered
        }
        if (other.CompareTag("Player"))
        {
            player_triggerd = true;
            was_triggered = true;
            StartCoroutine(TriggerTextCoroutine());
        }
    }
   
}
