using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    // Depth-First Search implementation with destination node
    public bool DepthFirstSearch(Node startNode, Node destinationNode)
    {
        if (startNode == null || destinationNode == null) return false;

        Stack<Node> stack = new Stack<Node>();
        stack.Push(startNode);

        while (stack.Count > 0)
        {
            Node currentNode = stack.Pop();        

            if (!currentNode.visited)
            {
                currentNode.visited = true;
                Debug.Log($"Visited Node: {currentNode.gameObject.name}");

                // Check if we've reached the destination node
                if (currentNode == destinationNode)
                {
                    Debug.Log("Destination node reached!");
                    return true;
                }

                foreach (Node neighbor in currentNode.neighbors)
                {
                    if (!neighbor.visited)
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        // Return false if destination node is not reachable
        return false;
    }

    // Breadth-First Search (BFS) - Empty for now
    public void BreadthFirstSearch(Node startNode, Node destinationNode)
    {
        // BFS implementation will go here
    }

    // Dijkstra's Algorithm - Empty for now
    public void Dijkstra(Node startNode, Node destinationNode)
    {
        // Dijkstra's algorithm implementation will go here
    }

    // A* Algorithm - Empty for now
    public void AStar(Node startNode, Node destinationNode)
    {
        // A* algorithm implementation will go here
    }

    // Method to reset all nodes
    public void ResetNodes(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.ResetNode();
        }
    }
}