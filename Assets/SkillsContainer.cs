using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsContainer : MonoBehaviour
{
    public void AddToChildren(GameObject skillUI)
    {
        if (skillUI != null)
        {
            skillUI.transform.SetParent(transform);
        }
        else
        {
            Debug.LogWarning("Attempted to add a null skill UI to SkillsContainer.");
        }
    }
}
