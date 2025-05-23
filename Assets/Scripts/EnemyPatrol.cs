using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement Parameters")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float speedVariance = 0.5f;

    [Header("Stuck Detection")]
    private float stuckTimer = 0f;
    [SerializeField] private float stuckCheckInterval = 0.1f;
    [SerializeField] private float stuckThreshold = 0.1f;

    private float prev_x;

    private Vector3 initScale;
    private bool movingLeft;

    [Header("Random Pause Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float pauseChance = 0.5f;
    [SerializeField] private float minWait = 0.2f;
    [SerializeField] private float maxWait = 1.0f;

    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;

    [Header("Pause Cooldown")]
    [SerializeField] private float waitCooldownDuration = 5f;
    private float waitCooldownTimer = 0f;

    private float speed;

    private void Awake()
    {
        initScale = enemy.localScale;
        speed = baseSpeed + Random.Range(-speedVariance, speedVariance);
        prev_x = enemy.position.x;
    }

    private void Update()
    {
        if (waitCooldownTimer > 0f)
            waitCooldownTimer -= Time.deltaTime;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
                waitTimer = 0f;
                speed = baseSpeed + Random.Range(-speedVariance, speedVariance);
                DirectionChange();
            }
            return;
        }

        if (movingLeft)
        {
            if (enemy.position.x > leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                if (Random.value < pauseChance && waitCooldownTimer <= 0f)
                    StartRandomWait();
                else
                    DirectionChange();
            }
        }
        else
        {
            if (enemy.position.x < rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                if (Random.value < pauseChance && waitCooldownTimer <= 0f)
                    StartRandomWait();
                else
                    DirectionChange();
            }
        }

        float movedDistance = Mathf.Abs(enemy.position.x - prev_x);
        if (movedDistance < stuckThreshold && !isWaiting)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckCheckInterval)
            {
                stuckTimer = 0f;
                DirectionChange();
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        prev_x = enemy.position.x;
    }

    private void MoveInDirection(int _direction)
    {
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
                                     enemy.position.y, enemy.position.z);
    }

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }

    private void StartRandomWait()
    {
        isWaiting = true;
        waitDuration = Random.Range(minWait, maxWait);
        waitCooldownTimer = waitCooldownDuration;
    }
}
