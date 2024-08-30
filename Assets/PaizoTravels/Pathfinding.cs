using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
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

        
        return false;
    }

   
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

          
            if (currentNode == destinationNode)
            {
                Debug.Log("Destination node reached!");
                return true;
            }

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.visited)
                {
                    neighbor.visited = true;  
                    queue.Enqueue(neighbor);
                }
            }
        }

        
        return false;
    }

    public bool Dijkstra(Node startNode, Node destinationNode)
    {
        if (startNode == null || destinationNode == null) return false;


        //SortedList esta bueno para pre-loaded data, es un toque mas lenta para agregar cosas durante runtime
        SortedList<float, Node> priorityQueue = new SortedList<float, Node>();

        startNode.distance = 0;
        priorityQueue.Add(startNode.distance, startNode);

        while (priorityQueue.Count > 0)
        {
     
            Node currentNode = priorityQueue.Values[0];
            priorityQueue.RemoveAt(0);

            
            if (currentNode == destinationNode)
            {
                Debug.Log("Destination node reached!");
                return true;
            }

            currentNode.visited = true;

            
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (!neighbor.visited)
                {
                   
                    float tentativeDistance = currentNode.distance + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                    if (tentativeDistance < neighbor.distance)
                    {
                        neighbor.distance = tentativeDistance;
                        neighbor.previousNode = currentNode;

                        
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
        return false;
    }

    public bool AStar(Node startNode, Node destinationNode)
    {
        if (startNode == null || destinationNode == null) return false;

        // Initialize the open and closed lists
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        // Initialize the start node
        startNode.cost = 0;
        startNode.heuristic = Heuristic(startNode, destinationNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Get the node with the lowest F score (F = G + H)
            Node currentNode = GetNodeWithLowestFScore(openList);

            // If we reach the destination node, we're done
            if (currentNode == destinationNode)
            {
                Debug.Log("Destination node reached!");
                return true;
            }
           
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            currentNode.visited = true;

            // Explore neighbors
            foreach (Node neighbor in currentNode.neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue; // Skip already evaluated nodes
                }

                float tentativeCost = currentNode.cost + Vector3.Distance(currentNode.transform.position, neighbor.transform.position);

                if (!openList.Contains(neighbor) || tentativeCost < neighbor.cost)
                {
                    neighbor.previousNode = currentNode;
                    neighbor.cost = tentativeCost;
                    neighbor.heuristic = Heuristic(neighbor, destinationNode);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // Return false if the destination node is not reachable
        return false;
    }

    // Heuristic function: Using Euclidean distance as a heuristic
    private float Heuristic(Node node, Node destinationNode)
    {
        return Vector3.Distance(node.transform.position, destinationNode.transform.position);
    }

    // Get the node with the lowest F score from the open list
    private Node GetNodeWithLowestFScore(List<Node> openList)
    {
        Node lowestFScoreNode = openList[0];

        foreach (Node node in openList)
        {
            float fScore = node.cost + node.heuristic;
            float lowestFScore = lowestFScoreNode.cost + lowestFScoreNode.heuristic;

            if (fScore < lowestFScore)
            {
                lowestFScoreNode = node;
            }
        }

        return lowestFScoreNode;
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