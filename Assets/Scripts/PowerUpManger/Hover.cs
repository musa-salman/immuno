using UnityEngine;

public class Hover : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float hoverSpeed = 1.2f;

    private float startY;

    void Start()
    {
        startY = transform.position.y;
    }

    void Update()
    {
        float newY = startY + amplitude * Mathf.Cos(Time.time * hoverSpeed);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
