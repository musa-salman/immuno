using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float amplitud = 0.002f;
    public float hoverSpeed = 1.2f;
    void Update()
    {
        Vector3 hoverPosition = transform.position;
        hoverPosition.y += amplitud * Mathf.Cos(Time.time * hoverSpeed);
        transform.position = hoverPosition;
    }
}
