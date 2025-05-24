using System;
using UnityEngine;

[Serializable]
public class EnemyPatrollingStateData
{
    public AnimationClip walkingClip;
    public float walkSpeed;
    public float patrolRadius = 5f;
    public float arrivalThreshold = 0.2f;
    public float playerDetectionRange = 5f;
}

public class EnemyPatrollingState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyPatrollingStateData _data;
    private Vector3 _targetPosition;
    private bool _hasTarget;

    // For debug ray gizmo
    private Vector3 _rayOrigin;
    private Vector3 _rayDirection;
    private float _rayLength;

    public Vector3 RayOrigin => _rayOrigin;
    public Vector3 RayDirection => _rayDirection;
    public float RayLength => _rayLength;

    public EnemyPatrollingState(EnemyAI enemyAI, EnemyPatrollingStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
    }

    public void Enter()
    {
        Debug.Log("Enter Patrolling State");
        PickNewPatrolPoint();
    }

    public void StateUpdate()
    {
        // Optionally detect players here (not shown in this sample)
    }

    public void StateFixedUpdate()
    {
        if (!_hasTarget) return;

        Debug.Log("Patrolling State Fixed Update");

        Vector2 currentPos = _enemyAI.Rb.position;
        Vector2 direction = (_targetPosition - (Vector3)currentPos).normalized;

        // Obstacle detection
        float rayLength = _data.walkSpeed * Time.fixedDeltaTime + 0.2f;
        _rayOrigin = currentPos;
        _rayDirection = direction;
        _rayLength = rayLength;

        // Use Raycast2D for 2D physics
        RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, rayLength);
        if (hit.collider != null && !hit.collider.isTrigger && !hit.collider.CompareTag("Player"))
        {
            Debug.Log("Obstacle detected: " + hit.collider.name);
            _hasTarget = false;
            _enemyAI.StateMachine.ChangeState(_enemyAI.IdleState);
            return;
        }

        // Move toward target
        Vector2 newPosition = currentPos + direction * _data.walkSpeed * Time.fixedDeltaTime;
        _enemyAI.Rb.MovePosition(newPosition);

        // Check if arrived
        if (Vector2.Distance(currentPos, _targetPosition) <= _data.arrivalThreshold)
        {
            _hasTarget = false;
            _enemyAI.StateMachine.ChangeState(_enemyAI.IdleState);
        }
    }

    public void Exit()
    {
        _hasTarget = false;
    }

    private void PickNewPatrolPoint()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _data.patrolRadius;
        Vector2 basePosition = _enemyAI.transform.position;
        _targetPosition = new Vector2(basePosition.x + randomPoint.x, basePosition.y + randomPoint.y);
        _hasTarget = true;
        Debug.Log("Picked new patrol point: " + _targetPosition);
    }
}
