using System.Collections;
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

    [Header("Stuck Detection")]
    [SerializeField] private float stuckTimeout = 1f;        
    [SerializeField] private float movementThreshold = 0.01f;

    private float stuckTimer = 0f;
    private float prev_x;
    private float speed;

    private Vector3 initScale;
    private bool movingLeft;
    private bool isPaused = false;

    private void Awake()
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy Transform not assigned!");
            enabled = false;
            return;
        }

        initScale = enemy.localScale;
        speed = baseSpeed;
        prev_x = enemy.position.x;
    }

    private void Update()
    {
        if (isPaused) return;

        PatrolLogic();
        DetectStuck();
    }

    private void PatrolLogic()
    {
        if (movingLeft)
        {
            if (leftEdge == null || enemy.position.x > leftEdge.position.x)
            {
                MoveInDirection(-1);
            }
            else
            {
                DirectionChange();
            }
        }
        else
        {
            if (rightEdge == null || enemy.position.x < rightEdge.position.x)
            {
                MoveInDirection(1);
            }
            else
            {
                DirectionChange(); 
            }
        }
    }

    private void DetectStuck()
    {
        float movedDistance = Mathf.Abs(enemy.position.x - prev_x);

        if (movedDistance < movementThreshold)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= stuckTimeout)
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

    private void MoveInDirection(int direction)
    {
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * direction, initScale.y, initScale.z);
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * direction * speed,
                                     enemy.position.y, enemy.position.z);
    }

    private void DirectionChange()
    {
        movingLeft = !movingLeft;
    }

    public void PausePatrol(float duration)
    {
        StartCoroutine(PauseCoroutine(duration));
    }

    private IEnumerator PauseCoroutine(float duration)
    {
        isPaused = true;
        yield return new WaitForSeconds(duration);
        isPaused = false;
    }

    public void StopMovement() => isPaused = true;
    public void ResumeMovement() => isPaused = false;
}
