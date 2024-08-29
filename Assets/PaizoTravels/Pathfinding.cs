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