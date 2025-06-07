using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStateSteering<T> : State<T>
{
    FlockingManager _flockingManager;
    GoonEnemyModel _goon;
    Rigidbody _target;

    public GoonStateSteering(GoonEnemyModel goon, Rigidbody target, FlockingManager flockingManager)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _target = target;
    }

    public override void Enter()
    {
        base.Enter();

        _flockingManager.SetFlockingActive(FlockingType.Leader, true);

        var leaderBehaviour = _flockingManager.GetFlocking(FlockingType.Leader) as LeaderBehaviour;
        if (leaderBehaviour != null)
        {
            leaderBehaviour.LeaderRb = _target;
        }
        _flockingManager.SetFlockingMultiplier(FlockingType.Leader, 2f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 1.5f);
    }

    public override void Execute()
    {
        var steering = _flockingManager.GetDir();
        steering.y = 0;
        _goon.Move(steering);
    }

    public override void Exit()
    {
        base.Exit();

        _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        _flockingManager.SetFlockingMultiplier(FlockingType.Leader, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 1f);
    }
}