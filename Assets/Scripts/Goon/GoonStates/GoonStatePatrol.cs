using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStatePatrol<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Rigidbody _leaderTarget;

    public GoonStatePatrol(GoonEnemyModel goon, Rigidbody leaderTarget, FlockingManager flockingManager)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _leaderTarget = leaderTarget;
    }

    public override void Enter()
    {
        base.Enter();
        _flockingManager.SaveCurrentState();

        // Configurar flockings para patrol
        _flockingManager.SetFlockingActive(FlockingType.Cohesion, true);
        _flockingManager.SetFlockingActive(FlockingType.Alignment, true);
        _flockingManager.SetFlockingActive(FlockingType.Avoidance, true);

        if (IsLeaderValid())
        {
            _flockingManager.SetFlockingActive(FlockingType.Leader, true);
            var leaderBehaviour = _flockingManager.GetFlocking(FlockingType.Leader) as LeaderBehaviour;
            if (leaderBehaviour != null)
            {
                leaderBehaviour.LeaderRb = _leaderTarget;
            }
            _flockingManager.SetFlockingMultiplier(FlockingType.Leader, 15f);
        }
        else
        {
            _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        }

        _flockingManager.SetFlockingMultiplier(FlockingType.Cohesion, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Alignment, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 3f);
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

    public void UpdateLeaderTarget(Rigidbody newLeader)
    {
        _leaderTarget = newLeader;
    }
}
