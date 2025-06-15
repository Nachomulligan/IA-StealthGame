using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStateIdleConfigurable<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private FlockingConfiguration _flockingConfig;
    private string _stateName = "Idle";

    public GoonStateIdleConfigurable(GoonEnemyModel goon, FlockingManager flockingManager,
        FlockingConfiguration flockingConfig)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _flockingConfig = flockingConfig;
    }

    public override void Enter()
    {
        Debug.Log("Entering Idle state - Doing nothing");
        base.Enter();
        _flockingManager.ApplyConfiguration(_flockingConfig, _stateName);
    }

    public override void Execute()
    {
        // Idle no hace nada
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exiting Idle state");
        _flockingManager.RestorePreviousState();
    }
}