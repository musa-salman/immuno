using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private GameObject[] bullets;

    private PlayerMovment playerMovement;

    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovment>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer > attackCooldown)
        {
            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        cooldownTimer = 0;
        bullets[0].transform.position = bulletPoint.position;
        bullets[0].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

}