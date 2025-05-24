using System;
using UnityEngine;

[Serializable]
public class EnemyChasingStateData
{
    public AnimationClip runningClip;
}

public class EnemyChasingState : IState
{
    private readonly EnemyAI _enemyAI;
    private readonly EnemyChasingStateData _data;
    public EnemyChasingState(EnemyAI enemyAI, EnemyChasingStateData data)
    {
        _enemyAI = enemyAI;
        _data = data;
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void StateFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void StateUpdate()
    {
        throw new System.NotImplementedException();
    }
}
