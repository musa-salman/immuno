using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    [SerializeField] private float speed;
    [SerializeField] private float resetTime;

    private float lifetime;
    private Vector2 moveDirection;

    public void ActivateProjectile(Vector2 targetPosition)
    {
        lifetime = 0f;

        Vector2 startPosition = transform.position;
        moveDirection = (targetPosition - startPosition).normalized;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * moveDirection, Space.World);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player") || collision.CompareTag("Ground"))
            gameObject.SetActive(false);
    }
}
