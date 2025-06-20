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

    private float speed;

    private void Awake()
    {
        initScale = enemy.localScale;
        speed = baseSpeed;
        prev_x = enemy.position.x;
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x > leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
        }
        else
        {
            if (enemy.position.x < rightEdge.position.x)
            {
                MoveInDirection(1);
            }
        }

        float movedDistance = Mathf.Abs(enemy.position.x - prev_x);
        if (movedDistance < stuckThreshold)
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
}
