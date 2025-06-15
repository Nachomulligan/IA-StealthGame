using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStatePatrolConfigurable<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Rigidbody _leaderTarget;
    private FlockingConfiguration _flockingConfig;
    private string _stateName = "Patrol";

    public GoonStatePatrolConfigurable(GoonEnemyModel goon, Rigidbody leaderTarget,
        FlockingManager flockingManager, FlockingConfiguration flockingConfig)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _leaderTarget = leaderTarget;
        _flockingConfig = flockingConfig;
    }

    public override void Enter()
    {
        base.Enter();
        _flockingManager.ApplyConfiguration(_flockingConfig, _stateName);

        if (IsLeaderValid())
        {
            var leaderBehaviour = _flockingManager.GetFlocking(FlockingType.Leader) as LeaderBehaviour;
            if (leaderBehaviour != null)
            {
                leaderBehaviour.LeaderRb = _leaderTarget;
            }
        }
        else
        {
            _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        }
    }

    public override void Execute()
    {
        if (!IsLeaderValid())
        {
            _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        }

        var steering = _flockingManager.GetDir();
        steering.y = 0;
        _goon.Move(steering);
    }

    public override void Exit()
    {
        base.Exit();
        _flockingManager.RestorePreviousState();
    }

    private bool IsLeaderValid()
    {
        return _leaderTarget != null && _leaderTarget.gameObject != null;
    }
}