using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    Dictionary<Vector3, int> _obs = new Dictionary<Vector3, int>();
    static ObstacleManager instance;
    public bool skipY = true;
    public static ObstacleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("ObstacleManager").AddComponent<ObstacleManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void AddColl(Collider coll)
    {
        var points = GetPointsOnCollider(coll, skipY);
        for (int i = 0; i < points.Count; i++)
        {
            if (_obs.ContainsKey(points[i]))
            {
                _obs[points[i]]++;
            }
            else
            {
                _obs[points[i]] = 1;
            }
        }
    }
    public void RemoveColl(Collider coll)
    {
        var points = GetPointsOnCollider(coll, skipY);
        for (int i = 0; i < points.Count; i++)
        {
            if (_obs.ContainsKey(points[i]))
            {
                _obs[points[i]] -= 1;
                if (_obs[points[i]] <= 0)
                {
                    _obs.Remove(points[i]);
                }
            }
        }
    }
    public bool IsRightPos(Vector3 curr)
    {
        curr = Vector3Int.RoundToInt(curr);
        if (skipY)
        {
            curr.y = 0;
        }
        return !_obs.ContainsKey(curr);
    }
    List<Vector3> GetPointsOnCollider(Collider coll, bool skipY = true)
    {
        List<Vector3> points = new List<Vector3>();
        Bounds bounds = coll.bounds;

        int minX = Mathf.FloorToInt(bounds.min.x);
        int maxX = Mathf.CeilToInt(bounds.max.x);

        int minY = skipY ? 0 : Mathf.FloorToInt(bounds.min.y);
        int maxY = skipY ? 0 : Mathf.CeilToInt(bounds.max.y);

        int minZ = Mathf.FloorToInt(bounds.min.z);
        int maxZ = Mathf.CeilToInt(bounds.max.z);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Vector3 point = new Vector3(x, y, z);
                    if (bounds.Contains(point))
                    {
                        points.Add(point);
                    }
                }
            }
        }

        return points;
    }
    private void OnDrawGizmosSelected()
    {
        if (_obs == null) return;
        Gizmos.color = Color.red;
        foreach (var item in _obs)
        {
            Gizmos.DrawWireSphere(item.Key, 0.25f);
        }
    }
}