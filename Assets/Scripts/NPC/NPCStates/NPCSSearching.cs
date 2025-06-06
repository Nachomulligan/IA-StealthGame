using System.Collections.Generic;
using UnityEngine;
public class NPCSSearching<T> : StateFollowPoints<T>
{
    private Transform _target;
    private bool _isPathfindingActive;
    private float _searchTimer;
    private float _searchDuration;
    private bool _searchOver;

    public NPCSSearching(Transform entity, float searchDuration = 5f, float distanceToPoint = 0.2f)
        : base(entity, distanceToPoint)
    {
        _searchDuration = searchDuration;
        _searchOver = false;
    }

    public override void Initialize(params object[] p)
    {
        base.Initialize(p);
        _move = p[0] as IMove;
        _look = p[1] as ILook;
    }

    public override void Enter()
    {
        base.Enter();
        _searchTimer = 0f;
        _searchOver = false;
        _isPathfindingActive = false;

        // Iniciar pathfinding hacia el target si existe
        if (_target != null)
        {
            StartPathfinding();
        }

        Debug.Log("Started searching for target");
    }

    public override void Execute()
    {
        base.Execute(); // Esto ejecuta Run() de StateFollowPoints

        _searchTimer += Time.deltaTime;

        // Si se acabó el tiempo de búsqueda
        if (_searchTimer >= _searchDuration)
        {
            if (!_searchOver) // Solo ejecutar una vez
            {
                _searchOver = true;
                _move.Move(Vector3.zero);
                Debug.Log("Search time over");
            }
            return;
        }

        // Si terminó el pathfinding pero aún hay tiempo de búsqueda
        if (IsFinishPath && _isPathfindingActive)
        {
            _isPathfindingActive = false;
            Debug.Log("Reached search location but target not found");
        }
    }

    protected override void OnMove(Vector3 dir)
    {
        base.OnMove(dir);
        // Solo moverse si no se acabó el tiempo de búsqueda
        if (!_searchOver && _move != null)
        {
            _move.Move(dir);
            if (_look != null)
            {
                _look.LookDir(dir);
            }
        }
    }

    protected override void OnStartPath()
    {
        base.OnStartPath();
        _isPathfindingActive = true;
        Debug.Log("Started pathfinding to last known target location");
    }

    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        Debug.Log("Arrived at search destination");
    }

    public override void Exit()
    {
        base.Exit();
        _searchTimer = 0f;
        _searchOver = false;
        _isPathfindingActive = false;
        if (_move != null)
        {
            _move.Move(Vector3.zero);
        }
    }

    // Método para establecer el objetivo de búsqueda (última posición conocida del target)
    public void SetSearchTarget(Transform target)
    {
        _target = target;
    }

    private void StartPathfinding()
    {
        if (_target == null)
        {
            Debug.LogWarning("Cannot start searching - no target set");
            _searchOver = true;
            return;
        }

        SetPathAStarPlusVector();
    }

    private void SetPathAStarPlusVector()
    {
        Vector3 init = Vector3Int.RoundToInt(_entity.transform.position);
        List<Vector3> path = ASTAR.Run<Vector3>(init, IsSatisfied, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        Debug.Log("Search Path " + path.Count);
        SetWaypoints(path);
    }

    bool InView(Vector3 grandparent, Vector3 child)
    {
        var diff = child - grandparent;
        return !Physics.Raycast(grandparent, diff.normalized, diff.magnitude, PathfindingConstants.obsMask);
    }

    float Heuristic(Vector3 current)
    {
        float distanceMultiplier = 1;
        float h = Vector3.Distance(current, _target.transform.position) * distanceMultiplier;
        return h;
    }

    float GetCost(Vector3 parent, Vector3 child)
    {
        float distanceMultiplier = 1;
        float cost = Vector3.Distance(parent, child) * distanceMultiplier;
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
        var neighbours = new List<Vector3>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue;
                var child = new Vector3(x, 0, z) + curr;
                if (ObstacleManager.Instance.IsRightPos(child))
                {
                    neighbours.Add(child);
                }
            }
        }
        return neighbours;
    }

    public bool IsSearchOver => _searchOver;
    public bool IsPathfindingActive => _isPathfindingActive;
}