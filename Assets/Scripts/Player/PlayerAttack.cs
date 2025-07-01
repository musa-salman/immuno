using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private GameObject[] bullets;

    [SerializeField] private AudioClip bulletSound;

    private Animator animator;
    private PlayerMovement playerMovement;
    private PlayerHealth health;

    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer > SkillManager.Instance.GetEffectiveLevel(SkillManager.SkillType.AttackSpeed) &&
            health.CurrentHealth > 0 && playerMovement.CanAttack())
        {
            animator.SetTrigger("Attack");

            Attack();
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        SoundManager.instance.PlaySound(bulletSound);
        cooldownTimer = 0;
        bullets[FindBullet()].transform.position = bulletPoint.position;
        bullets[FindBullet()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindBullet()
    {
        for (int i = 0; i < bullets.Length; i++)
        {
            if (!bullets[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

}