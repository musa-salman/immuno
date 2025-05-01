
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;

    private bool hit;

    private BoxCollider2D boxCollider;

    private float direction;

    private float lifetime; 

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (hit) return;

        float moveSpeed = speed * Time.deltaTime * direction;
        transform.Translate(moveSpeed, 0, 0);
        lifetime += Time.deltaTime;
        if (lifetime >= 2) 
        {
            Deactivate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        Deactivate();

        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Health>().TakeDamage(1f);
        }
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
