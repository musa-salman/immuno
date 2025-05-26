using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerMovment : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float dashSpeed = 20f; 
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private float maxSpeed = 5f;

    private Animator animator;

    private Rigidbody2D body;
    private Transform mainCamera;
    private int jumpCount = 0;
    private int maxJumps = 2;

    private char lastKeyPressed;
    private float lastKeyPressTime = -Mathf.Infinity;
    private bool CanDash = true;
    private bool isDashing = false;
    private float DashPower = 2f;
    private float DashTime = 0.5f;
    private float DashCooldown = 1.5f;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main.transform;
        
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (mainCamera != null)
        {
            mainCamera.position = new Vector3(transform.position.x, transform.position.y, mainCamera.position.z);
        }
        if (isDashing)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        var new_speed = body.velocity.x + horizontalInput * speed;
        var curr_vel = new Vector2(Mathf.Abs(new_speed) > maxSpeed ? horizontalInput * maxSpeed : new_speed, body.velocity.y);

        if (Mathf.Abs(curr_vel.x) < 0.8f)
        {
            curr_vel.x = 0;
        }

        body.velocity = curr_vel;
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

        HandleDash();
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        jumpCount++;
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash)
        {
            StartCoroutine(Dash(transform.localScale.x));
        }
    }

    private IEnumerator Dash(float direction)
    {
        CanDash = false;
        isDashing = true;
        float og_gravity = body.gravityScale;
        body.gravityScale = 0f;
        body.velocity = new Vector2(Mathf.Abs(body.velocity.x) > 0.1f? body.velocity.x * DashPower : speed * direction * DashPower, body.velocity.y);
        Debug.Log(body.velocity.x);
        yield return new WaitForSeconds(DashTime);
        body.gravityScale = og_gravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        CanDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            jumpCount = 0;
        foreach (ContactPoint2D contact in collision.contacts)
        {

            if (contact.normal.y < -0.5f)
            {
                jumpCount = maxJumps;
            }
        }


    }

    private void FixedUpdate() 
    {
        animator.SetFloat("xVelocity", Mathf.Abs(body.velocity.x));
        animator.SetFloat("yVelocity", body.velocity.y);
    }

    public bool canAttack()
    {
        return true;
    } 

}
