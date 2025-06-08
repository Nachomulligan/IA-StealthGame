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
        _flockingManager.SetFlockingActive(FlockingType.Cohesion, true);
        _flockingManager.SetFlockingActive(FlockingType.Alignment, true);
        _flockingManager.SetFlockingActive(FlockingType.Avoidance, true);

        // Solo configurar leader si existe y es válido
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
            // Si no hay líder válido, desactivar comportamiento de líder
            _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        }

        // Multiplicadores normales para patrullar 
        _flockingManager.SetFlockingMultiplier(FlockingType.Cohesion, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Alignment, 1f);
        _flockingManager.SetFlockingMultiplier(FlockingType.Avoidance, 1f);
    }

    public override void Execute()
    {
        // Verificar si el líder sigue siendo válido durante la ejecución
        if (!IsLeaderValid())
        {
            // Si el líder se vuelve inválido durante la ejecución, desactivar su comportamiento
            _flockingManager.SetFlockingActive(FlockingType.Leader, false);
        }

        var steering = _flockingManager.GetDir();
        steering.y = 0;
        _goon.Move(steering);
    }

    public override void Exit()
    {
        base.Exit();
        // No desactivamos los comportamientos aquí porque otros estados los pueden usar 
    }

    // Método helper para verificar si el líder es válido
    private bool IsLeaderValid()
    {
        return _leaderTarget != null && _leaderTarget.gameObject != null;
    }

    // Método público para actualizar la referencia del líder desde fuera
    public void UpdateLeaderTarget(Rigidbody newLeader)
    {
        _leaderTarget = newLeader;
    }
}