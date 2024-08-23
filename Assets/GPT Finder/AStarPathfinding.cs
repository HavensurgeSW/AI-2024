using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public List<Node<object>> FindPath(Node<object> startNode, Node<object> targetNode)
    {
        var openSet = new HashSet<Node<object>>();
        var closedSet = new HashSet<Node<object>>();
        var cameFrom = new Dictionary<Node<object>, Node<object>>();
        var gScore = new Dictionary<Node<object>, int>();
        var fScore = new Dictionary<Node<object>, int>();

        openSet.Add(startNode);
        gScore[startNode] = 0;
        fScore[startNode] = HeuristicCost(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node<object> current = GetLowestFScoreNode(openSet, fScore);

            if (current.Equals(targetNode))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in current.Neighbors)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeGScore = gScore[current] + CostToMoveToNeighbor(current, neighbor);

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, int.MaxValue))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCost(neighbor, targetNode);
            }
        }

        // Return an empty path if no path is found
        return new List<Node<object>>();
    }

    private int HeuristicCost(Node<object> nodeA, Node<object> nodeB)
    {
        // Example heuristic: Manhattan distance for grid-based pathfinding
        return Mathf.Abs(nodeA.Position.x - nodeB.Position.x) + Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
    }

    private int CostToMoveToNeighbor(Node<object> nodeA, Node<object> nodeB)
    {
        // Example movement cost
        return 1;
    }

    private Node<object> GetLowestFScoreNode(HashSet<Node<object>> openSet, Dictionary<Node<object>, int> fScore)
    {
        Node<object> lowest = null;
        int lowestScore = int.MaxValue;

        foreach (var node in openSet)
        {
            int score = fScore.GetValueOrDefault(node, int.MaxValue);
            if (score < lowestScore)
            {
                lowestScore = score;
                lowest = node;
            }
        }

        return lowest;
    }

    private List<Node<object>> ReconstructPath(Dictionary<Node<object>, Node<object>> cameFrom, Node<object> current)
    {
        var path = new List<Node<object>> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}