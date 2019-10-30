using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    private static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        Stack<Node> totalPath = new Stack<Node>();
        totalPath.Push(current);
        while(cameFrom.Keys.Contains(current))
        {
            current = cameFrom[current];
            totalPath.Push(current);
        }
        return new List<Node>(totalPath);
    }

    private static float heuristic(Node current, Node objective) => Mathf.Max(Mathf.Abs(current.X() - objective.X()), Mathf.Abs(current.Y() - objective.Y()));

    private static float distance(Node current, Node obj) => Mathf.Sqrt((Mathf.Pow(obj.X(), 2) - Mathf.Pow(current.X(), 2)) + (Mathf.Pow(obj.Y(), 2) - Mathf.Pow(current.Y(), 2)));

    public static List<Node> Path(Node start, Node objective, Node[] allNodes)
    {
        HashSet<Node> openSet = new HashSet<Node> { start };

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        gScore[start] = 0;
        foreach(Node node in allNodes)
        {
            if(node != start)
            {
                gScore[node] = float.MaxValue;
            }
        }
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        foreach (Node node in allNodes)
        {
            if (node != start)
            {
                fScore[node] = float.MaxValue;
            }
        }
        fScore[start] = heuristic(start, objective);

        while(openSet.Count != 0)
        {
            Node current = getMin(fScore);
            if(current == objective)
            {
                return ReconstructPath(cameFrom, current);
            }
            openSet.Remove(current);
            foreach(Node neighbour in current.neighbours)
            {
                float tentativeGscore = gScore[current] + distance(current, neighbour);
                if(tentativeGscore < gScore[neighbour])
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGscore;
                    fScore[neighbour] = gScore[neighbour] + heuristic(neighbour, objective);
                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return new List<Node>();
    }

    private static Node getMin(Dictionary<Node, float> dict)
    {
        (Node, float) min = (dict.First().Key, float.MaxValue);
        foreach(float val in dict.Values)
        {
            if(val < min.Item2)
            {
                min.Item1 = dict.FirstOrDefault(x => x.Value == val).Key;
                min.Item2 = val;
            }
        }

        return min.Item1;
    }
}
