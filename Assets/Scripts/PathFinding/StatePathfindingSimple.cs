using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StatePathfindingSimple<T> : StateFollowPoints<T>
{
    private IMove _move;
    private ILook _look;
    private Transform _target;
    private bool _isPathfindingActive;

    public StatePathfindingSimple(Transform entity, IMove move, Transform target, float distanceToPoint = 0.2f)
        : base(entity, distanceToPoint)
    {
        _move = move;
        _target = target;
        _isPathfindingActive = false;
    }
    public StatePathfindingSimple(Transform entity, IMove move, List<Vector3> waypoints, float distanceToPoint = 0.2f) : base(entity, waypoints, distanceToPoint)
    {
        _move = move;
    }
    public override void Enter()
    {
        base.Enter();
        if (_target != null)
        {
            StartPathfinding();
        }
    }

    public override void Execute()
    {
        base.Execute();

        // Si terminó el path, el estado se mantiene pero no hace nada hasta que se inicie un nuevo pathfinding
        if (IsFinishPath && _isPathfindingActive)
        {
            _isPathfindingActive = false;
            Debug.Log("Pathfinding completed - reached goal");
        }
    }

    protected override void OnMove(Vector3 dir)
    {
        base.OnMove(dir);
        _move.Move(dir);
        //_look.LookDir(dir);
    }

    protected override void OnStartPath()
    {
        base.OnStartPath();
        _isPathfindingActive = true;
        Debug.Log("Started pathfinding to target");
    }

    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        Debug.Log("Finished pathfinding - arrived at destination");
    }

    public void StartPathfinding()
    {
        if (_target == null)
        {
            Debug.LogWarning("Cannot start pathfinding - no target set");
            return;
        }

        SetPathAStarPlusVector();
    }

    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    private void SetPathAStarPlusVector()
    {
        Vector3 init = Vector3Int.RoundToInt(_entity.transform.position);
        List<Vector3> path = ASTAR.Run<Vector3>(init, IsSatisfied, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        Debug.Log("Path " + path.Count);
        SetWaypoints(path);
    }

    bool InView(Vector3 grandparent, Vector3 child)
    {
        Debug.Log("INVIEW");
        var diff = child - grandparent;
        return !Physics.Raycast(grandparent, diff.normalized, diff.magnitude, PathfindingConstants.obsMask);
    }

    float Heuristic(Vector3 current)
    {
        float distanceMultiplier = 1;

        float h = 0;
        h += Vector3.Distance(current, _target.transform.position) * distanceMultiplier;
        return h;
    }

    float GetCost(Vector3 parent, Vector3 child)
    {
        float distanceMultiplier = 1;

        float cost = 0;
        cost += Vector3.Distance(parent, child) * distanceMultiplier;
        return cost;
    }

    bool IsSatisfied(Vector3 curr)
    {
        var targetPos = _target.position;
        targetPos.y = curr.y;
        if (Vector3.Distance(curr, targetPos) > 1.25f) return false;
        return InView(curr, _target.transform.position);
    }

    List<Vector3> GetConnections(Vector3 curr)
    {
        var neightbourds = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue;
                //if (x == z || x == -z) continue;
                var child = new Vector3(x, 0, z) + curr;
                if (ObstacleManager.Instance.IsRightPos(child))
                {
                    neightbourds.Add(child);
                }
            }
        }
        return neightbourds;
    }

    public bool IsPathfindingActive => _isPathfindingActive;
}