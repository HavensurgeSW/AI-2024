using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>();  // List of adjacent nodes
    public bool visited;                             // To check if the node has been visited

    public Vector2Int manhattanID;

    // Method to add a neighbor to the node
    public void AddNeighbor(Node neighbor)
    {
        if (!neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
            // Optionally, add this node to the neighbor's list as well
            neighbor.neighbors.Add(this);
        }
    }

    // Method to reset the visited state of the node
    public void ResetNode()
    {
        visited = false;
    }
}