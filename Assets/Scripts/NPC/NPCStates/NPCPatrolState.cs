using System.Collections.Generic;
using UnityEngine;

public class NPCPatrolState<T> : NPCSBase<T>
{
    private PatrolToWaypoints _patrol;

    public NPCPatrolState(PatrolToWaypoints patrol)
    {
        _patrol = patrol;
    }

    public override void Execute()
    {
        base.Execute();

        Vector3 dir = _patrol.GetDir();

        if (dir == Vector3.zero)
            return;

        _move.Move(dir);
        _look.LookDir(dir);
    }
}