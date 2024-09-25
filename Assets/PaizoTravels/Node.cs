using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    #region PATHFINDING_DATA
    [Header("Node Properties")]
    public List<Node> neighbors = new List<Node>();  // List of adjacent nodes
    public float distance = Mathf.Infinity;  // Distance from the start node, initialized to infinity
    public float cost = Mathf.Infinity;  // Cost (G score) for A* and Dijkstra algorithms
    public float heuristic = Mathf.Infinity;  // Heuristic (H score) for A* algorithm
    public Node previousNode = null;  // Previous node in the shortest path
    #endregion

    public Vector2Int mapPos = Vector2Int.zero;
    public bool isRoad;
    public bool visited = false;


    [Header("Game Properties")]
    [SerializeField] private Structure structure = null;
    private Material material = null;

    public void AddNeighbor(Node neighbor)
    {
        if (!neighbors.Contains(neighbor))
        {
            neighbors.Add(neighbor);
            neighbor.neighbors.Add(this);
        }
    }

    
    public void ResetNode()
    {
        distance = Mathf.Infinity;
        cost = Mathf.Infinity;
        heuristic = Mathf.Infinity;
        previousNode = null;
        visited = false;
        material = null;
    }

    public void SetStructure(Structure str)
    {
        structure = str;       
    }

    public void SetTown(TownCenter str) {
        structure = str;
    }

    public bool CheckForStructure() { return structure != null; }

    public void SetMaterial(Material mat) {
        material = mat;
    }

}