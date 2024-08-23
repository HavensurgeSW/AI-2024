using System.Collections.Generic;
using UnityEngine;

public class Node<T>
{
    public T Value { get; set; }
    public Vector2Int Position { get; set; }
    public int Cost { get; set; }
    public int Heuristic { get; set; }
    public Node<T> Parent { get; set; }

    public List<Node<T>> Neighbors { get; private set; } = new List<Node<T>>();

    public Node(T value, Vector2Int position)
    {
        Value = value;
        Position = position;
    }

    // Add a neighbor to this node
    public void AddNeighbor(Node<T> neighbor)
    {
        Neighbors.Add(neighbor);
    }
}