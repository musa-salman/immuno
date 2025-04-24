using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float dashForce = 125f; 
    [SerializeField] private float dashCooldown = 0.5f;

    private Rigidbody2D body;
    private Transform mainCamera;
    private int jumpCount = 0;
    private int maxJumps = 2;

    private float lastDashTime = -Mathf.Infinity;
    private float lastKeyPressTime = -Mathf.Infinity;
    private string lastKeyPressed = "";


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        float horizontalInput = Input.GetAxis("Horizontal");

        // HandleDash();

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

        if ((Input.GetKeyDown(KeyCode.W)) && jumpCount < maxJumps)
        {
            Jump();
        }
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        jumpCount++;
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (lastKeyPressed == "D" && Time.time - lastKeyPressTime < 0.2f && Time.time - lastDashTime > dashCooldown)
            {
                Teleport(Vector2.right);
            }
            lastKeyPressed = "D";
            lastKeyPressTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (lastKeyPressed == "A" && Time.time - lastKeyPressTime < 0.2f && Time.time - lastDashTime > dashCooldown)
            {
                Teleport(Vector2.left);
            }
            lastKeyPressed = "A";
            lastKeyPressTime = Time.time;
        }
    }

    private void Teleport(Vector2 direction)
    {
        float teleportDistance = 5f;
        Vector3 newPosition = transform.position + new Vector3(direction.x * teleportDistance, 0, 0);
        transform.position = newPosition;
        lastDashTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            jumpCount = 0;
        }
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                jumpCount = maxJumps;
            }
        }
    }

    public bool canAttack()
    {
        return true;
    } 

}