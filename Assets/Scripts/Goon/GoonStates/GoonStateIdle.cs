using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStateIdle<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;

    public GoonStateIdle(GoonEnemyModel goon, FlockingManager flockingManager)
    {
        _flockingManager = flockingManager;
        _goon = goon;
    }

    public override void Enter()
    {
        Debug.Log("Entering Idle state - Doing nothing");
        base.Enter();
        _flockingManager.SaveCurrentState();

        _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        _flockingManager.SetFlockingActive(FlockingType.Predator, false);
        _flockingManager.SetFlockingActive(FlockingType.Seek, false);
        _flockingManager.SetFlockingActive(FlockingType.Alignment, false);
        _flockingManager.SetFlockingActive(FlockingType.Cohesion, false);
        _flockingManager.SetFlockingActive(FlockingType.Avoidance, false);
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exiting Idle state");

        _flockingManager.RestorePreviousState();
    }
}