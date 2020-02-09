using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public static class Pathfinder
{
     public static List<Node> GetPath(Node start, Node goal)
    {
        var path = new List<Node>();
        var frontier = new SimplePriorityQueue<Node>();
        var cost = new Dictionary<Node, int>();
        var from = new Dictionary<Node, Node>();

        cost.Add(start, 0);
        frontier.Enqueue(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            if (current == goal)
                break;

            foreach (var neighbor in current.GetNeighbors())
            {
                var newCost = cost[current] + 1;
                if (!cost.ContainsKey(neighbor) || newCost < cost[neighbor])
                {
                    cost[neighbor] = newCost;
                    frontier.Enqueue(neighbor, newCost);
                    from.Add(neighbor, current);
                }
            }
        }

        if (!from.ContainsKey(goal))
        {
            Debug.LogError("error, no path to goal");
        }
        else
        {
            path.Add(goal);
            var current = goal;
            while (from.ContainsKey(current))
            {
                path.Add(from[current]);
                current = from[current];

                if (current == start)
                    break;
            }
        }

        path.Reverse();
        return path;
    }
}