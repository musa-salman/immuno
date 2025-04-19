using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform camera;
    public float speed;
    void Update()
    {
        Vector3 newPosition = new Vector3(camera.position.x * speed, camera.position.y * speed, transform.position.z);

        transform.position = newPosition;
        
    }
}
