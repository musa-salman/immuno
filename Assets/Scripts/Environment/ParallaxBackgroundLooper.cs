using UnityEngine;

public class ParallaxBackgroundLooper : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector2 chunkSize = new Vector2(10f, 10f);
    public GameObject[] chunks;

    private Vector2 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;

        Vector3 centerOffset = new Vector3(-chunkSize.x / 2f, -chunkSize.y / 2f, 0f);
        foreach (GameObject chunk in chunks)
        {
            chunk.transform.position += centerOffset;
        }
    }

    void Update()
    {
        foreach (GameObject chunk in chunks)
        {
            Vector2 offset = cameraTransform.position - chunk.transform.position;

            if (Mathf.Abs(offset.x) > chunkSize.x * 1.5f)
            {
                float moveX = Mathf.Sign(offset.x) * chunkSize.x * 3f;
                chunk.transform.position += new Vector3(moveX, 0f, 0f);
            }

            if (Mathf.Abs(offset.y) > chunkSize.y * 1.5f)
            {
                float moveY = Mathf.Sign(offset.y) * chunkSize.y * 3f;
                chunk.transform.position += new Vector3(0f, moveY, 0f);
            }
        }

        lastCameraPosition = cameraTransform.position;
    }
}

