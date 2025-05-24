using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyIdleStateData
{
    public float StandStillTime; // Time to stand still before moving

    public AnimationClip idleUpClip;
    public AnimationClip idleDownClip;
    public AnimationClip idleSideClip;
}

public class EnemyIdleState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyIdleStateData _data;
    private float _standStillTimer;
    public EnemyIdleState(EnemyAI enemyAI, EnemyIdleStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
    }
    public void Enter()
    {
        _standStillTimer = 0;
        Debug.Log("Enter Idle State");

    }
    public void StateUpdate()
    {
        if(_standStillTimer >= _data.StandStillTime)
        {
            // Transition to the next state (e.g., patrolling or chasing)
            _enemyAI.StateMachine.ChangeState(_enemyAI.PatrollingState);
            return;
        }
        else _standStillTimer += Time.deltaTime;
    }
    public void StateFixedUpdate()
    {

    }

    public void Exit()
    {
        
    }

    
}
