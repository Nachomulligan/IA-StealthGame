using System.Collections.Generic;
using System;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public static List<T> Run<T>(T start, Func<T, bool> isSatisfied, Func<T, List<T>> getConnections, Func<T, T, float> getCost, Func<T, float> heuristic, int watchdog = 500, int watchdogPath = 500)
    {
        Dictionary<T, T> parents = new Dictionary<T, T>();
        PriorityQueue<T> pending = new PriorityQueue<T>();
        HashSet<T> visited = new HashSet<T>();
        Dictionary<T, float> cost = new Dictionary<T, float>();
        cost[start] = 0;
        pending.Enqueue(start, 0);
        while (!pending.IsEmpty)
        {
            watchdog--;
            if (watchdog <= 0) break;
            T current = pending.Dequeue();
            Debug.Log("AStar");
            if (isSatisfied(current))
            {
                List<T> path = new List<T>();
                path.Add(current);
                while (parents.ContainsKey(path[path.Count - 1]))
                {
                    watchdogPath--;
                    if (watchdogPath <= 0) break;
                    path.Add(parents[path[path.Count - 1]]);
                }
                path.Reverse();
                return path;
            }
            else
            {
                visited.Add(current);
                List<T> connections = getConnections(current);

                for (int i = 0; i < connections.Count; i++)
                {
                    T child = connections[i];
                    if (visited.Contains(child)) continue;
                    float currentCost = cost[current] + getCost(current, child);
                    if (cost.ContainsKey(child) && currentCost > cost[child]) continue;

                    cost[child] = currentCost;
                    pending.Enqueue(child, currentCost + heuristic(child));
                    parents[child] = current;
                }
            }
        }

        return new List<T>();
    }
    public static List<T> CleanPath<T>(List<T> path, Func<T, T, bool> inView)
    {
        if (path == null) return path;
        if (path.Count <= 2) return path;
        var newPath = new List<T>();
        newPath.Add(path[0]);

        for (int i = 2; i < path.Count; i++)
        {
            var last = newPath[newPath.Count - 1];
            if (!inView(last, path[i]))
            {
                newPath.Add(path[i - 1]);
            }
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
}
