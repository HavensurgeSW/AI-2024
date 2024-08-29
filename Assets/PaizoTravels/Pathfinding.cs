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
    public bool BreadthFirstSearch(Node startNode, Node destinationNode)
    {
        if (startNode == null || destinationNode == null) return false;

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(startNode);
        startNode.visited = true;

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();
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
                    neighbor.visited = true;  // Mark neighbor as visited
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Return false if destination node is not reachable
        return false;
    }

    public bool Dijkstra(Node startNode, Node destinationNode)
    {
        if (startNode == null || destinationNode == null) return false;

        // Initialize the priority queue (using a sorted list as a simple priority queue)
        SortedList<float, Node> priorityQueue = new SortedList<float, Node>();

        // Set the start node's distance to 0 and add it to the priority queue
        startNode.distance = 0;
        priorityQueue.Add(startNode.distance, startNode);

        while (priorityQueue.Count > 0)
        {
            // Get the node with the smallest distance (the first element in the sorted list)
            Node currentNode = priorityQueue.Values[0];
            priorityQueue.RemoveAt(0);

            // If we reach the destination node, we're done
            if (currentNode == destinationNode)
            {
                Debug.Log("Destination node reached!");
                return true;
            }

            currentNode.visited = true;

            // Explore neighbors
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.visited)
                {
                    // Calculate the tentative distance to the neighbor
                    float tentativeDistance = currentNode.distance + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                    if (tentativeDistance < neighbor.distance)
                    {
                        neighbor.distance = tentativeDistance;
                        neighbor.previousNode = currentNode;

                        // Add the neighbor to the priority queue or update its position
                        if (!priorityQueue.ContainsValue(neighbor))
                        {
                            priorityQueue.Add(neighbor.distance, neighbor);
                        }
                        else
                        {
                            priorityQueue.RemoveAt(priorityQueue.IndexOfValue(neighbor));
                            priorityQueue.Add(neighbor.distance, neighbor);
                        }
                    }
                }
            }
        }

        // Return false if the destination node is not reachable
        return false;
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