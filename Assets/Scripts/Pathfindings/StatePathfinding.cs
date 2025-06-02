using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.PlayerSettings;

public class StatePathfinding<T> : StateFollowPoints<T>
{
    IMove _move;
    Animator _anim;
    public Node start;
    public Node goal;
    public Transform target;

    public StatePathfinding(Transform entity, IMove move, Animator anim, Transform target, float distanceToPoint = 0.2F) : base(entity, distanceToPoint)
    {
        _move = move;
        _anim = anim;
        this.target = target;
    }
    public StatePathfinding(Transform entity, IMove move, Animator anim, List<Vector3> waypoints, float distanceToPoint = 0.2f) : base(entity, waypoints, distanceToPoint)
    {
        _move = move;
        _anim = anim;
    }

    protected override void OnMove(Vector3 dir)
    {
        base.OnMove(dir);
        _move.Move(dir);
        _move.LookDir(dir);
    }
    protected override void OnStartPath()
    {
        base.OnStartPath();
        //_move.SetPosition(_waypoints[0]);
        _anim.SetFloat("Vel", 1);
    }
    protected override void OnFinishPath()
    {
        base.OnFinishPath();
        _anim.SetFloat("Vel", 0);
    }
    public void SetPathBFS()
    {
        List<Node> path = BFS.Run<Node>(start, IsSatisfied, GetConnections, 5000);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        _move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }
    public void SetPathDFS()
    {
        List<Node> path = DFS.Run<Node>(start, IsSatisfied, GetConnections, 5000);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        _move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }
    public void SetPathDijkstra()
    {
        List<Node> path = DIJKSTRA.Run<Node>(start, IsSatisfied, GetConnections, GetCost);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        _move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }
    public void SetPathAStar()
    {
        var init = GetNearNode(_entity.transform.position);
        goal = GetNearNode(target.transform.position);
        List<Node> path = ASTAR.Run<Node>(init, IsSatisfied, GetConnections, GetCost, Heuristic);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        //_move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }
    public void SetPathAStarPlus()
    {
        var init = GetNearNode(_entity.transform.position);
        goal = GetNearNode(target.transform.position);
        List<Node> path = ASTAR.Run<Node>(init, IsSatisfied, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        //_move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }

    public void SetPathAStarPlusVector()
    {
        //goal = Vector3Int.RoundToInt(target.transform.position);
        Vector3 init = Vector3Int.RoundToInt(_entity.transform.position);
        List<Vector3> path = ASTAR.Run<Vector3>(init, IsSatisfied, GetConnections, GetCost, Heuristic);
        path = ASTAR.CleanPath(path, InView);
        Debug.Log("Path " + path.Count);
        //_move.SetPosition(start.transform.position);
        SetWaypoints(path);
    }

    public void SetPathThetaStar()
    {
        var init = GetNearNode(_entity.transform.position);
        goal = GetNearNode(target.transform.position);
        List<Node> path = THETA.Run<Node>(init, IsSatisfied, GetConnections, GetCost, Heuristic, InView);
        List<Vector3> pathVector = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            pathVector.Add(path[i].transform.position);
        }
        Debug.Log("Path " + path.Count);
        //_move.SetPosition(start.transform.position);
        SetWaypoints(pathVector);
    }
    bool InView(Node grandparent, Node child)
    {
        return InView(grandparent.transform.position, child.transform.position);
    }
    bool InView(Vector3 grandparent, Vector3 child)
    {
        Debug.Log("INVIEW");
        var diff = child - grandparent;
        return !Physics.Raycast(grandparent, diff.normalized, diff.magnitude, PathfindingConstants.obsMask);
    }
    Node GetNearNode(Vector3 position)
    {
        Collider[] nodes = Physics.OverlapSphere(position, PathfindingConstants.nearRadius, PathfindingConstants.nodeMask);
        Node nearNode = null;
        float nearDistance = Mathf.Infinity;
        for (int i = 0; i < nodes.Length; i++)
        {
            var currNode = nodes[i].GetComponent<Node>();
            if (currNode == null) continue;

            var dir = currNode.transform.position - position;
            var currDistance = dir.magnitude;
            if (Physics.Raycast(position, dir.normalized, currDistance, PathfindingConstants.obsMask)) continue;
            if (nearNode == null || nearDistance > currDistance)
            {
                nearNode = currNode;
                nearDistance = currDistance;
            }
        }
        Debug.Log(nearNode);
        return nearNode;
    }
    float Heuristic(Node current)
    {
        float distanceMultiplier = 1;

        float h = 0;
        h += Vector3.Distance(current.transform.position, goal.transform.position) * distanceMultiplier;
        return h;
    }
    float Heuristic(Vector3 current)
    {
        float distanceMultiplier = 1;

        float h = 0;
        h += Vector3.Distance(current, target.transform.position) * distanceMultiplier;
        return h;
    }
    float GetCost(Node parent, Node child)
    {
        float distanceMultiplier = 1;
        float trapMultiplier = 100;

        float cost = 0;
        cost += Vector3.Distance(parent.transform.position, child.transform.position) * distanceMultiplier;
        cost += child.hasTrap ? trapMultiplier : 0;
        return cost;
    }
    float GetCost(Vector3 parent, Vector3 child)
    {
        float distanceMultiplier = 1;

        float cost = 0;
        cost += Vector3.Distance(parent, child) * distanceMultiplier;
        return cost;
    }
    bool IsSatisfied(Node curr)
    {
        return curr == goal;
    }
    bool IsSatisfied(Vector3 curr)
    {
        if (Vector3.Distance(curr, target.transform.position) > 1.25f) return false;
        return InView(curr, target.transform.position);
    }
    List<Node> GetConnections(Node curr)
    {
        return curr.neightbourds;
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
}
