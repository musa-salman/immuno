using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; 
    private Rigidbody2D rb; 
    private Vector2 movement; 
    private Transform mainCamera; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (mainCamera != null)
        {
            mainCamera.position = new Vector3(transform.position.x, transform.position.y, mainCamera.position.z);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}