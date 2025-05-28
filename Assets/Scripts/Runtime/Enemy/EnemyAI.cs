using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : NetworkBehaviour
{
    #region StateMachine Setup
    public StateMachine StateMachine { get; private set; }

    // State Data
    public EnemyIdleStateData IdleStateData;
    public EnemyPatrollingStateData PatrollingStateData;
    public EnemyChasingStateData ChasingStateData;

    // State References
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrollingState PatrollingState { get; private set; }
    public EnemyChasingState ChasingState { get; private set; }
    #endregion

    #region Components
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Animator Animator;
    #endregion

    #region Variables
    public string playerTag = "Player";
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    // Current target player transform (null if none)
    public Transform TargetPlayer { get; private set; }

    // For gizmo visualization
    public Vector2 DetectorOriginOffset; // Offset for the detection origin
    [HideInInspector] public Vector2 GizmoFanOrigin;
    [HideInInspector] public Vector2 GizmoFanDirection;
    [HideInInspector] public float GizmoFanRange;
    [HideInInspector] public float GizmoFanAngle;
    [HideInInspector] public int GizmoFanRayCount;

    public float Acceleration;

    [SerializeField]
    private bool _canMove = true; // Flag to control movement
    public bool CanMove
    {
        get => _canMove;
        set
        {
            _canMove = value;
            if(!_canMove)
                Rb.linearVelocity = Vector2.zero; // Stop movement when CanMove is false
        }
    }
    public bool CanAttack = true; // Flag to control attacking
    
    #endregion

    #region Projectile Setting
    [SerializeField] private GameObject projectilePrefab; 
    [SerializeField] private float projectileSpeed; 
    [SerializeField] private float _projectileDamage;
    #endregion

    #region Animations Variables
    public string HorizontalParameter 
    {
        get => "Horizontal";
    }
        
    public string VerticalParameter 
    {
        get => "Vertical";
    }
    public string SpeedParameter
    {
        get => "Speed";
    }

    public string AttackParameter
    {
        get => "Attack";
    }

    public string HurtParameter
    {
        get => "Hurt";
    }
    private Vector2 _lastMovement;
    public Vector2 LastMovement
    {
        get => _lastMovement;
        set
        {
            _lastMovement = value;
            // Clamp or round to -1, 0, or 1 to snap to a single direction
            float clampedX = Mathf.Abs(Mathf.Round(_lastMovement.x));
            float clampedY = Mathf.Round(_lastMovement.y);

            Animator.SetFloat(HorizontalParameter, clampedX);
            Animator.SetFloat(VerticalParameter, clampedY);
            if (_lastMovement.x > 0 && !IsFacingRight) 
            {
                IsFacingRight = true;
            }
            else if(_lastMovement.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
    }
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        set
        {
            _isFacingRight = value;
            transform.localScale = _isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }
    }
    #endregion

    #region Events
    #endregion
    private void Awake()
    {

        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();

        StateMachine = new StateMachine();

        IdleState = new EnemyIdleState(this, IdleStateData);
        PatrollingState = new EnemyPatrollingState(this, PatrollingStateData);
        ChasingState = new EnemyChasingState(this, ChasingStateData);
    }

    private void Start()
    {
        StateMachine.ChangeState(IdleState);
    }
    private void Update()
    {
        StateMachine.StateUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.StateFixedUpdate();
    }

    public void ApplyMovement(Vector2 direction, float speed, float acceleration)
    {
        if (!CanMove)
        {
            if (Rb.linearVelocity.magnitude > 0.1f)
                Rb.AddForce(Rb.linearVelocity * -acceleration, ForceMode2D.Force);
            else
                Rb.linearVelocity = Vector2.zero;
            return;
        }

        if (direction != Vector2.zero)
        {
            Rb.AddForce(direction * acceleration, ForceMode2D.Force);

            if (Rb.linearVelocity.magnitude > speed)
                Rb.linearVelocity = Rb.linearVelocity.normalized * speed;

            LastMovement = direction;
        }
        else
        {
            if (Rb.linearVelocity.magnitude > 0.1f)
                Rb.AddForce(Rb.linearVelocity * -acceleration, ForceMode2D.Force);
            else
                Rb.linearVelocity = Vector2.zero;
        }
    }


    public bool DetectPlayerFan(Vector2 origin, Vector2 forward, float range, float angle, int rayCount)
    {
        origin += DetectorOriginOffset;
        GizmoFanOrigin = origin;
        GizmoFanDirection = forward;
        GizmoFanRange = range;
        GizmoFanAngle = angle;
        GizmoFanRayCount = rayCount;

        float halfAngle = angle / 2f;
        float angleStep = angle / (rayCount - 1);
        float startAngle = -halfAngle;

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * forward;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, range, obstacleLayer | playerLayer);

            if (hit.collider != null && hit.collider.CompareTag(playerTag))
            {
                SetTargetPlayer(hit.collider.transform);
                return true;
            }
        }
        return false;
    }

    public void SetTargetPlayer(Transform player)
    {
        if (TargetPlayer != null) return;
        TargetPlayer = player;
    }

    public void ClearTargetPlayer()
    {
        TargetPlayer = null;
    }

    public void AttackTriggerByAnimationEvent()
    {

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        if (projectile.GetComponent<ArrowProjectile>() != null)
        {
            Vector2 AttackDirection = ((Vector2)TargetPlayer.position - Rb.position).normalized;
            projectile.GetComponent<ArrowProjectile>().DamageAmount = _projectileDamage;
            // Rotate projectile to face the direction
            float angle = Mathf.Atan2(AttackDirection.y, AttackDirection.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

            // Optionally, set velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.linearVelocity = AttackDirection.normalized * projectileSpeed;
        }
        else if (projectile.GetComponentInChildren<ExplosionProjectile>() != null)
        {
            FakeHeightObject fakeHeightObject = projectile.GetComponentInChildren<FakeHeightObject>();
            fakeHeightObject.Initialize(TargetPlayer.position);
        }

    }


    public void StopAllAction()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void StartAllAction()
    {
        CanMove = true;
        CanAttack = true;
    }

    public void EnemyOnHit(Vector2 knockBackDirection)
    {
        Animator.SetTrigger(HurtParameter);

        LastMovement = -knockBackDirection.normalized;

        StartCoroutine(ApplyKnockback(knockBackDirection));
    }

    private IEnumerator ApplyKnockback(Vector2 knockBackDirection)
    {
        yield return new WaitUntil(() => CanMove == false);

        Rb.AddForce(knockBackDirection, ForceMode2D.Impulse);
    }
    // Draw the fan rays in the editor for debugging
    private void OnDrawGizmos()
    {
        if (GizmoFanRayCount <= 1 || GizmoFanRange <= 0f)
            return;

        float halfAngle = GizmoFanAngle / 2f;
        float angleStep = GizmoFanAngle / (GizmoFanRayCount - 1);
        float startAngle = -halfAngle;

        for (int i = 0; i < GizmoFanRayCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = Quaternion.Euler(0, 0, currentAngle) * GizmoFanDirection;

            // Perform a raycast to check for obstacle or player
            RaycastHit2D hit = Physics2D.Raycast(GizmoFanOrigin, dir, GizmoFanRange, obstacleLayer | playerLayer);

            if (hit.collider != null)
            {
                // Change color if it hits a player
                if (hit.collider.CompareTag(playerTag))
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.gray;

                Gizmos.DrawRay(GizmoFanOrigin, dir * hit.distance);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(GizmoFanOrigin, dir * GizmoFanRange);
            }
        }

        
        if (StateMachine?.CurrentState == PatrollingState)
        {
            Gizmos.color = PatrollingState.RayHit ? Color.red : Color.cyan;
            Vector2 origin = PatrollingState.RayOrigin;
            Vector2 dir = PatrollingState.RayDirection.normalized;
            float length = PatrollingState.RayLength;
            Gizmos.DrawRay(origin, dir * length);
        }else if(StateMachine?.CurrentState == ChasingState)
        {
            Vector2 origin = ChasingState.RayOrigin + ChasingStateData.rayOriginOffset;

            for (int i = 0; i < ChasingState.RayDirections.Length; i++)
            {
                Vector2 dir = ChasingState.RayDirections[i];
                bool hit = ChasingState.RayHits[i];

                Gizmos.color = hit ? Color.red : Color.cyan;
                Gizmos.DrawRay(origin, dir * ChasingStateData.checkRadius);
            }
        }
    }

}
