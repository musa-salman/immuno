using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    private Transform cam;
    private Vector3 lastCamPos;

    public Vector2 parallaxMultiplier = new Vector2(0.1f, 0.05f);

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cam.position - lastCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0f);
        lastCamPos = cam.position;
    }
}

