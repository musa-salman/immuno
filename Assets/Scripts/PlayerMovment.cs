using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float speed = 5; 
    [SerializeField] private float jumpForce = 6f;

    private Rigidbody2D body; 
    private Transform mainCamera; 

    private int jumpCount = 0;

    private int maxJumps = 2;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2( horizontalInput* speed, body.velocity.y);

        if (mainCamera != null)
        {
            mainCamera.position = new Vector3(transform.position.x, transform.position.y, mainCamera.position.z);
        }

        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }

        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && jumpCount < maxJumps)
        {
            Jump();
            jumpCount++;
        }
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            jumpCount = 0;
        }
    }

}