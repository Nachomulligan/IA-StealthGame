using System.Collections.Generic;
using UnityEngine;

public class E1PatrolState<T> : Enemy1Base<T>
{
    private List<Transform> _waypoints = new List<Transform>();
    private int _currentWaypointIndex;
    public E1PatrolState(Transform waypointGroup)
    {
        foreach (Transform child in waypointGroup)
        {
            _waypoints.Add(child);
        }

        _currentWaypointIndex = 0;
    }
    public override void Execute()
    {
        base.Execute();

        if (_waypoints == null || _waypoints.Count == 0)
            return;

        Transform currentTarget = _waypoints[_currentWaypointIndex];
        Vector3 direction = currentTarget.position - _move.Position;

        _move.Move(direction.normalized);
        _look.LookDir(direction.normalized);

        if (Vector3.Distance(_move.Position, currentTarget.position) < 0.2f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Count;
        }
    }
}
