using System.Collections.Generic;
using UnityEngine;

public class CustomPathfinding  
{
    private LayerMask wallMask;

    public CustomPathfinding(LayerMask mask)
    {
        wallMask = mask;
    }

    public List<CustomNode> AStar(CustomNode start, CustomNode goal)
    {
        PriorityQueue<CustomNode> frontier = new PriorityQueue<CustomNode>();
        frontier.Enqueue(start, 0);

        Dictionary<CustomNode, CustomNode> cameFrom = new Dictionary<CustomNode, CustomNode>();
        cameFrom.Add(start, null);

        Dictionary<CustomNode, int> costSoFar = new Dictionary<CustomNode, int>();
        costSoFar.Add(start, 0);

        CustomNode current = default;

        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == goal) break;

            foreach (var next in current.neighbours)
            {
                if (next.nodeEvent.GetPersistentEventCount() > 0 && !next.canDoEvent) continue;
                

                int newCost = costSoFar[current] + next.cost;

                if (!costSoFar.ContainsKey(next.node))
                {
                    costSoFar.Add(next.node, newCost);
                    frontier.Enqueue(next.node, newCost + Heuristic(next.node.transform.position, goal.transform.position));
                    cameFrom.Add(next.node, current);
                }
                else if (newCost < costSoFar[current])
                {
                    frontier.Enqueue(next.node, newCost + Heuristic(next.node.transform.position, goal.transform.position));
                    costSoFar[next.node] = newCost;
                    cameFrom[next.node] = current;
                }
            }
        }
        List<CustomNode> path = new List<CustomNode>();
        if (current != goal) return path;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();
        
        return path;
    }


    readonly List<CustomNode> EMPTY = new List<CustomNode>();


    float Heuristic(Vector2 start, Vector2 end)
    {
        return Vector3.Distance(start, end);
    }

   /* public List<Vector2> ThetaStar(CustomNode start, CustomNode goal, Vector2 nextPos)
    {
        if (start == null || goal == null) return EMPTY;

        var path = AStar(start, goal);
        path.Add(start.transform.position);
        path.Reverse();
        path.Add(nextPos);
        path.Reverse();

        int current = 0;

        while (current + 2 < path.Count)
        {
            if (InLOSTool.InLOS(path[current], path[current + 2], wallMask))
                path.RemoveAt(current + 1);
            else
                current++;
        }


        return path;

    }

    public List<Vector2> ThetaStar(CustomNode start, CustomNode goal)
    {
        if (start == null || goal == null) return EMPTY;

        var path = AStar(start, goal);
        path.Add(start.transform.position);       

        int current = 0;

        while (current + 2 < path.Count)
        {
            if (InLOSTool.InLOS(path[current], path[current + 2], wallMask))
                path.RemoveAt(current + 1);
            else
                current++;
        }


        return path;

    }*/
}
