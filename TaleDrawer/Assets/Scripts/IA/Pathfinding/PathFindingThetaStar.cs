using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingThetaStar
{
    private LayerMask wallMask; 

    public PathFindingThetaStar(LayerMask mask)
    {
        wallMask = mask;
    }

    public List<Vector3> AStar(Node start, Node goal)
    {
        PriorityQueue<Node> frontier = new PriorityQueue<Node>();
        frontier.Enqueue(start, 0);

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);

        Dictionary<Node, int> costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(start, 0);

        Node current = default;

        while (frontier.Count != 0)
        {
            current = frontier.Dequeue();

            if (current == goal) break;

            foreach (var next in current.neighbours)
            {
                

                int newCost = costSoFar[current] + next.cost;

                if (!costSoFar.ContainsKey(next))
                {
                    costSoFar.Add(next, newCost);
                    frontier.Enqueue(next, newCost + Heuristic(next.transform.position, goal.transform.position));
                    cameFrom.Add(next, current);
                }
                else if (newCost < costSoFar[current])
                {
                    frontier.Enqueue(next, newCost + Heuristic(next.transform.position, goal.transform.position));
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }
        List<Vector3> path = new List<Vector3>();
        if (current != goal) return path;

        while (current != start)
        {
            path.Add(current.transform.position);
            current = cameFrom[current];
        }
        
        path.Reverse();
        return path;
    }


    readonly List<Vector3> EMPTY = new List<Vector3>();


    float Heuristic(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end);
    }

    public List<Vector3> ThetaStar(Node start, Node goal)
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

    }
}
