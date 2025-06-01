using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStateSteering<T> : State<T>
{
    ISteering _steering;
    GoonEnemy _magikarp;
    ObstacleAvoidance _obs;
    Rigidbody _target;
    LeaderBehaviour _leaderBehaviour;
    public GoonStateSteering(GoonEnemy goon, LeaderBehaviour leaderBehaviour, Rigidbody target, ISteering steering, ObstacleAvoidance obs)
    {
        _steering = steering;
        _magikarp = goon;
        _obs = obs;
        _target = target;
        _leaderBehaviour = leaderBehaviour;
    }
    public override void Enter()
    {
        base.Enter();
        _leaderBehaviour.IsActive = true;
        _leaderBehaviour.LeaderRb = _target;
    }
    public override void Execute()
    {
        var steering = _steering.GetDir();
        steering.y = 0;
        var dir = _obs.GetDir(steering, false);
        _magikarp.Move(dir);
        _magikarp.LookDir(dir);
    }
    public override void Exit()
    {
        base.Exit();
        _leaderBehaviour.IsActive = false;
    }
}
