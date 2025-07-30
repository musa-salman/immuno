using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float knockbackForce = 6f;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip floatingSound;

    CollectionsManager collectionsManager;

    private Animator animator;

    private Rigidbody2D body;
    private Transform mainCamera;
    private int jumpCount = 0;

    private bool CanDash = true;
    private bool isDashing = false;
    private readonly float DashPower = 2f;
    private readonly float DashTime = 0.5f;

    private float originalCameraSize;

    private float knockbackTime = 0.25f;
    private bool isTakingDamage = false;
    public bool canTakeDamage = true;
    private readonly float damageCooldown = 1f;
    private bool isSolvingPuzzle = false;

    private GameObject enemyCollider;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main.transform;
        collectionsManager = CollectionsManager.Instance;

        /// find in children
        enemyCollider = GameObject.Find("EnemyCollider");
    }

    private void Update()
    {
        transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 2), Quaternion.Euler(0, 0, 0));
        if (isSolvingPuzzle)
        {
            return;
        }
        float speed_level = SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.Speed);
        float speed_limit = maxSpeed + (speed_level * 0.2f);
        mainCamera.position = new Vector3(transform.position.x, transform.position.y, mainCamera.position.z);

        if (isDashing || isTakingDamage)
        {
            return;
        }


        float horizontalInput = Input.GetAxis("Horizontal");
        var new_speed = body.velocity.x + horizontalInput * speed;
        var curr_vel = new Vector2(Mathf.Abs(new_speed) > speed_limit ? horizontalInput * speed_limit : new_speed, body.velocity.y);
        bool isWalking = Mathf.Abs(horizontalInput) > 0.01f && Mathf.Abs(body.velocity.x) > 0.1f && jumpCount == 0 && !isDashing;

        if (isWalking)
        {
            SoundManager.instance.PlaySound(floatingSound);
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

        if (Input.GetKeyDown(KeyCode.W) && jumpCount < SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.DoubleJump))
        {
            Jump();
        }

        HandleDash();
        if (collectionsManager.isPowerUpActive)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            collectionsManager.HandlePowerUp(PowerUpType.DamageUp);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            collectionsManager.HandlePowerUp(PowerUpType.SpeedUp);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            collectionsManager.HandlePowerUp(PowerUpType.UltraShield);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            collectionsManager.HandlePowerUp(PowerUpType.InstantHealth);

        }
    }

    private void Jump()
    {
        SoundManager.instance.PlaySound(jumpSound);
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        jumpCount++;
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanDash)
        {
            SoundManager.instance.PlaySound(dashSound);
            StartCoroutine(Dash(transform.localScale.x));
        }
    }

    private IEnumerator Dash(float direction)
    {
        CanDash = false;
        isDashing = true;
        canTakeDamage = false;

        enemyCollider.SetActive(false);
        float og_gravity = body.gravityScale;
        body.gravityScale = 0f;
        body.velocity = new Vector2(Mathf.Abs(body.velocity.x) > 0.1f ? body.velocity.x * DashPower : speed * direction * DashPower, body.velocity.y);
        yield return new WaitForSeconds(DashTime);
        body.gravityScale = og_gravity;
        isDashing = false;
        canTakeDamage = true;
        yield return new WaitForSeconds(SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.DashCooldownReduction));
        enemyCollider.SetActive(true);
        CanDash = true;
    }

    public IEnumerator KnockBack(float direction)
    {
        CanDash = false;
        isTakingDamage = true;
        canTakeDamage = false;
        body.velocity = new Vector2(body.velocity.x < 0 ? speed * -knockbackForce : speed * knockbackForce, knockbackForce);
        yield return new WaitForSeconds(knockbackTime);
        isTakingDamage = false;
        CanDash = true;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            jumpCount = 0;
        foreach (ContactPoint2D contact in collision.contacts)
        {

            if (contact.normal.y < -0.5f)
            {
                jumpCount = (int)SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.DoubleJump);
            }
        }


    }

    private void FixedUpdate()
    {
        animator.SetFloat("xVelocity", Mathf.Abs(body.velocity.x));
        animator.SetFloat("yVelocity", body.velocity.y);
    }

    public void SetPuzzleTransform(Transform puzzleTransform)
    {
        StartCoroutine(TransportWithFade(puzzleTransform));
    }

    private IEnumerator TransportWithFade(Transform puzzleTransform)
    {
        yield return FadeManager.Instance.FadeOut();

        isSolvingPuzzle = true;
        body.velocity = Vector2.zero;
        body.isKinematic = true;

        mainCamera.position = new Vector3(puzzleTransform.position.x, puzzleTransform.position.y, mainCamera.position.z);

        if (mainCamera.TryGetComponent<Camera>(out var cam))
        {
            originalCameraSize = cam.orthographicSize;
            cam.orthographicSize = originalCameraSize * 2f;
        }

        yield return FadeManager.Instance.FadeIn();
    }

    public void DoneSolvingPuzzle()
    {
        StartCoroutine(ExitPuzzleWithFade());
    }

    private IEnumerator ExitPuzzleWithFade()
    {
        yield return FadeManager.Instance.FadeOut();

        mainCamera.position = new Vector3(transform.position.x, transform.position.y, mainCamera.position.z);

        if (mainCamera.TryGetComponent<Camera>(out var cam))
        {
            cam.orthographicSize = originalCameraSize;
        }

        body.isKinematic = false;
        isSolvingPuzzle = false;

        yield return FadeManager.Instance.FadeIn();
    }

    public bool CanAttack()
    {
        return true;
    }

}
