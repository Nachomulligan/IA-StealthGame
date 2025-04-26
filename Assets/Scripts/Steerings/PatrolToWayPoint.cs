using System.Collections.Generic;
using UnityEngine;

public class PatrolToWaypoints : ISteering
{
    private List<Vector3> _waypoints;
    private int _currentIndex = 0;
    private Transform _self;
    private Vector3 _direction;
    private int _patrolDirection = 1; // 1 = forward, -1 = backward

    private float _threshold = 0.2f;

    private bool _completedCycle = false;

    public bool CompletedCycle => _completedCycle;

    public PatrolToWaypoints(List<Vector3> waypoints, Transform self, float threshold = 0.2f)
    {
        _waypoints = waypoints;
        _self = self;
        _threshold = threshold;
    }

    public Vector3 GetDir()
    {
        UpdateWaypointTarget();
        return _direction.normalized;
    }

    /// <summary>
    /// Actualiza la direccion hacia el waypoint actual y cambia al siguiente si esta lo suficientemente cerca.
    /// </summary>
    private void UpdateWaypointTarget()
    {
        if (_waypoints == null || _waypoints.Count == 0)
        {
            _direction = Vector3.zero;
            return;
        }

        Vector3 currentPosition = _self.position;
        Vector3 targetPosition = _waypoints[_currentIndex];
        targetPosition.y = currentPosition.y;

        if (Vector3.Distance(currentPosition, targetPosition) < _threshold)
        {
            AdvanceToNextWaypoint();
        }

        _direction = targetPosition - currentPosition;
        _direction.y = 0;
    }

    /// <summary>
    /// Avanza al siguiente waypoint segun la direccion de patrol, invirtiendo al llegar a los extremos.
    /// </summary>
    private void AdvanceToNextWaypoint()
    {
        _currentIndex += _patrolDirection;

        if (_currentIndex >= _waypoints.Count)
        {
            _currentIndex = _waypoints.Count - 2;
            _patrolDirection = -1;
            _completedCycle = true; // 🚩 Patrulla ida completada
        }
        else if (_currentIndex < 0)
        {
            _currentIndex = 1;
            _patrolDirection = 1;
            _completedCycle = true; // 🚩 Patrulla vuelta completada
        }
    }

    public int GetCurrentWaypointIndex()
    {
        return _currentIndex;
    }

    public int GetPatrolDirection()
    {
        return _patrolDirection;
    }

    public void ResetPatrol()
    {
        _currentIndex = 0;
        _patrolDirection = 1;
    }

    public void SetWaypoints(List<Vector3> newWaypoints)
    {
        _waypoints = newWaypoints;
        ResetPatrol();
    }
}