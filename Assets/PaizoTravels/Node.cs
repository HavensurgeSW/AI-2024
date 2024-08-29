using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>();  // List of adjacent nodes
    public float distance = Mathf.Infinity;  // Distance from the start node, initialized to infinity
    public Node previousNode = null;  // Previous node in the shortest path

    public bool visited = false;  // Whether the node has been visited or not

    // Method to add a neighbor to the node
    public void AddNeighbor(Node neighbor)
    {
        if (!neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
            neighbor.neighbors.Add(this);  // Optionally, add this node to the neighbor's list as well
        }
    }

    // Reset the node's properties for a new pathfinding operation
    public void ResetNode()
    {
        distance = Mathf.Infinity;
        previousNode = null;
        visited = false;
    }
}