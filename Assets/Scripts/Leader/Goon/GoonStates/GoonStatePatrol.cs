using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoonStatePatrol<T> : State<T>
{
    private FlockingManager _flockingManager;
    private GoonEnemyModel _goon;
    private Rigidbody _leaderTarget; // Target para seguir como líder

    public GoonStatePatrol(GoonEnemyModel goon, Rigidbody leaderTarget, FlockingManager flockingManager)
    {
        _flockingManager = flockingManager;
        _goon = goon;
        _leaderTarget = leaderTarget;
    }

    public override void Enter()
    {
        base.Enter();

        // Activar comportamientos de flocking para patrullar
        _flockingManager.SetFlockingActive(FlockingType.Leader, true);
        _flockingManager.SetFlockingActive(FlockingType.Cohesion, true);
        _flockingManager.SetFlockingActive(FlockingType.Alignment, true);
        _flockingManager.SetFlockingActive(FlockingType.Avoidance, true);

        // Configurar el líder
        var leaderBehaviour = _flockingManager.GetFlocking(FlockingType.Leader) as LeaderBehaviour;
        if (leaderBehaviour != null)
        {
            leaderBehaviour.LeaderRb = _leaderTarget; // Usar leaderTarget para seguir
        }

        // Multiplicadores normales para patrullar
        _flockingManager.SetFlockingMultiplier(FlockingType.Leader, 1.5f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Cohesion, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Alignment, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 1f);
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
        // No desactivamos los comportamientos aquí porque otros estados los pueden usar
    }
}