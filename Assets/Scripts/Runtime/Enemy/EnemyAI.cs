using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
    public Rigidbody2D Rb;
    private Animator _animator;

    #endregion


    private void Awake()
    {

        Rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

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
    public void StopMoving()
    {

    }

    private void OnDrawGizmos()
    {

        if (StateMachine?.CurrentState is EnemyPatrollingState patrolState)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(patrolState.RayOrigin, patrolState.RayOrigin + patrolState.RayDirection.normalized * patrolState.RayLength);
        }
    }
}
